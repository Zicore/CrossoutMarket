using System;

namespace Crossout.Web.Auth
{
    public interface IUserRepository
    {
        void SaveUser(CrossoutUser user);
        CrossoutUser LoadUser(string authId);
        CrossoutUser LoadUser(Guid guid);
    }
}