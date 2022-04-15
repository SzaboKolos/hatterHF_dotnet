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
            if (existsPlan(id))
                return dbContext.TransportPlans.First(x => x.Id == id);
            return null;
        }
        public Milestone FindMilestoneById(long planId, long milestoneId)
        {
            if (existsMilestone(planId, milestoneId))
            {
                if (isToMilestone(planId, milestoneId))
                {
                    return FindTransportplanById(planId).Sections
                        .First(x => x.ToMilestoneId == milestoneId).ToMilestone;
                } else
                {
                    return FindTransportplanById(planId).Sections
                        .First(x => x.FromMilestoneId == milestoneId).FromMilestone;
                }

            }
            return null;
        }
        public Boolean isToMilestone(long planId, long milestoneId)
        {
            if (existsMilestone(planId, milestoneId) && FindTransportplanById(planId).Sections.Any(x => x.ToMilestoneId == milestoneId))
                return true;
            return false;
        }
        public Boolean existsPlan(long id)
        {
            return dbContext.TransportPlans.Any(x => x.Id == id);
        }
        public Boolean existsMilestone(long planId, long milestoneId)
        {
            return FindTransportplanById(planId).Sections
                .ToList()
                .Any(x => x.ToMilestoneId == milestoneId || x.FromMilestoneId == milestoneId);
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
            if (!existsPlan(planId) || !existsMilestone(planId,milestoneId))
            {
                throw new ArgumentException();
            }
            else
            {
                Console.WriteLine(FindMilestoneById(planId, milestoneId));
                FindMilestoneById(planId, milestoneId).PlannedTime.AddMinutes(delayInMinutes);
                Console.WriteLine(FindMilestoneById(planId, milestoneId));

                dbContext.SaveChanges();
            }
        }

        // TODO: Megvalósítani az 5. c. feladat szerint
        public void AddSection(long planId, long fromMilestoneId, long toMilestoneId, int number)
        {
            if (planId != FindTransportplanById(planId).Id)
                throw new ArgumentException();

            if (existsMilestone(planId,fromMilestoneId) || existsMilestone(planId, toMilestoneId))
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
