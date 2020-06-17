using Crossout.Data.Info;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data
{
    public class ContributorCollection
    {
        public List<Contributor> Contributors { get; set; } = new List<Contributor>();

        public void ReadJsonResource(string path)
        {
            StreamReader sr = new StreamReader(path);

            string output = sr.ReadToEnd();

            Contributors = JsonConvert.DeserializeObject<List<Contributor>>(output);
        }
    }
}
