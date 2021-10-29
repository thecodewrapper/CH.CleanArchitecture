using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IApplicationUserService
    {
        /// <summary>
        /// Creates a new user according to the provided <paramref name="user"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        Task<Result> CreateUser(User user, string password, List<string> roles, bool isActive);

        /// <summary>
        /// Activates the application user with username <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<Result> ActivateUser(string username);

        /// <summary>
        /// Deactivates the application user with username <paramref name="username"/>
        /// </summary>
        /// <returns></returns>
        Task<Result> DeactivateUser(string username);

        /// <summary>
        /// Adds a role to a user
        /// </summary>
        /// <param name="request">the role assignment request</param>
        /// <returns></returns>
        Task<Result> AddRoles(RoleAssignmentRequestDTO request);

        /// <summary>
        /// Removes a role from a user
        /// </summary>
        /// <param name="request">the role assignment request</param>
        /// <returns></returns>
        Task<Result> RemoveRoles(RoleAssignmentRequestDTO request);

        /// <summary>
        /// Updates a user's roles
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="roles">the list of new roles</param>
        /// <returns></returns>
        Task<Result> UpdateRoles(string username, List<string> roles);

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <returns>list of <see cref="User"/></returns>
        Task<Result<IList<User>>> GetAllUsers(QueryOptions options);

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="id">the user ID</param>
        /// <returns> a <see cref="User"/></returns>
        Task<Result<User>> GetUser(string id);

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Result> ChangePassword(string username, string password);

        Task<Result> UpdateUserDetails(UpdateUserDetailsDTO request);
    }
}
