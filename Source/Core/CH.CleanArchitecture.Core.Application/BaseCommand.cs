using CH.CleanArchitecture.Common;
using CH.Messaging.Abstractions;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Collections.Generic;
using System.Security.Claims;

namespace CH.CleanArchitecture.Core.Application
{
    public abstract class BaseCommand<TResponse> : IRequest<TResponse>, ICommand
        where TResponse : class, IResult
    {
        public List<OperationAuthorizationRequirement> Requirements { get; private set; } = new();
        public ClaimsPrincipal User { get; set; }

        protected void AddRequirements(params OperationAuthorizationRequirement[] requirements) {
            Requirements.AddRange(requirements);
        }
    }
}
