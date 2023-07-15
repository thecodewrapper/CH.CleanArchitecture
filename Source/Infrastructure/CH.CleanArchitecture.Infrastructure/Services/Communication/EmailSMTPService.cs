using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    public class EmailSMTPService : IEmailService
    {
        public Task<bool> SendEmailAsync(string from, string to, string subject, string message) {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailAsync(string to, string subject, string message) {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailAsync(string from, List<string> tos, string subject, string message) {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailAsync(List<string> tos, string subject, string message) {
            throw new NotImplementedException();
        }
    }
}
