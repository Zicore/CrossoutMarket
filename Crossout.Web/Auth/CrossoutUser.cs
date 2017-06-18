using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web.Auth
{
    public class CrossoutUser : IUserIdentity
    {
        public static readonly CrossoutUser EmptyUser = new CrossoutUser(true);

        public CrossoutUser()
        {
            
        }

        private CrossoutUser(bool empty)
        {
            Empty = empty;
        }
        
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string AuthId { get; set; }
        public string AuthName { get; set; }
        public string AuthUsername { get; set; }
        public string AuthEmail { get; set; }

        public bool IsAdmin { get; set; }

        public bool Empty { get; private set; }

        public string UserName => Guid.ToString();

        public IEnumerable<string> Claims { get; set; }
    }
}
