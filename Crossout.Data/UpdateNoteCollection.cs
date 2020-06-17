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
    public class UpdateNoteCollection
    {
        public List<UpdateNote> UpdateNotes { get; set; } = new List<UpdateNote>();

        public void ReadJsonResource(string path)
        {
            StreamReader sr = new StreamReader(path);

            string output = sr.ReadToEnd();

            UpdateNotes = JsonConvert.DeserializeObject<List<UpdateNote>>(output);
        }
    }
}
