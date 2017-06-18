using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Crossout.Web.Auth;
using Microsoft.Owin;
using Nancy;
using Nancy.Helpers;
using Nancy.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Newtonsoft.Json;

namespace Crossout.Web
{
    public static class NancyExtensions
    {
        public static class CrossoutWebClaimTypes
        {
            public const string Identifier = "urn:crossoutweb:id";
            public const string Admin = "urn:crossoutweb:admin";
            public const string PartialIdentity = "urn:crossoutweb:partialid";
        }

        public static class Constants
        {
            public static readonly string AuthResultCookie = "crossoutdb.authResult";
            public static readonly Version JabbRVersion = typeof(Constants).Assembly.GetName().Version;
            public static readonly string CrossoutDBAuthType = "CrossoutDB";
        }

        public static Response SignIn(this NancyModule module, IEnumerable<Claim> claims)
        {
            var requestEnvironment = (IDictionary<string, object>) module.Context.Items["OWIN_REQUEST_ENVIRONMENT"];
            var owinContext = new OwinContext(requestEnvironment);

            var identity = new ClaimsIdentity(claims, Constants.CrossoutDBAuthType);
            owinContext.Authentication.SignIn(identity);

            return module.AsRedirectQueryStringOrDefault("~/");
        }

        public static Response SignIn(this NancyModule module, CrossoutUser user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(CrossoutWebClaimTypes.Identifier, user.AuthId));

            // Add the admin claim if the user is an Administrator
            if (user.IsAdmin)
            {
                claims.Add(new Claim(CrossoutWebClaimTypes.Admin, "true"));
            }

            return module.SignIn(claims);
        }

        public static void SignOut(this NancyModule module)
        {
            var requestEnvironment = (IDictionary<string, object>)module.Context.Items["OWIN_REQUEST_ENVIRONMENT"];
            var owinContext = new OwinContext(requestEnvironment);
            
            owinContext.Authentication.SignOut(Constants.CrossoutDBAuthType);
        }

        public static Response AsRedirectQueryStringOrDefault(this NancyModule module, string defaultUrl)
        {
            string returnUrl = module.Request.Query.returnUrl;
            if (String.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = defaultUrl;
            }

            return module.Response.AsRedirect(returnUrl);
        }

        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            if (env.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return default(T);
        }
    }
}
