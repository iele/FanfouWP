using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Utils
{
    public class ScheduledTask
    {
        static PeriodicTask periodicTask;

        static string periodicTaskName = "PeriodicAgent";
        public static bool agentsAreEnabled = true;
        public static void RemoveAgent()
        {
            try
            {
                ScheduledActionService.Remove(periodicTaskName);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
        }

        public static void StartPeriodicAgent()
        {
            agentsAreEnabled = true;
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                RemoveAgent();
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            periodicTask.Description = "该后台用于定期获取最新消息通知";

            try
            {
                ScheduledActionService.Add(periodicTask);
                //ScheduledActionService.LaunchForTest(periodicTask.Name, TimeSpan.FromSeconds(60));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    agentsAreEnabled = false;
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {

                }
            }
            catch (SchedulerServiceException)
            {
            }
        }
    }
}
