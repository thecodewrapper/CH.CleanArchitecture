using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Authorization;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.CleanArchitecture.Core.Application.Queries;
using CH.CleanArchitecture.Core.Application.ReadModels;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Presentation.Framework;
using CH.CleanArchitecture.Presentation.Web.ViewModels;
using CH.Messaging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CH.CleanArchitecture.Presentation.Web.Controllers
{
    [Route("Users")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IServiceBus _serviceBus;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly INotificationService _notificationService;
        private readonly IAuthenticatedUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UsersController(IServiceBus serviceBus,
            IHttpContextAccessor contextAccessor,
            IMapper mapper,
            ILocalizationService localizer,
            INotificationService notificationService,
            IAuthenticatedUserService userService,
            IAuthorizationService authorizationService) {
            _serviceBus = serviceBus;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _localizer = localizer;
            _notificationService = notificationService;
            _userService = userService;
            _authorizationService = authorizationService;
        }


        [HttpPost]
        public async Task<IActionResult> LoadData([FromForm] DataTablesParameters parameters) {
            // Getting all users 
            var queryOptions = parameters.ToQueryOptions();
            var usersQuery = await _serviceBus.SendAsync(new GetAllUsersQuery { Options = queryOptions });

            if (usersQuery.Failed) return null;

            var recordCount = usersQuery.GetMetadata<int>("RecordCount");
            //Returning Json Data  
            return Json(new { draw = parameters.Draw, recordsFiltered = recordCount, recordsTotal = recordCount, data = usersQuery.Data });
        }

        /// <summary>
        /// Returns a view of all users
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index() {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, UserOperations.Read());
            if (!authorizationResult.Succeeded) {
                _notificationService.ErrorNotification("You are not authorized to view users");
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [Route("{id}")]
        public async Task<IActionResult> Details(Guid? id) {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, UserOperations.Read());
            if (!authorizationResult.Succeeded) {
                _notificationService.ErrorNotification("You are not authorized to view users");
                return RedirectToAction(nameof(Index));
            }

            var user = await _serviceBus.SendAsync(new GetUserQuery { Id = id.ToString() ?? _userService.UserId });

            if (user.Succeeded)
                return View(_mapper.Map<UserDetailsModel>(user.Data));
            else {
                _notificationService.ErrorNotification("Unable to retrieve user.  Please try again");
                return BadRequest();
            }
        }

        [Route("Create")]
        public IActionResult Create() {
            return View(new CreateUserViewModel());
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="createUserModel">The details of the user to be created</param>
        /// <returns></returns>
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel createUserModel) {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, UserOperations.Create);
            if (!authorizationResult.Succeeded) {
                _notificationService.ErrorNotification("You are not authorized to create a user");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(createUserModel);

            Result result = await _serviceBus.SendAsync(_mapper.Map<CreateUserCommand>(createUserModel));

            if (result.Succeeded) {
                var returnUrl = _contextAccessor.HttpContext.Request.Query["ReturnUrl"];
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                _notificationService.SuccessNotification(string.Format(_localizer[ResourceKeys.Notifications_UserCreated_Success], createUserModel.Username));
                return RedirectToAction(nameof(Index));
            }
            else {
                _notificationService.ErrorNotification(result.MessageWithErrors);
                ModelState.AddModelError("", "User creation failed.");
            }

            return View();
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id) {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, UserOperations.Edit());
            if (!authorizationResult.Succeeded) {
                _notificationService.ErrorNotification("You are not authorized to edit a user");
                return RedirectToAction(nameof(Index));
            }

            var user = await _serviceBus.SendAsync(new GetUserQuery { Id = id });

            if (user.Failed || user.Data == null)
                return NotFound();

            var userEditModel = _mapper.Map<EditUserViewModel>(user.Data);
            return View(userEditModel);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model) {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, UserOperations.Edit());
            if (!authorizationResult.Succeeded) {
                _notificationService.ErrorNotification("You are not authorized to edit a user");
                return RedirectToAction(nameof(Index));
            }

            var user = await _serviceBus.SendAsync(new GetUserQuery { Id = id });

            if (user.Failed)
                return BadRequest();

            if (await HandleUserUpdate(model, user.Data) == false)
                return View(model);

            _notificationService.SuccessNotification(string.Format(_localizer[ResourceKeys.Notifications_UserUpdated_Success], model.Username));
            return RedirectToAction(nameof(Details), new { id = id });
        }

        private async Task<bool> HandleUserUpdate(EditUserViewModel model, UserReadModel user) {
            try {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
                    var updateUserDetailsCommand = _mapper.Map<UpdateUserDetailsCommand>(model);
                    var updateResult = await _serviceBus.SendAsync(updateUserDetailsCommand);
                    if (updateResult.Failed)
                        throw new Exception(updateResult.MessageWithErrors);

                    if (!string.IsNullOrWhiteSpace(model.Password)) {
                        var changePasswordResult = await _serviceBus.SendAsync(new ChangeUserPasswordCommand(user.Username, model.Password));
                        if (changePasswordResult.Failed)
                            throw new Exception(changePasswordResult.MessageWithErrors);
                    }

                    if (user.IsActive != model.IsActive) {
                        var updateStatusResult = model.IsActive
                                ? await _serviceBus.SendAsync(new ActivateUserCommand(model.Username))
                                : await _serviceBus.SendAsync(new DeactivateUserCommand(model.Username));
                        if (updateStatusResult.Failed)
                            throw new Exception(updateResult.MessageWithErrors);
                    }

                    if (!user.Roles.All(model.Roles.Contains) || user.Roles.Count() != model.Roles.Count()) {
                        var updateRolesResult = await _serviceBus.SendAsync(_mapper.Map<UpdateUserRolesCommand>(model));
                        if (updateRolesResult.Failed)
                            throw new Exception(updateRolesResult.MessageWithErrors);
                    }

                    transaction.Complete();
                }
            }
            catch (Exception ex) {
                _notificationService.ErrorNotification(ex);
                return false;
            }

            return true;
        }
    }
}
