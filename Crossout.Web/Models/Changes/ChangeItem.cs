using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossout.Web.Models.Changes
{
    public class ChangeItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ChangeType { get; set; }
        public string Field { get; set; }
        public string TranslatedField { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimestampString => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
