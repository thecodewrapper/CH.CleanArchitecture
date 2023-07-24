using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    ///<inheritdoc cref="IApplicationUserService"/>
    internal class ApplicationUserService : IApplicationUserService
    {
        private readonly ILogger<ApplicationUserService> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private const string AUTHENTICATOR_URI_FORMAT = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ApplicationUserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<ApplicationUserService> logger, IMapper mapper, UrlEncoder urlEncoder) {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _urlEncoder = urlEncoder;
        }

        public async Task<Result> CreateUserAsync(User user, string password, List<string> roles, bool isActive) {
            var serviceResult = new Result().Succeed();
            try {
                var applicationUser = _mapper.Map<ApplicationUser>(user);
                applicationUser.Id = Guid.NewGuid().ToString();
                applicationUser.IsActive = isActive;
                applicationUser.MustChangePassword = false;
                var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                using (transaction) {
                    var identityResult = await _userManager.CreateAsync(applicationUser, password);

                    if (!identityResult.Succeeded) {
                        serviceResult.Fail().WithErrors(_mapper.Map<List<ResultError>>(identityResult.Errors));
                    }
                    else if (roles?.Any() ?? false) {
                        var rolesResult = await _userManager.AddToRolesAsync(applicationUser, roles.Select(nr => nr.ToUpper()));
                        if (!rolesResult.Succeeded)
                            serviceResult.Fail().WithErrors(_mapper.Map<List<ResultError>>(identityResult.Errors));
                    }
                    else
                        serviceResult.Succeed();

                    transaction.Complete();
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to create user.");
            }
            return serviceResult;
        }

        public async Task<Result> ActivateUserAsync(string username) {
            var serviceResult = new Result();
            try {
                _logger.LogInformation($"Activating user '{username}'");
                var applicationUser = await _userManager.FindByNameAsync(username);

                var validationResult = ValidateUserForActivation(applicationUser);
                if (validationResult.IsFailed)
                    return validationResult;

                applicationUser.IsActive = true;
                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded) {
                    serviceResult.Succeed();
                    _logger.LogInformation($"User '{username}' activated succesfully");
                }
                else {
                    serviceResult.Fail().WithMessage($"Unable to activate user.");
                    _logger.LogWarning($"Unable to activate user '{username}'");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to activate user.");
            }
            return serviceResult;
        }

        public async Task<Result> DeactivateUserAsync(string username) {
            var serviceResult = new Result();
            try {
                _logger.LogInformation($"Deactivating user '{username}'");
                var applicationUser = await _userManager.FindByNameAsync(username);

                if (applicationUser == null)
                    return serviceResult.Fail().WithMessage("User not found.");

                if (!applicationUser.IsActive)
                    return serviceResult.Fail().WithMessage("User is not active.");

                applicationUser.IsActive = false;
                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded) {
                    serviceResult.Succeed();
                    _logger.LogInformation($"User '{username}' deactivated succesfully");
                }
                else {
                    serviceResult.Fail().WithMessage($"Unable to deactivate user.");
                    _logger.LogWarning($"Unable to deactivate user '{username}'");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to deactivate user.");
            }

            return serviceResult;
        }

        public async Task<Result> AddRolesAsync(RoleAssignmentRequestDTO request) {
            var serviceResult = new Result().Succeed();
            try {
                var applicationUser = await _userManager.FindByNameAsync(request.Username);

                if (applicationUser == null)
                    return serviceResult.Fail().WithMessage("User not found.");

                var validationResult = await ValidateRolesForAdditionAsync(request.Roles, applicationUser);
                if (validationResult.IsFailed)
                    return validationResult;

                var result = await _userManager.AddToRolesAsync(applicationUser, request.Roles.Select(nr => nr.ToUpper()));

                if (result.Succeeded)
                    serviceResult.Succeed();
                else
                    serviceResult.Fail().WithMessage($"Unable to add role user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to add role to user.");
            }

            return serviceResult;
        }

        public async Task<Result> RemoveRolesAsync(RoleAssignmentRequestDTO request) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(request.Username);
                if (applicationUser == null)
                    return serviceResult.Fail().WithMessage("User not found.");

                var validationResult = await ValidateRolesForRemovalAsync(request.Roles, applicationUser);
                if (validationResult.IsFailed)
                    return validationResult;

                var result = await _userManager.RemoveFromRolesAsync(applicationUser, request.Roles.Select(r => r.ToUpper()));

                if (result.Succeeded)
                    serviceResult.Succeed();
                else
                    serviceResult.Fail().WithMessage($"Unable to remove role from user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to remove role from user.");
            }

            return serviceResult;
        }

        public async Task<Result> ChangePasswordAsync(string username, string oldPassword, string newPassword) {
            var serviceResult = new Result();
            try {
                _logger.LogInformation($"Changing password for user '{username}'");

                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Fail().WithMessage("User not found.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(applicationUser, oldPassword, newPassword);
                if (!changePasswordResult.Succeeded) {
                    serviceResult.Fail();
                    foreach (var error in changePasswordResult.Errors) {
                        serviceResult.AddError(error.Description, error.Code);
                    }
                    _logger.LogWarning($"Unable to change password for user '{username}'");
                    return serviceResult.WithMessage("Unable to change user password");
                }

                await _signInManager.RefreshSignInAsync(applicationUser);
                serviceResult.Succeed();
                _logger.LogInformation($"Password changed succesfully for user '{username}'");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to change user password.");
            }

            return serviceResult;
        }

        public async Task<Result> ResetPasswordAsync(string username, string token, string password) {
            var serviceResult = new Result();
            try {
                _logger.LogInformation($"Reseting password for user '{username}'");

                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Fail().WithMessage("User not found.");
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(applicationUser, token, password);
                if (!resetPasswordResult.Succeeded) {
                    serviceResult.Fail();
                    foreach (var error in resetPasswordResult.Errors) {
                        serviceResult.AddError(error.Description, error.Code);
                    }
                    _logger.LogWarning($"Unable to reset password for user '{username}'");
                    return serviceResult.WithMessage("Unable to reset user password");
                }

                serviceResult.Succeed();
                _logger.LogInformation($"Password reset succesfully for user '{username}'");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to reset user password.");
            }

            return serviceResult;
        }

        public async Task<Result<IList<User>>> GetAllUsersAsync(QueryOptions options = null) {
            var serviceResult = new Result<IList<User>>();
            try {
                var query = _userManager.Users.AsNoTracking().Include(u => u.Roles).ThenInclude(ur => ur.Role).AsQueryable();

                if (!string.IsNullOrWhiteSpace(options?.SearchTerm)) {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{options.SearchTerm}%"));
                }

                if (!string.IsNullOrWhiteSpace(options?.OrderBy)) {
                    query = query.OrderBy(options.OrderBy);
                }

                if (options?.Skip != null && options?.PageSize != null) {
                    query = query.Skip(options.Skip);
                    query = query.Take(options.PageSize);
                }

                serviceResult.Data = await _mapper.ProjectTo<User>(query).ToListAsync();
                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve all users.");
            }
            return serviceResult;
        }

        public async Task<Result<User>> GetUserByIdAsync(string id) {
            var serviceResult = new Result<User>();
            try {
                var userWithRoles = await _userManager.Users.AsNoTracking().Include(u => u.Roles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);
                if (userWithRoles == null) {
                    serviceResult.Fail().WithError($"Unable to load user with ID '{id}'.");
                    return serviceResult;
                }
                serviceResult.Data = _mapper.Map<User>(userWithRoles);
                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user by id.");
            }

            return serviceResult;
        }

        public async Task<Result<User>> GetUserByEmailAsync(string email) {
            var serviceResult = new Result<User>();
            try {
                ApplicationUser user = await _userManager.FindByEmailAsync(email);
                if (user != null) {
                    serviceResult.Data = _mapper.Map<User>(user);
                }

                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user by email.");
            }

            return serviceResult;
        }

        public async Task<Result<User>> GetUserByNameAsync(string username) {
            var serviceResult = new Result<User>();
            try {
                ApplicationUser user = await _userManager.FindByNameAsync(username);
                if (user != null) {
                    serviceResult.Data = _mapper.Map<User>(user);
                }

                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user by username.");
            }

            return serviceResult;
        }

        public async Task<Result> UpdateRolesAsync(string username, List<string> roles) {
            var serviceResult = new Result();
            try {
                _logger.LogInformation($"Updating roles for user '{username}'. Roles: {string.Join(",", roles)}");
                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Fail().WithMessage("User not found.");
                }

                var existingRoles = await _userManager.GetRolesAsync(applicationUser);
                var identityResult = await _userManager.RemoveFromRolesAsync(applicationUser, existingRoles);

                if (!identityResult.Succeeded) {
                    _logger.LogWarning($"Unable to update roles for user '{username}'");
                    return serviceResult.Fail().WithMessage($"Unable to update user roles.");
                }

                identityResult = await _userManager.AddToRolesAsync(applicationUser, roles.Select(r => r.ToUpper()));

                if (identityResult.Succeeded) {
                    _logger.LogWarning($"Roles for user '{username}' updated succesfully. Roles: {string.Join(",", roles)}");
                    serviceResult.Succeed();
                }
                else {
                    _logger.LogWarning($"Unable to update roles for user '{username}'");
                    serviceResult.Fail().WithMessage($"Unable to update user roles.");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to update roles of user.");
            }

            return serviceResult;
        }

        public async Task<Result> UpdateUserDetailsAsync(UpdateUserDetailsDTO request) {
            var serviceResult = new Result();
            try {
                _logger.LogInformation($"Updating user details for user with id '{request.Id}'");

                var applicationUser = await _userManager.FindByIdAsync(request.Id);

                if (applicationUser == null)
                    return serviceResult.Fail().WithMessage("User not found.");

                if (!string.IsNullOrWhiteSpace(request.Email) && applicationUser.Email != request.Email)
                    applicationUser.Email = request.Email;

                if (!string.IsNullOrWhiteSpace(request.Name) && applicationUser.Name != request.Name)
                    applicationUser.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.PrimaryPhone) && applicationUser.PhoneNumber != request.PrimaryPhone)
                    applicationUser.PhoneNumber = request.PrimaryPhone;

                if (!string.IsNullOrWhiteSpace(request.SecondaryPhone) && applicationUser.SecondaryPhoneNumber != request.SecondaryPhone)
                    applicationUser.SecondaryPhoneNumber = request.SecondaryPhone;

                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded) {
                    serviceResult.Succeed();
                    _logger.LogInformation($"User details updated for user with id '{request.Id}'");
                }
                else {
                    serviceResult.Fail().WithMessage($"Unable to update user details.");
                    _logger.LogWarning($"Unable to update user details for user with id '{request.Id}'");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to update user details.");
            }

            return serviceResult;
        }

        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(User user) {
            var serviceResult = new Result<string>();
            try {
                ApplicationUser appUser = _mapper.Map<ApplicationUser>(user);
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                serviceResult.Succeed().WithData(token);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to generate email confirmation token.");
            }
            return serviceResult;
        }

        public async Task<Result> ConfirmUserEmailAsync(string userId, string code) {
            Result serviceResult = new Result();
            try {
                _logger.LogInformation($"Confirming user email for user with id '{userId}'. Code: {code}");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) {
                    return serviceResult.Fail().WithError($"Unable to load user with ID '{userId}'.");
                }

                var confirmationResult = await _userManager.ConfirmEmailAsync(user, code);
                if (confirmationResult.Succeeded) {
                    serviceResult.Succeed();
                    _logger.LogInformation($"User email confirmed for user with id '{userId}'. Code: {code}");
                }
                else {
                    serviceResult.Fail().WithError("Unable to confirm user email");
                    _logger.LogWarning($"User email confirmation failed for user with id '{userId}'. Code: {code}. {confirmationResult}");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to confirm user email.");
            }

            return serviceResult;
        }

        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userEmail) {
            Result<string> serviceResult = new Result<string>();
            try {
                _logger.LogInformation($"Generating password reset token for user email '{userEmail}'");

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) {
                    _logger.LogWarning($"Unable to generate password reset token for user '{userEmail}'. User not found or email is not confirmed");
                    return serviceResult.Fail().WithError("User not found or email is not confirmed");
                }

                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                serviceResult.Succeed().WithData(code);

                _logger.LogInformation($"Generated password reset token for user '{userEmail}'. Token: {code}");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to generate password reset token.");
            }
            return serviceResult;
        }

        public async Task<Result<bool>> GetTwoFactorAuthenticationStatusAsync(string userId) {
            Result<bool> serviceResult = new Result<bool>();
            try {
                _logger.LogInformation($"Retrieving 2FA status for user '{userId}'");

                var user = await _userManager.FindByIdAsync(userId);
                bool twoFactorIsEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                serviceResult.Succeed().WithData(twoFactorIsEnabled);

                _logger.LogInformation($"Retrieved 2FA status for user '{userId}'. Status: {twoFactorIsEnabled}");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get two-factor authentication status for user.");
            }
            return serviceResult;
        }

        public async Task<Result> DisableTwoFactorAuthenticationAsync(string userId) {
            Result serviceResult = new Result();
            try {
                _logger.LogInformation($"Disabling 2FA for user '{userId}'");

                var user = await _userManager.FindByIdAsync(userId);
                var twoFactorResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!twoFactorResult.Succeeded) {
                    _logger.LogWarning($"Unable to disable 2FA for user '{userId}'. {twoFactorResult}");
                    return serviceResult.Fail().WithError($"Cannot disable 2FA for user as it's not currently enabled.");
                }

                serviceResult.Succeed();
                _logger.LogInformation($"Disabled 2FA for user '{userId}'");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to disable two-factor authentication for user.");
            }
            return serviceResult;
        }

        public async Task<Result<Dictionary<string, string>>> GetUserPersonalDataAsync(string userId) {
            Result<Dictionary<string, string>> serviceResult = new Result<Dictionary<string, string>>();
            try {
                _logger.LogInformation($"Retrieving personal data for user '{userId}'");

                var user = await _userManager.FindByIdAsync(userId);
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps) {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var l in logins) {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

                serviceResult.Succeed().WithData(personalData);

                _logger.LogInformation($"Returning personal data for user '{userId}'");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get user personal data.");
            }
            return serviceResult;
        }

        public async Task<Result> EnableAuthenticatorAsync(string userId, string verificationCode) {
            Result serviceResult = new Result();
            try {
                _logger.LogInformation($"Enabling authenticator for user '{userId}'. VerificationCode: {verificationCode}");
                var user = await _userManager.FindByIdAsync(userId);

                //Check for valid 2FA token
                bool is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
                if (!is2faTokenValid) {
                    return serviceResult.Fail().WithError("Verification code is invalid.");
                }

                await _userManager.SetTwoFactorEnabledAsync(user, true);
                serviceResult.Succeed();

                _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to enable authenticator for user.");
            }
            return serviceResult;
        }

        public async Task<Result> ResetAuthenticatorAsync(string userId) {
            Result serviceResult = new Result();
            try {
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _userManager.ResetAuthenticatorKeyAsync(user);

                await _signInManager.RefreshSignInAsync(user);

                return serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to reset authenticator for user.");
            }
            return serviceResult;
        }

        public async Task<Result<(string SharedKey, string AuthenticatorUri)>> GetAuthenticatorSharedKeyAndQrCodeUriAsync(string userId) {
            Result<(string SharedKey, string AuthenticatorUri)> serviceResult = new();
            try {
                _logger.LogInformation($"Retrieving authenticator shared key and QR code URI for user '{userId}'");

                var user = await _userManager.FindByIdAsync(userId);
                // Load the authenticator key & QR code URI to display on the form
                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey)) {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }

                string sharedKey = FormatAuthenticatorKey(unformattedKey);
                string authenticatorUri = GenerateAuthenticatorQrCodeUri(user.Email, unformattedKey);

                serviceResult.Succeed().WithData((sharedKey, authenticatorUri));

                _logger.LogInformation($"Returning authenticator shared key and QR code URI for user '{userId}'");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get authenticator shared key and qr code uri for user.");
            }
            return serviceResult;
        }

        public async Task<Result<IEnumerable<string>>> GenerateTwoFactorRecoveryCodesAsync(string userId, int numberOfCodesToGenerate) {
            Result<IEnumerable<string>> serviceResult = new();
            try {
                _logger.LogInformation($"Generating {numberOfCodesToGenerate} two-factor recovery codes for user '{userId}'");

                var user = await _userManager.FindByIdAsync(userId);
                int userExistingRecoveryCodesCount = await _userManager.CountRecoveryCodesAsync(user);
                if (userExistingRecoveryCodesCount > 0) {
                    _logger.LogWarning($"Unable to generate new recovery codes for user '{userId}'. User already has {userExistingRecoveryCodesCount} recovery codes generated");
                    return serviceResult.Fail().WithMessage("User already has recovery codes generated");
                }

                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodesToGenerate);

                _logger.LogInformation($"Generated {recoveryCodes.Count()} two-factor recovery codes for user '{userId}'");
                return serviceResult.Succeed().WithData(recoveryCodes);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to generate 2FA recovery codes for user.");
            }
            return serviceResult;
        }

        private Result<string> ValidateUserForActivation(ApplicationUser applicationUser) {
            var serviceResult = new Result<string>();
            if (applicationUser == null)
                return serviceResult.Fail().WithMessage("User not found.");

            if (applicationUser.IsActive)
                return serviceResult.Fail().WithMessage("User is already active.");
            return serviceResult;
        }

        private async Task<Result> ValidateRolesForAdditionAsync(List<string> roles, ApplicationUser applicationUser) {
            Result validationResult = new Result();
            var rolesAlreadyAssigned = await GetAlreadyAssignedRolesFromUserAsync(roles, applicationUser);
            var invalidRoles = GetInvalidRoles(roles);

            if (rolesAlreadyAssigned.Any())
                return validationResult.Fail().WithErrors(rolesAlreadyAssigned.Select(r => new ResultError($"Role {r} is already assigned.")).ToList());

            if (invalidRoles.Any())
                return validationResult.Fail().WithErrors(invalidRoles.Select(r => new ResultError($"Role {r} is invalid.")).ToList());

            return validationResult.Succeed();
        }

        private async Task<Result> ValidateRolesForRemovalAsync(List<string> roles, ApplicationUser applicationUser) {
            Result validationResult = new Result();
            var invalidRoles = GetInvalidRoles(roles);
            var rolesNotAssigned = await GetUnassignedRolesFromUserAsync(roles, applicationUser);

            if (rolesNotAssigned.Any())
                return validationResult.Fail().WithErrors(rolesNotAssigned.Select(r => new ResultError($"Role {r} is not assigned.")).ToList());

            if (invalidRoles.Any())
                return validationResult.Fail().WithErrors(invalidRoles.Select(r => new ResultError($"Role {r} is invalid.")).ToList());

            return validationResult.Succeed();
        }

        private List<string> GetInvalidRoles(List<string> rolesToCheck) {
            List<string> invalidRoles = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                var isRoleInvalid = !Enum.IsDefined(typeof(RoleEnum), roleToCheck);
                if (isRoleInvalid)
                    invalidRoles.Add(roleToCheck);
            }
            return invalidRoles;
        }

        private async Task<List<string>> GetUnassignedRolesFromUserAsync(List<string> rolesToCheck, ApplicationUser applicationUser) {
            List<string> rolesNotAssigned = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                bool isRoleAssigned = await _userManager.IsInRoleAsync(applicationUser, roleToCheck.ToUpper());
                if (isRoleAssigned == false)
                    rolesNotAssigned.Add(roleToCheck);
            }
            return rolesNotAssigned;
        }

        private async Task<List<string>> GetAlreadyAssignedRolesFromUserAsync(List<string> rolesToCheck, ApplicationUser applicationUser) {
            List<string> rolesAssigned = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                if (await _userManager.IsInRoleAsync(applicationUser, roleToCheck.ToUpper())) {
                    rolesAssigned.Add(roleToCheck);
                }
            }
            return rolesAssigned;
        }

        private string FormatAuthenticatorKey(string unformattedKey) {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length) {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length) {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateAuthenticatorQrCodeUri(string email, string unformattedKey) {
            return string.Format(
            CultureInfo.InvariantCulture,
            AUTHENTICATOR_URI_FORMAT,
                _urlEncoder.Encode("CH.CleanArchitecture"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
