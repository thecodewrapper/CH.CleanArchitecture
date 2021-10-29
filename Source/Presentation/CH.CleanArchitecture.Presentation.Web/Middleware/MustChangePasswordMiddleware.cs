using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CH.CleanArchitecture.Presentation.Web.Middleware
{
    public class MustChangePasswordMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public MustChangePasswordMiddleware(RequestDelegate next, IConfiguration configuration) {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context) {
            //Implement must-change password logic here
            await _next(context).ConfigureAwait(true);
        }
    }

    public static class MustChangePasswordMiddlewareExtensions
    {
        public static IApplicationBuilder UseMustChangePassword(
            this IApplicationBuilder builder) {
            return builder.UseMiddleware<MustChangePasswordMiddleware>();
        }
    }
}
