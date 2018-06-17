using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Worker.Tasks;

namespace Crossout.Worker
{
    public static class Scheduler
    {
        public static Dictionary<BaseTask, DateTime> Schedule { get; set; } = new Dictionary<BaseTask, DateTime>();

        public static void ScheduleTasks()
        {
            Schedule.Clear();

            foreach (var task in TaskCollector.TaskList)
            {
                AddTask(task);
            }

            SortSchedule();
        }

        public static void UpdateTask(BaseTask task)
        {
            if (Schedule.ContainsKey(task))
            {
                Schedule.Remove(task);
                AddTask(task);
                SortSchedule();
            }
        }

        private static void AddTask(BaseTask task)
        {
            if (task.ExecutionTime != DateTime.MinValue && task.ExecutionInterval == TimeSpan.Zero)
            {
                if (!Schedule.ContainsKey(task))
                {
                    if (task.ExecutionTime.ToUniversalTime() > DateTime.UtcNow)
                    {
                        Schedule.Add(task, task.ExecutionTime.ToUniversalTime());
                    }
                    else
                    {
                        var execTime = task.ExecutionTime.AddDays(1);
                        Schedule.Add(task, execTime.ToUniversalTime());
                    }

                }
            }
            else if (task.ExecutionTime == DateTime.MinValue && task.ExecutionInterval != TimeSpan.Zero)
            {
                if (!Schedule.ContainsKey(task))
                {
                    DateTime nextExecution = DateTime.UtcNow.AddSeconds(task.ExecutionInterval.TotalSeconds);
                    Schedule.Add(task, nextExecution.ToUniversalTime());
                }
            }
            else if (task.ExecutionTime != DateTime.MinValue && task.ExecutionInterval != TimeSpan.Zero)
            {
                if (!Schedule.ContainsKey(task))
                {
                    DateTime nextExecution = task.ExecutionTime;
                    while (nextExecution.ToUniversalTime() < DateTime.UtcNow)
                    {
                        nextExecution = nextExecution.AddSeconds(task.ExecutionInterval.TotalSeconds);
                    }
                    Schedule.Add(task, nextExecution.ToUniversalTime());
                }
            }
        }

        private static void SortSchedule()
        {
            Schedule = Schedule.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
