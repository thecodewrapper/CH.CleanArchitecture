using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Infrastructure.Data.Models;
using CH.CleanArchitecture.Infrastructure.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Shared
{
    ///<inheritdoc cref="IApplicationUserService"/>
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ILogger<ApplicationUserService> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILocalizationService _localizer;

        public ApplicationUserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<ApplicationUserService> logger, IMapper mapper, ILocalizationService localizer) {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> ActivateUser(string username) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);

                var validationResult = ValidateUserForActivation(applicationUser);
                if (validationResult.Failed)
                    return validationResult;

                applicationUser.IsActive = true;
                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to activate user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to activate user.");
            }
            return serviceResult;
        }

        public async Task<Result> DeactivateUser(string username) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);

                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                if (!applicationUser.IsActive)
                    return serviceResult.Failed().WithMessage("User is not active.");

                applicationUser.IsActive = false;
                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to deactivate user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to deactivate user.");
            }

            return serviceResult;
        }

        public async Task<Result> AddRoles(RoleAssignmentRequestDTO request) {
            var serviceResult = new Result().Successful();
            try {
                var applicationUser = await _userManager.FindByNameAsync(request.Username);

                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                var validationResult = await ValidateRolesForAdditionAsync(request.Roles, applicationUser);
                if (validationResult.Failed)
                    return validationResult;

                var result = await _userManager.AddToRolesAsync(applicationUser, request.Roles.Select(nr => nr.ToUpper()));

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to add role user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to add role to user.");
            }

            return serviceResult;
        }

        public async Task<Result> RemoveRoles(RoleAssignmentRequestDTO request) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(request.Username);
                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                var validationResult = await ValidateRolesForRemovalAsync(request.Roles, applicationUser);
                if (validationResult.Failed)
                    return validationResult;

                var result = await _userManager.RemoveFromRolesAsync(applicationUser, request.Roles.Select(r => r.ToUpper()));

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to remove role from user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to remove role from user.");
            }

            return serviceResult;
        }

        public async Task<Result> CreateUser(User user, string password, List<string> roles, bool isActive) {
            var serviceResult = new Result().Successful();
            try {
                var applicationUser = _mapper.Map<ApplicationUser>(user);
                applicationUser.Id = Guid.NewGuid().ToString();
                applicationUser.IsActive = isActive;
                applicationUser.MustChangePassword = true;
                var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                using (transaction) {
                    var identityResult = await _userManager.CreateAsync(applicationUser, password);

                    if (!identityResult.Succeeded) {
                        serviceResult.Failed().WithErrors(_mapper.Map<List<ResultError>>(identityResult.Errors));
                    }
                    else if (roles?.Any() ?? false) {
                        var rolesResult =
                            await _userManager.AddToRolesAsync(applicationUser, roles.Select(nr => nr.ToUpper()));
                        if (!rolesResult.Succeeded)
                            serviceResult.Failed().WithErrors(_mapper.Map<List<ResultError>>(identityResult.Errors));
                    }
                    else
                        serviceResult.Successful();

                    transaction.Complete();
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to create user.");
            }
            return serviceResult;
        }

        public async Task<Result> ChangePassword(string username, string password) {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Failed().WithMessage("User not found.");
                }

                var hashResult = passwordHasher.VerifyHashedPassword(applicationUser, applicationUser.PasswordHash, password);
                if (hashResult == PasswordVerificationResult.Success) {
                    return serviceResult.Failed().WithMessage(_localizer[ResourceKeys.Validations_UseDifferentPassword]);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);
                var result = await _userManager.ResetPasswordAsync(applicationUser, token, password);

                if (result.Succeeded) {
                    if (applicationUser.MustChangePassword) {
                        applicationUser.MustChangePassword = false;
                        await _userManager.UpdateAsync(applicationUser);

                        await _signInManager.SignOutAsync();
                        await _signInManager.PasswordSignInAsync(applicationUser, password, false, false);
                    }
                    serviceResult.Successful();
                }
                else {
                    serviceResult.Failed().WithMessage($"Unable to change user password.");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to change user password.");
            }

            return serviceResult;
        }

        public async Task<Result<IList<User>>> GetAllUsers(QueryOptions options = null) {
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
                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to create user.");
            }
            return serviceResult;
        }

        public async Task<Result<User>> GetUser(string id) {
            var serviceResult = new Result<User>();
            try {
                serviceResult.Data = await _mapper.ProjectTo<User>(GetUsersWithRoles()).FirstOrDefaultAsync(u => u.Id == id);
                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user.");
            }

            return serviceResult;
        }

        public async Task<Result> UpdateRoles(string username, List<string> roles) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Failed().WithMessage("User not found.");
                }

                var existingRoles = await _userManager.GetRolesAsync(applicationUser);
                var identityResult = await _userManager.RemoveFromRolesAsync(applicationUser, existingRoles);

                if (!identityResult.Succeeded) return serviceResult.Failed().WithMessage($"Unable to update user roles.");

                identityResult = await _userManager.AddToRolesAsync(applicationUser, roles.Select(r => r.ToUpper()));

                if (identityResult.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to update user roles.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to update roles of user.");
            }

            return serviceResult;
        }

        public async Task<Result> UpdateUserDetails(UpdateUserDetailsDTO request) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByIdAsync(request.Id);

                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                if (!string.IsNullOrWhiteSpace(request.Email) && applicationUser.Email != request.Email)
                    applicationUser.Email = request.Email;

                if (!string.IsNullOrWhiteSpace(request.Name) && applicationUser.Name != request.Name)
                    applicationUser.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.PrimaryPhoneNumber) && applicationUser.PhoneNumber != request.PrimaryPhoneNumber)
                    applicationUser.PhoneNumber = request.PrimaryPhoneNumber;

                if (!string.IsNullOrWhiteSpace(request.SecondaryPhoneNumber) && applicationUser.SecondaryPhoneNumber != request.SecondaryPhoneNumber)
                    applicationUser.SecondaryPhoneNumber = request.SecondaryPhoneNumber;

                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to update user details.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to update user details.");
            }

            return serviceResult;
        }

        private IQueryable<ApplicationUser> GetUsersWithRoles() {
            return _userManager.Users.AsNoTracking().Include(u => u.Roles).ThenInclude(ur => ur.Role);
        }

        private Result<string> ValidateUserForActivation(ApplicationUser applicationUser) {
            var serviceResult = new Result<string>();
            if (applicationUser == null)
                return serviceResult.Failed().WithMessage("User not found.");

            if (applicationUser.IsActive)
                return serviceResult.Failed().WithMessage("User is already active.");
            return serviceResult;
        }

        private async Task<Result> ValidateRolesForAdditionAsync(List<string> roles, ApplicationUser applicationUser) {
            Result validationResult = new Result();
            var rolesAlreadyAssigned = await GetAlreadyAssignedRolesFromUserAsync(roles, applicationUser);
            var invalidRoles = GetInvalidRoles(roles);

            if (rolesAlreadyAssigned.Any())
                return validationResult.Failed().WithErrors(rolesAlreadyAssigned.Select(r => new ResultError($"Role {r} is already assigned.")).ToList());

            if (invalidRoles.Any())
                return validationResult.Failed().WithErrors(invalidRoles.Select(r => new ResultError($"Role {r} is invalid.")).ToList());

            return validationResult.Successful();
        }

        private async Task<Result> ValidateRolesForRemovalAsync(List<string> roles, ApplicationUser applicationUser) {
            Result validationResult = new Result();
            var invalidRoles = GetInvalidRoles(roles);
            var rolesNotAssigned = await GetUnassignedRolesFromUserAsync(roles, applicationUser);

            if (rolesNotAssigned.Any())
                return validationResult.Failed().WithErrors(rolesNotAssigned.Select(r => new ResultError($"Role {r} is not assigned.")).ToList());

            if (invalidRoles.Any())
                return validationResult.Failed().WithErrors(invalidRoles.Select(r => new ResultError($"Role {r} is invalid.")).ToList());

            return validationResult.Successful();
        }

        private List<string> GetInvalidRoles(List<string> rolesToCheck) {
            List<string> invalidRoles = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                var isRoleInvalid = !Enum.IsDefined(typeof(RolesEnum), roleToCheck);
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
    }
}
