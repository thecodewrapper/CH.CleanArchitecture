using System.Collections.Generic;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Queries
{
    public class GetAllUsersQuery : IRequest<Result<IEnumerable<UserReadModel>>>
    {
        public QueryOptions Options { get; set; }
        public bool ApplyRoleFilter { get; set; } = true;
    }
}