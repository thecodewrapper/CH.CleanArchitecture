using CH.CleanArchitecture.Common;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CH.CleanArchitecture.Core.Application
{
    public abstract class BaseCommandHandler<TRequest, TResponse> : BaseMessageHandler<TRequest, TResponse>
        where TRequest : BaseCommand<TResponse>
        where TResponse : class, IResult
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly ILogger _logger;
        private readonly IValidator<TRequest> _validator;

        public BaseCommandHandler(IServiceProvider serviceProvider) {
            _authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
            _authenticatedUserService = serviceProvider.GetRequiredService<IAuthenticatedUserService>();
            _logger = serviceProvider.GetRequiredService<ILogger<TRequest>>();
            _validator = serviceProvider.GetService<IValidator<TRequest>>();
        }

        public override async Task Consume(ConsumeContext<TRequest> context) {
            var requirements = context.Message.Requirements;

            // Checking authorization requirements
            if (requirements.Any()) {
                var user = _authenticatedUserService.User;
                var authorizationResult = await _authorizationService.AuthorizeAsync(user, null, requirements);

                if (!authorizationResult.Succeeded) {
                    _logger.LogError($"Authorization failed for {context.Message.GetType()}"); //push properties for failure reasons here
                    await context.RespondAsync(new Result().Fail().WithError("Authorization failed"));
                    return;
                }
            }

            // Perform validation, if any
            if (_validator != null) {
                _logger.LogDebug($"Validating command {context.Message.GetType()}");
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(context.Message);

                if (!validationResult.IsValid) {
                    string validationErrorMessage = string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogError($"Validation failed for {context.Message.GetType()}. Validation Error: {validationErrorMessage}");
                    await context.RespondAsync(new Result().Fail().WithMessage(validationErrorMessage));
                    return;
                }
            }

            _logger.LogDebug($"Handling {context.Message.GetType()}");
            var messageResult = await HandleAsync(context.Message);
            _logger.LogDebug($"Handled {context.Message.GetType()}");
            await context.RespondAsync(messageResult);
        }
    }
}
