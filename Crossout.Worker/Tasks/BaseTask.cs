using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Worker.Tasks
{
    public abstract class BaseTask
    {
        protected BaseTask(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
        public TimeSpan ExecutionInterval { get; set; }
        public DateTime ExecutionTime { get; set; }

        public abstract void Workload(SqlConnector sql);
    }
}
