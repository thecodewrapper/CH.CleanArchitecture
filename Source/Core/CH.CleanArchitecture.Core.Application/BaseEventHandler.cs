using CH.Messaging.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CH.CleanArchitecture.Core.Application
{
    public abstract class BaseEventHandler<TRequest> : IHandler<TRequest>, IConsumer<TRequest>
        where TRequest : BaseEvent
    {
        private readonly ILogger _logger;

        public BaseEventHandler(IServiceProvider serviceProvider) {
            _logger = serviceProvider.GetRequiredService<ILogger<TRequest>>();
        }

        public virtual async Task Consume(ConsumeContext<TRequest> context) {
            HandleAsync(context.Message);
        }

        ///// <summary>
        ///// Method to be overriden by all event handlers
        ///// </summary>
        ///// <returns></returns>
        public abstract Task HandleAsync(TRequest request);
    }
}
