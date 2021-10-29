using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CH.CleanArchitecture.Presentation.Framework.Interfaces
{
    public interface IAuthorizationStateProvider
    {
        bool this[IAuthorizationRequirement requirement] { get; }

        Task<bool> Any(IAuthorizationRequirement[] requirements);

        Task Refresh();

        Task<bool> TryAddAndCheckRequirement(IAuthorizationRequirement requirement);
    }
}
