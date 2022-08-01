using CH.CleanArchitecture.Core.Application.Authorization;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.CleanArchitecture.Core.Application.Mappings;
using CH.CleanArchitecture.Core.Application.Queries;
using CH.CleanArchitecture.Core.Application.Queries.Orders;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Core.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services) {
            services.AddScoped<PhoneNumberToStringConverter>();
            services.AddScoped<StringToPhoneNumberConverter>();

            services.AddAutoMapper(config =>
            {
                config.ConstructServicesUsing(t => services.BuildServiceProvider().GetRequiredService(t));
                config.AddProfile<UserProfile>();
                config.AddProfile<OrderProfile>();
            });
            services.AddMediator(x =>
            {
                #region Commands

                #region User

                x.AddConsumer<CreateUserCommandHandler>();
                x.AddConsumer<ActivateUserCommandHandler>();
                x.AddConsumer<DeactivateUserCommandHandler>();
                x.AddConsumer<AddRolesCommandHandler>();
                x.AddConsumer<RemoveRolesCommandHandler>();
                x.AddConsumer<ChangeUserPasswordCommandHandler>();
                x.AddConsumer<UpdateUserRolesCommandHandler>();
                x.AddConsumer<UpdateUserDetailsCommandHandler>();
                x.AddConsumer<CreateNewOrderCommandHandler>();

                #endregion User

                #endregion Commands

                #region Queries

                x.AddConsumer<GetAllUsersQueryHandler>();
                x.AddConsumer<GetUserQueryHandler>();
                x.AddConsumer<GetAllOrdersQueryHandler>();
                x.AddConsumer<GetOrderByIdQueryHandler>();

                #endregion
            });

            services.AddAuthorizationCore();
            services.AddAuthorizationPolicies();
        }

        private static void AddAuthorizationPolicies(this IServiceCollection services) {
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, UserOperationAuthorizationHandler>();
        }
    }
}