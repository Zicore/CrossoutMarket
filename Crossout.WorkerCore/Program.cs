using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossout.WorkerCore.Tasks;
using System.Threading;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.WorkerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Collecting tasks...");
            TaskCollector.CollectTasks();
            TaskCollector.GenerateDefaultSettings();

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Loading settings...");
            WorkerSettings.Settings.Load();
            WorkerSettings.Settings.Save(); // Saving defaults

            TaskCollector.ApplySettings();

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Scheduling tasks...");
            Scheduler.ScheduleTasks();

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Connecting to DB...");
            SqlConnector sql = new SqlConnector(ConnectionType.MySql);
            sql.Open(WorkerSettings.Settings.CreateDescription());

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Worker started!");

            bool running = true;
            while (running)
            {
                if (Scheduler.Schedule.First().Value <= DateTime.UtcNow)
                {
                    RunFirstTaskInSchedule(sql);
                }
                Thread.Sleep(100);
            }
        }

        static void RunFirstTaskInSchedule(SqlConnector sql)
        {
            BaseTask task = Scheduler.Schedule.First().Key;
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Executing task: {task.Key}");
            Scheduler.UpdateTask(task);

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                task.Workload(sql);
            }, cts.Token);
        }
    }
}
