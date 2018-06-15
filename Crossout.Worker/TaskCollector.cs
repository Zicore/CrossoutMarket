using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.Worker.Tasks;
using System.Reflection;

namespace Crossout.Worker
{
    public static class TaskCollector
    {
        public static List<BaseTask> TaskList = new List<BaseTask>();

        public static void CollectTasks()
        {
            List<Type> taskTypes = new List<Type>();
            foreach (var domainAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var assemblyType in domainAssembly.GetTypes())
                {
                    if (typeof(BaseTask).IsAssignableFrom(assemblyType) && assemblyType.Name != nameof(BaseTask))
                    {
                        taskTypes.Add(assemblyType);
                    }
                }
            }

            foreach (var taskType in taskTypes)
            {
                ConstructorInfo[] constuctors = taskType.GetConstructors();
                object taskObj = constuctors[0].Invoke(new object[] { taskType.Name });
                TaskList.Add((BaseTask)taskObj);
            }
        }

        public static void GenerateDefaultSettings()
        {
            foreach (var task in TaskList)
            {
                TaskSettings taskSettings = new TaskSettings() { Key = task.Key, ExecutionInterval = null, ExecutionTime = null };
                WorkerSettings.Settings.TaskSettings.Add(taskSettings);
            }
        }

        public static void ApplySettings()
        {
            foreach (var task in TaskList)
            {
                TimeSpan readTimeSpan;
                DateTime readDateTime;
                if (TimeSpan.TryParse(WorkerSettings.Settings.TaskSettings.Find(x => x.Key == task.Key).ExecutionInterval, out readTimeSpan))
                {
                    task.ExecutionInterval = readTimeSpan;
                }
                if (DateTime.TryParse(WorkerSettings.Settings.TaskSettings.Find(x => x.Key == task.Key).ExecutionTime, out readDateTime))
                {
                    task.ExecutionTime = readDateTime;
                }
            }
        }
    }
}
