using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Imagegram.Extensions
{
    internal static class HttpContextExtensions
    {
        internal static string GetUser(this HttpContext context)
        {
            var user = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            if (string.IsNullOrEmpty(user))
            {
                throw new ApplicationException("Couldn't get id of user from context");
            }

            return user;
        }
    }
}