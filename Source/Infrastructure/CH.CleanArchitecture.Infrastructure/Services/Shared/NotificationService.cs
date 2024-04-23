using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Infrastructure.Constants;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.Data.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class NotificationService : INotificationService
    {
        #region Private Fields

        private readonly IApplicationConfigurationService _appConfigService;
        private readonly IEntityRepository<NotificationEntity, Guid> _notificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly ILogger<NotificationService> _logger;
        private readonly IMapper _mapper;

        #endregion Private Fields

        #region Public Constructors

        public NotificationService(ILogger<NotificationService> logger,
            IEmailService emailService,
            ISMSService smsService,
            IMapper mapper,
            IApplicationConfigurationService appConfigService,
            IEntityRepository<NotificationEntity, Guid> notificationRepository,
            UserManager<ApplicationUser> userManager) {
            _logger = logger;
            _emailService = emailService;
            _smsService = smsService;
            _mapper = mapper;
            _appConfigService = appConfigService;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Get all notifications
        /// </summary>
        /// <returns>IQueryable with Notification DTO's. See <see cref="NotificationDTO"/> for more information.</returns>
        public Result<IQueryable<NotificationDTO>> GetAll() {
            var result = new Result<IQueryable<NotificationDTO>>();
            try {
                IQueryable<NotificationEntity> allNotifications = _notificationRepository.GetAll();
                result.WithData(_mapper.ProjectTo<NotificationDTO>(allNotifications));
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to retrieve notifications from database");
            }
            return result;
        }

        /// <summary>
        /// Get all user notifications for the specified user
        /// </summary>
        /// <param name="userFor">User for which notifications will be retrieved.</param>
        /// <returns>IQueryable with Notification DTO's. See <see cref="NotificationDTO"/> for more information.</returns>
        public Result<IQueryable<NotificationDTO>> GetAllForUser(string userFor) {
            var result = new Result<IQueryable<NotificationDTO>>();
            try {
                var allNotificationsForUser = _notificationRepository.GetAll().Where(n => n.UserFor == userFor);
                result.WithData(_mapper.ProjectTo<NotificationDTO>(allNotificationsForUser));
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to retrieve notifications for user");
            }
            return result;
        }

        /// <summary>
        /// Purge the notifications DB table based on the configuration variable NotificationsPurgeHistoryTableInterval.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>True if succeeds</returns>
        public async Task<Result> PurgeNotificationsAsync(CancellationToken cancellationToken = default) {
            var result = new Result();
            try {
                int notificationsPurgeHistoryTableInterval = _appConfigService.GetValueInt(AppConfigKeys.NOTIFICATIONS.PURGE_HISTORYTABLE_INTERVAL).Unwrap();
                if (notificationsPurgeHistoryTableInterval > 0) {
                    var limitDate = DateTime.Today.AddDays(-notificationsPurgeHistoryTableInterval);

                    var entitiesToDelete = _notificationRepository.GetAll().Where(a => a.DateCreated < limitDate).ToList();
                    _logger.LogDebug($"PurgeHistory: Found {entitiesToDelete.Count} to delete.");
                    _notificationRepository.DeleteRange(entitiesToDelete);
                    if (!cancellationToken.IsCancellationRequested)
                        await _notificationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                    result.Succeed();
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to purge notifications");
            }
            return result;
        }

        /// <summary>
        /// Sends a notification to the recipients asynchronously, specified by the type of notification
        /// </summary>
        /// <param name="sendNotificationDTO">The request parameters object</param>
        /// <returns></returns>
        public async Task<Result<int>> SendNotificationAsync(SendNotificationDTO sendNotificationDTO) {
            _logger.LogTrace($"Attempting to send new {sendNotificationDTO.Type} notification. Title: {sendNotificationDTO.Title}, Message: {sendNotificationDTO.Message}, Recipients count: {sendNotificationDTO.Recipients.Count}");
            var result = new Result<int>();
            try {
                List<NotificationDTO> notificationsToSend = new();

                //Retrieve user DTOs
                List<ApplicationUser> recipients = _userManager.Users.Where(u => sendNotificationDTO.Recipients.Contains(u.Id)).ToList();

                switch (sendNotificationDTO.Type) {
                    case NotificationType.Portal: {
                            foreach (var user in recipients) {
                                NotificationDTO notificationDTO = ConstructNewNotificationDTO(sendNotificationDTO.Title, sendNotificationDTO.Message, sendNotificationDTO.Type, user.Id);
                                notificationDTO.IsSent = true;
                                notificationsToSend.Add(notificationDTO);
                            }
                        }
                        break;
                    case NotificationType.SMS: {
                            foreach (var user in recipients) {
                                NotificationDTO notificationDTO = ConstructNewNotificationDTO(sendNotificationDTO.Title, sendNotificationDTO.Message, sendNotificationDTO.Type, user.Id);
                                notificationDTO.IsNew = false; //setting this to false because it has no impact on SMS notifications
                                if (string.IsNullOrEmpty(user.PhoneNumber)) {
                                    _logger.LogWarning($"User {user.Id} does not have a phone number. Skipping sending notification...");
                                    continue; //continue to next iteration
                                }
                                var sentResult = await _smsService.SendSMSAsync(user.PhoneNumber, sendNotificationDTO.Message);

                                //if result is invalid perhaps there was an exception, maybe user doesnt have a phone number, so we handle it here
                                notificationDTO.IsSent = sentResult.IsSuccessful;
                                notificationsToSend.Add(notificationDTO);
                            }
                        }
                        break;
                    case NotificationType.Email: {
                            List<string> emailList = recipients.Select(u => u.Email).ToList();
                            var sentResult = await _emailService.SendEmailAsync(emailList, sendNotificationDTO.Title, sendNotificationDTO.Message);

                            foreach (var user in recipients) {
                                NotificationDTO notificationDTO = ConstructNewNotificationDTO(sendNotificationDTO.Title, sendNotificationDTO.Message, sendNotificationDTO.Type, user.Id);
                                notificationDTO.IsNew = false; //setting this to false because it has no impact on email notifications
                                notificationDTO.Description = sendNotificationDTO.Message.Chop(500); //chop the string, in case of email HTML content
                                notificationDTO.IsSent = sentResult.IsSuccessful;
                                notificationsToSend.Add(notificationDTO);
                            }
                        }
                        break;
                }

                //Add the notifications to DB
                if (notificationsToSend.Any()) {
                    await AddMultipleAsync(notificationsToSend);
                }

                result.Succeed().WithData(notificationsToSend.Count);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to send notification(s)");
            }
            return result;
        }

        public async Task<Result> MarkAllAsReadForUserAsync(string user) {
            var result = new Result();
            try {
                var allNotificationsForUser = _notificationRepository.GetAll().Where(n => n.UserFor == user && n.IsNew).ToList();
                allNotificationsForUser.ForEach(n => n.IsNew = false);

                _notificationRepository.UpdateRange(allNotificationsForUser);
                await _notificationRepository.UnitOfWork.SaveChangesAsync();
                result.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to mark notifications for user as read");
            }
            return result;
        }

        public async Task<Result> MarkAsReadAsync(Guid id) {
            var result = new Result();
            try {
                var notification = await _notificationRepository.FindAsync(id);
                notification.IsNew = false;

                _notificationRepository.Update(notification);
                await _notificationRepository.UnitOfWork.SaveChangesAsync();
                result.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, $"Error while trying to mark notification with id {id} as read");
            }
            return result;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Add a single notification to the datastore asynchronously
        /// </summary>
        /// <param name="notificationDTO">Notification DTO. See <see cref="NotificationDTO"/> for more information.</param>
        private async Task AddAsync(NotificationDTO notificationDTO) {
            _logger.LogDebug($"Saving notification into DB");
            try {
                var entity = _mapper.Map<NotificationEntity>(notificationDTO);
                await _notificationRepository.AddAsync(entity);
                int changes = await _notificationRepository.UnitOfWork.SaveChangesAsync();

                //log if saving goes wrong
                if (changes > 0)
                    _logger.LogDebug($"Saved notification to DB");
                else
                    _logger.LogError($"Failed to save notification to DB");
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// Add multiple notifications to the datastore asynchronously
        /// </summary>
        /// <param name="notificationDTOs">Notification DTOs. See <see cref="NotificationDTO"/> for more information.</param>
        private async Task AddMultipleAsync(List<NotificationDTO> notificationDTOs) {
            _logger.LogDebug($"Saving notifications into DB");
            try {
                List<NotificationEntity> entities = new List<NotificationEntity>();
                foreach (var notDTO in notificationDTOs) {
                    entities.Add(_mapper.Map<NotificationEntity>(notDTO));
                }
                await _notificationRepository.AddRangeAsync(entities);
                int changes = await _notificationRepository.UnitOfWork.SaveChangesAsync();

                //log if saving goes wrong
                if (changes > 0)
                    _logger.LogDebug($"Saved notification to DB");
                else
                    _logger.LogError($"Failed to save notification to DB");
            }
            catch {
                throw;
            }
        }

        private NotificationDTO ConstructNewNotificationDTO(string title, string message, NotificationType type, string usernameFor) {
            return new NotificationDTO()
            {
                Title = title,
                Description = message,
                CreatedOn = DateTime.UtcNow,
                IsNew = true,
                UserFor = usernameFor,
                Type = type,
                IsSent = false,
                Id = Guid.NewGuid()
            };
        }

        #endregion Private Methods
    }
}
