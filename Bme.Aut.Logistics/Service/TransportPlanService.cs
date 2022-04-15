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
        public TransportPlan FindTransportplanById(long id)
        {
            if (dbContext.TransportPlans.ToList().First(x => x.Id == id) != null )
                return dbContext.TransportPlans.First(x => x.Id == id);
            return null;
        }
        public Boolean existsPlan(long id)
        {
            return dbContext.TransportPlans.Any(x => x.Id == id);
        }

        // TODO: Megvalósítani az 5. a. feladat szerint
        // TIPP: ne fejeltsd a TransportPlan.Section eléréséhez az Include-ot
        public List<Milestone> GetFirstAndLastMilestone(long transportPlanId)
        {
            List<Milestone> result = new List<Milestone>();
            if (existsPlan(transportPlanId))
            {
                if (FindTransportplanById(transportPlanId).Sections.Any()) {
                    result.Add(FindTransportplanById(transportPlanId).Sections
                        .OrderByDescending(y => y.Id).Last().FromMilestone);
                    result.Add(FindTransportplanById(transportPlanId).Sections
                        .OrderByDescending(y => y.Id).First().ToMilestone);
                }
            }
            else
                throw new ArgumentException();
            return result;
        }

        // TODO: Megvalósítani az 5. b. feladat szerint
        public void RegisterDelay(long planId, long milestoneId, int delayInMinutes)
        {
            if (FindTransportplanById(planId) == null || 
                FindTransportplanById(planId).Sections.ToList()
                .Where(x=> milestoneId == x.FromMilestoneId 
                || milestoneId == x.ToMilestoneId).ToList() == null)
            {
                throw new ArgumentException();
            }
            else
            {
                if (milestoneId == FindTransportplanById(planId).Sections.ToList()
                .Where(x => milestoneId == x.FromMilestoneId).First().FromMilestoneId)
                {
                    FindTransportplanById(planId).Sections.ToList()
                    .Where(x => milestoneId == x.FromMilestoneId).First().FromMilestone.PlannedTime.AddMinutes(delayInMinutes);

                    if (FindTransportplanById(planId).Sections.ToList()
                    .Where(x => milestoneId == x.FromMilestoneId).First().Number != 0)
                    {
                        var prevSectionNum = FindTransportplanById(planId).Sections.ToList()
                        .Where(x => milestoneId == x.FromMilestoneId).First().Number - 1;
                        FindTransportplanById(planId).Sections.ToList()
                        .Where(x => x.Number == prevSectionNum).First().ToMilestone.PlannedTime.AddMinutes(delayInMinutes);
                    }

                } else
                {
                    FindTransportplanById(planId).Sections.ToList()
                    .Where(x => milestoneId == x.ToMilestoneId).First().ToMilestone.PlannedTime.AddMinutes(delayInMinutes);

                    if (FindTransportplanById(planId).Sections.ToList()
                    .Where(x => milestoneId == x.FromMilestoneId).First().Number != 
                    (FindTransportplanById(planId).Sections
                    .OrderByDescending(y => y.Id).First().Number))
                    {
                        var nextSectionNum = FindTransportplanById(planId).Sections.ToList()
                        .Where(x => milestoneId == x.FromMilestoneId).First().Number + 1;
                        FindTransportplanById(planId).Sections.ToList()
                        .Where(x => x.Number == nextSectionNum).First().FromMilestone.PlannedTime.AddMinutes(delayInMinutes);
                    }

                }
            }
        }

        // TODO: Megvalósítani az 5. c. feladat szerint
        public void AddSection(long planId, long fromMilestoneId, long toMilestoneId, int number)
        {
              if (planId != FindTransportplanById(planId).Id)
               throw new ArgumentException();

              if (fromMilestoneId == null || toMilestoneId == null)
                throw new ArgumentException();

            var newSection = new Section();
            newSection.FromMilestoneId = fromMilestoneId;
            newSection.ToMilestoneId = toMilestoneId;
            newSection.Number = number;
            dbContext.TransportPlans.First(x => x.Id == planId).Sections.Add(newSection);
            dbContext.SaveChanges();
          
        }
    }
}
