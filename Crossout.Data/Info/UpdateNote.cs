using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Data.Info
{
    public class UpdateNote
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public string FormatTimestamp { get => Timestamp.ToString("yyyy-MM-dd HH:mm:ss"); }
        public List<string> Notes { get; set; }
    }
}
