using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;

namespace CH.CleanArchitecture.Core.Application
{
    public interface IEmailService
    {
        /// <summary>
        /// Send an email asynchronously
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<Result> SendEmailAsync(string from, string to, string subject, string message);

        /// <summary>
        /// Send an email asynchronously from the system's default email address
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<Result> SendEmailAsync(string to, string subject, string message);

        /// <summary>
        /// Send an email asynchronously to multiple recipients
        /// </summary>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<Result> SendEmailAsync(string from, List<string> tos, string subject, string message);

        /// <summary>
        /// Send an email asynchronously from the system's default email address to multiple recipients
        /// </summary>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<Result> SendEmailAsync(List<string> tos, string subject, string message);
    }
}
