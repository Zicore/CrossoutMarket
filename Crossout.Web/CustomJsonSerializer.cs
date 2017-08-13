using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crossout.Web
{
    public sealed class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            this.Formatting = Formatting.Indented;
            this.TypeNameHandling = TypeNameHandling.None;
        }
    }
}
