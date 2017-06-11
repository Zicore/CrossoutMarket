using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace Crossout.Web.Auth
{
    public class UserMapper : IUserMapper
    {
        private readonly IUserRepository userRepository;
        public UserMapper(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var user = userRepository.LoadUser(identifier);

            if (user == CrossoutUser.EmptyUser)
            {
                return null;
            }

            var claims = new List<string> { "DefaultUser" };
            if (user.IsAdmin)
            {
                claims.Add("Admin");
            }
            user.Claims = claims;

            return user;
        }
    }
}
