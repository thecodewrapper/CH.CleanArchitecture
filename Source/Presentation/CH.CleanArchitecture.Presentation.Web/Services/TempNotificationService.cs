using System;
using System.Collections.Generic;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Presentation.Web.Enumerations;
using CH.CleanArchitecture.Presentation.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace CH.CleanArchitecture.Presentation.Web.Services
{
    /// <summary>
    /// Temp Data service
    /// </summary>
    public class TempNotificationService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructor

        public TempNotificationService(IHttpContextAccessor httpContextAccessor,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            ILocalizationService localizationService) {
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _localizationService = localizationService;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Save message into TempData
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        private void PrepareTempData(TempNotificationType type, string message, bool encode = true) {
            var context = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(context);

            //Messages have stored in a serialized list
            var messages = tempData.ContainsKey(Constants.Constants.NotificationListKey)
                ? JsonConvert.DeserializeObject<IList<TempNotificationData>>(tempData[Constants.Constants.NotificationListKey].ToString())
                : new List<TempNotificationData>();

            messages.Add(new TempNotificationData
            {
                Message = message,
                Type = type,
                Encode = encode
            });

            tempData[Constants.Constants.NotificationListKey] = JsonConvert.SerializeObject(messages);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void Notification(TempNotificationType type, string message, bool encode = true) {
            PrepareTempData(type, message, encode);
        }

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void SuccessNotification(string message, bool encode = true) {
            PrepareTempData(TempNotificationType.Success, message, encode);
        }

        /// <summary>
        /// Display warning notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void WarningNotification(string message, bool encode = true) {
            PrepareTempData(TempNotificationType.Warning, message, encode);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        public virtual void ErrorNotification(string message, bool encode = true) {
            if (string.IsNullOrWhiteSpace(message)) {
                message = _localizationService[ResourceKeys.Common_SomethingWentWrong];
            }

            PrepareTempData(TempNotificationType.Error, message, encode);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        public virtual void ErrorNotification(Exception exception) {
            if (exception == null)
                return;

            ErrorNotification(exception.Message);
        }

        #endregion
    }
}
