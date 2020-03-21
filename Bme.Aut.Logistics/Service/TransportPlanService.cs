using Bme.Aut.Logistics.Dal;
using Bme.Aut.Logistics.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bme.Aut.Logistics.Service
{
    public class TransportPlanService
    {
        private readonly LogisticsDbContext dbContext;

        public TransportPlanService(LogisticsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // TODO: Megvalósítani az 5. a. feladat szerint
        // TIPP: ne fejeltsd a TransportPlan.Section eléréséhez az Include-ot
        public List<Milestone> GetFirstAndLastMilestone(long transportPlanId)
        {
            throw new NotImplementedException();
        }

        // TODO: Megvalósítani az 5. b. feladat szerint
        public void RegisterDelay(long planId, long milestoneId, int delayInMinutes)
        {
            throw new NotImplementedException();
        }

        // TODO: Megvalósítani az 5. c. feladat szerint
        public void AddSection(long planId, long fromMilestoneId, long toMilestoneId, int number)
        {
            throw new NotImplementedException();
        }
    }
}
