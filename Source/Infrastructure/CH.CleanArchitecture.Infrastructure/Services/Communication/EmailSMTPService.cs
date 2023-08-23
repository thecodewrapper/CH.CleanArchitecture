using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class EmailSMTPService : IEmailService
    {
        public Task<Result> SendEmailAsync(string from, string to, string subject, string message) {
            throw new NotImplementedException();
        }

        public Task<Result> SendEmailAsync(string to, string subject, string message) {
            throw new NotImplementedException();
        }

        public Task<Result> SendEmailAsync(string from, List<string> tos, string subject, string message) {
            throw new NotImplementedException();
        }

        public Task<Result> SendEmailAsync(List<string> tos, string subject, string message) {
            throw new NotImplementedException();
        }
    }
}
