using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.Messaging.Abstractions;

namespace CH.CleanArchitecture.Core.Application.Queries
{
    public class GetUserQuery : IRequest<Result<UserReadModel>>
    {
        public string Id { get; set; }
    }
}