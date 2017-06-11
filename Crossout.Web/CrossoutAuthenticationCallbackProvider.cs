using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Crossout.Web.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.SimpleAuthentication;
using SimpleAuthentication.Core;

namespace Crossout.Web
{
    public class CrossoutAuthenticationCallbackProvider : IAuthenticationCallbackProvider
    {
        public CrossoutAuthenticationCallbackProvider(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        private readonly IUserRepository userRepository;

        public dynamic Process(NancyModule nancyModule, AuthenticateCallbackData model)
        {
            if (model.Exception != null)
            {
                throw new NotImplementedException("Login failure", model.Exception);
            }

            CrossoutUser loggedInUser = null;

            if (nancyModule.Context.CurrentUser != null)
            {
                loggedInUser = userRepository.LoadUser(nancyModule.Context.CurrentUser.UserName);
            }

            if (loggedInUser == null)
            {
                UserInformation userInfo = model.AuthenticatedClient.UserInformation;

                var user = userRepository.LoadUser(userInfo.Id);

                if (user == CrossoutUser.EmptyUser)
                {
                    userRepository.SaveUser(new CrossoutUser
                    {
                        AuthEmail = userInfo.Email,
                        AuthId = userInfo.Id,
                        AuthName = userInfo.Name,
                        AuthUsername = userInfo.UserName
                    });
                    user = userRepository.LoadUser(userInfo.Id);
                }
                return nancyModule.LoginAndRedirect(user.Guid, null, "~/admin");
            }

            return nancyModule.AsRedirectQueryStringOrDefault("~/admin");
        }

        public dynamic OnRedirectToAuthenticationProviderError(NancyModule nancyModule, string errorMessage)
        {
            throw new System.NotImplementedException(); // Provider canceled auth or it failed for some reason e. g. user canceled it
        }
    }
}
