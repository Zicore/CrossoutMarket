using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Model;
using Crossout.Web.Services;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Auth
{
    public class SqlUserRepository : IUserRepository
    {
        public SqlConnector DB { get; set; }

        public SqlUserRepository(SqlConnector db)
        {
            this.DB = db;
        }

        public void SaveUser(CrossoutUser user)
        {
            var existingUser = LoadUser(user.Guid);
            if (existingUser == CrossoutUser.EmptyUser)
            {
                var queryResult = DB.Insert("user", new string[]
                {
                    "guid",
                    "auth_id",
                    "name",
                    "username",
                    "email"
                }, new object[]
                {
                    Guid.NewGuid(),
                    user.AuthId,
                    user.AuthName,
                    user.AuthUsername,
                    user.AuthEmail
                });
            }
        }

        public CrossoutUser LoadUser(string authId)
        {
            var ds = DB.SelectDataSet(BuildUserByAuthIdQuery(), new List<Parameter> {new Parameter { Identifier = "@auth_id", Value = authId} });
            return CreateUserFromDataSet(ds);
        }

        public CrossoutUser LoadUser(Guid guid)
        {
            var ds = DB.SelectDataSet(BuildUserByGuidQuery(), new List<Parameter> { new Parameter { Identifier = "@guid", Value = guid } });
            return CreateUserFromDataSet(ds);
        }

        private static string BuildUserByAuthIdQuery()
        {
            string query = "SELECT user.id,user.guid,user.auth_id,user.name,user.username,user.email,user.is_admin FROM user WHERE user.auth_id = @auth_id";
            return query;
        }

        private static string BuildUserByGuidQuery()
        {
            string query = "SELECT user.id,user.guid,user.auth_id,user.name,user.username,user.email,user.is_admin FROM user WHERE user.guid = @guid";
            return query;
        }

        private static CrossoutUser CreateUserFromDataSet(List<object[]> ds)
        {
            if (ds.Count == 0)
            {
                return CrossoutUser.EmptyUser;
            }

            int i = 0;
            var row = ds[0];
            CrossoutUser user = new CrossoutUser
            {
                Id = row[i++].ConvertTo<int>(),
                Guid = Guid.Parse(row[i++].ConvertTo<string>()),
                AuthId = row[i++].ConvertTo<string>(),
                AuthName = row[i++].ConvertTo<string>(),
                AuthUsername = row[i++].ConvertTo<string>(),
                AuthEmail = row[i++].ConvertTo<string>(),
                IsAdmin = row[i++].ConvertTo<bool>(),
            };
            return user;
        }
    }
}
