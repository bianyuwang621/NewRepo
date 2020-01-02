using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace project44portail.SQLTrigger
{
    public class TrackingTrigger
    {
        private static TrackingTrigger _instance;

        private TrackingTrigger()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

            IJobDetail job = JobBuilder.Create<GetProLeg>().Build();
            ITrigger trigger = TriggerBuilder.Create().WithSimpleSchedule(s => s.WithIntervalInSeconds(120).RepeatForever()).StartNow().Build();

            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }


        public static TrackingTrigger GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TrackingTrigger();
            }
            return _instance;
        }

    }
}