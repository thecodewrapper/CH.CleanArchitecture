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
        Task<Result> CreateUserAsync(User user, string password, List<string> roles, bool isActive);

        /// <summary>
        /// Activates the application user with username <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<Result> ActivateUserAsync(string username);

        /// <summary>
        /// Deactivates the application user with username <paramref name="username"/>
        /// </summary>
        /// <returns></returns>
        Task<Result> DeactivateUserAsync(string username);

        /// <summary>
        /// Adds a role to a user
        /// </summary>
        /// <param name="request">the role assignment request</param>
        /// <returns></returns>
        Task<Result> AddRolesAsync(RoleAssignmentRequestDTO request);

        /// <summary>
        /// Removes a role from a user
        /// </summary>
        /// <param name="request">the role assignment request</param>
        /// <returns></returns>
        Task<Result> RemoveRolesAsync(RoleAssignmentRequestDTO request);

        /// <summary>
        /// Updates a user's roles
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="roles">the list of new roles</param>
        /// <returns></returns>
        Task<Result> UpdateRolesAsync(string username, List<string> roles);

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <returns>list of <see cref="User"/></returns>
        Task<Result<IList<User>>> GetAllUsersAsync(QueryOptions options);

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">the user ID</param>
        /// <returns> a <see cref="User"/></returns>
        Task<Result<User>> GetUserByIdAsync(string id);

        /// <summary>
        /// Get user by <paramref name="email"/>
        /// </summary>
        /// <param name="email">the user email</param>
        /// <returns> a <see cref="User"/></returns>
        Task<Result<User>> GetUserByEmailAsync(string email);

        /// <summary>
        /// Get user by <paramref name="username"/>
        /// </summary>
        /// <param name="id">the user email</param>
        /// <returns> a <see cref="User"/></returns>
        Task<Result<User>> GetUserByNameAsync(string username);

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword);

        /// <summary>
        /// Resets a user's password
        /// </summary>
        /// <param name="userId">The username</param>
        /// <param name="token">The password reset token</param>
        /// <param name="password">The new password</param>
        /// <returns></returns>
        Task<Result> ResetPasswordAsync(string userId, string token, string password);

        /// <summary>
        /// Updates the user details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Result> UpdateUserDetailsAsync(UpdateUserDetailsDTO request);

        /// <summary>
        /// Generates an email confirmation token for the specified <paramref name="user"/>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Result<string>> GenerateEmailConfirmationTokenAsync(User user);

        /// <summary>
        /// Generates a password reset token for the specified <paramref name="userEmail"/>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Result<string>> GeneratePasswordResetTokenAsync(string userEmail);

        /// <summary>
        /// Validates that the email confirmation token mathces the user specified by user Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Result> ConfirmUserEmailAsync(string userId, string code);

        /// <summary>
        /// Return true/false on whether 2FA is enabled for the user specified by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<bool>> GetTwoFactorAuthenticationStatusAsync(string userId);

        /// <summary>
        /// Enables/disables 2FA for the user specified by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result> DisableTwoFactorAuthenticationAsync(string userId);

        /// <summary>
        /// Retrieves a <see cref="Dictionary{TKey, TValue}"/> of the user's personal data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<Dictionary<string, string>>> GetUserPersonalDataAsync(string userId);

        /// <summary>
        /// Enables authenticator for the given <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result> EnableAuthenticatorAsync(string userId, string verificationCode);

        /// <summary>
        /// Resets the authenticator for the given <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result> ResetAuthenticatorAsync(string userId);

        /// <summary>
        /// Returns the authenticator key for the given <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<string>> GetAuthenticatorKeyAsync(string userId);

        /// <summary>
        /// Returns the authenticator shared key and QR code URI for the given <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Result<(string SharedKey, string AuthenticatorUri)>> GetAuthenticatorSharedKeyAndQrCodeUriAsync(string userId);

        /// <summary>
        /// Generates <paramref name="numberOfCodesToGenerate"/> 2FA recovery codes for the specified <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="numberOfCodesToGenerate"></param>
        /// <returns></returns>
        Task<Result<IEnumerable<string>>> GenerateTwoFactorRecoveryCodesAsync(string username, int numberOfCodesToGenerate);
    }
}
