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
            List<Milestone> result = new List<Milestone>();
            if ((dbContext.TransportPlans.First(t => t.Id == transportPlanId).Sections) != null)
            {
                if (dbContext.TransportPlans.First(x => x.Id == transportPlanId) != null) {
                    result.Add((dbContext.TransportPlans
                        .First(x => x.Id == transportPlanId).Sections
                        .OrderByDescending(y => y.Id).First().FromMilestone));
                    result.Add((dbContext.TransportPlans
                        .First(x => x.Id == transportPlanId).Sections
                        .OrderByDescending(y => y.Id).Last().ToMilestone));
                }
                else
                    throw new ArgumentException();
            }
            //üres lesz, ha nem jut be az első if ágra
            return result;
        }

        // TODO: Megvalósítani az 5. b. feladat szerint
        public void RegisterDelay(long planId, long milestoneId, int delayInMinutes)
        {
            throw new NotImplementedException();
        }

        // TODO: Megvalósítani az 5. c. feladat szerint
        public void AddSection(long planId, long fromMilestoneId, long toMilestoneId, int number)
        {
            dbContext.TransportPlans.First(x => x.Id == planId).Sections.Add(new Section());

            dbContext.SaveChanges();
        }
    }
}
