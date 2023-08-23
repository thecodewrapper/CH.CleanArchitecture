using System;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class SMSService : ISMSService
    {
        public Task<Result> SendSMSAsync(string to, string body) {
            throw new NotImplementedException();
        }

        public Task<Result> SendSMSAsync(string from, string to, string body) {
            throw new NotImplementedException();
        }
    }
}
