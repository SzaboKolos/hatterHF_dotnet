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
        public Section FindSectionByNumber(long planId, int number) 
        {
            if (existsSection(planId, number))
                return FindTransportplanById(planId).Sections.First(x => x.Number == number);
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
        public Boolean existsSection(long planId, int number)
        {
            return FindTransportplanById(planId).Sections.Any(x => x.Number == number);
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
                FindMilestoneById(planId, milestoneId).PlannedTime = 
                                FindMilestoneById(planId, milestoneId).PlannedTime.AddMinutes(delayInMinutes);

                //From
                if (isToMilestone(planId, milestoneId))
                {
                    int nextNum = FindTransportplanById(planId).Sections.First(x => x.ToMilestoneId == milestoneId).Number + 1;                               
                        FindTransportplanById(planId).Sections[nextNum].FromMilestone.PlannedTime =
                                    FindTransportplanById(planId).Sections[nextNum].FromMilestone.PlannedTime.AddMinutes(delayInMinutes);
                }
                //To
                else
                {
                    int num = FindTransportplanById(planId).Sections.First(x => x.FromMilestoneId == milestoneId).Number;
                        FindTransportplanById(planId).Sections[num].ToMilestone.PlannedTime =
                                    FindTransportplanById(planId).Sections[num].ToMilestone.PlannedTime.AddMinutes(delayInMinutes);
                    
                }

                dbContext.SaveChanges();
            }
        }

        // TODO: Megvalósítani az 5. c. feladat szerint
        public void AddSection(long planId, long fromMilestoneId, long toMilestoneId, int number)
        {

            //  F5/c/1
            if (!existsPlan(planId))
            {
                throw new ArgumentException("Nem létező szállítási terv!");
            }
            
            if (!existsMilestone(planId, fromMilestoneId) || !existsMilestone(planId, toMilestoneId))
            {
                throw new ArgumentException("Nem létező milestone!");
            }
            //  F5/c/2

            int MAX = FindTransportplanById(planId).Sections.Max(x => x.Number);
            if (number < 0 || number >= MAX)
            {
                throw new ArgumentException("number hiba");
            }
            //  F5/c/3
            Section newSection = new Section();
            newSection.Number = number;
            newSection.FromMilestoneId = fromMilestoneId;
            newSection.ToMilestoneId = toMilestoneId;
            FindTransportplanById(planId).Sections.Add(newSection);

            //  F5/c/4
            List<Section> sections = FindTransportplanById(planId).Sections.ToList().Where(x => x.Number >= number).ToList();
            foreach (var section in sections)
            {
                section.Number += 1;
            }
            //  F5/c/5
            if (number > 0)
                FindTransportplanById(planId).Sections[number + 1].FromMilestone = FindMilestoneById(planId, toMilestoneId);
            if (number < MAX)
                FindTransportplanById(planId).Sections[number - 1].ToMilestone = FindMilestoneById(planId, fromMilestoneId);
            
            dbContext.SaveChanges();
        
        }
    }
}
