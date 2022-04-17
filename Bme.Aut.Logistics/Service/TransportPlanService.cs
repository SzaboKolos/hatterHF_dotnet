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
            if (existsSection(number))
                return dbContext.Sections.First(x => x.Number == number);
            return null;
        }
        public Milestone FindMilestoneById(long milestoneId)
        {
            if (existsMilestone(milestoneId))
            {
                return dbContext.Milestones.ToList().First(x => x.Id == milestoneId);
            }
           return null;
        }
        public Boolean isToMilestone(long planId, long milestoneId)
        {
            if (existsMilestone(milestoneId) && FindTransportplanById(planId).Sections.Any(x => x.ToMilestoneId == milestoneId))
                return true;
            return false;
        }
        public Boolean existsPlan(long id)
        {
            return dbContext.TransportPlans.Any(x => x.Id == id);
        }
        public Boolean existsSection(int number)
        {
            return dbContext.Sections.Any(x => x.Number == number);
        }
        public Boolean existsMilestone(long milestoneId)
        {
            return dbContext.Milestones.Any(x=> x.Id == milestoneId);
        }
        public Boolean assignedMilestone(long planId, long milestoneId)
        {
            return FindTransportplanById(planId).Sections.Any(x => x.FromMilestoneId == milestoneId || x.ToMilestoneId == milestoneId);
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
            if (!existsPlan(planId) || !existsMilestone(milestoneId) || !assignedMilestone(planId,milestoneId))
            {
                throw new ArgumentException();
            }
            else
            {
                FindMilestoneById(milestoneId).PlannedTime = 
                                FindMilestoneById(milestoneId).PlannedTime.AddMinutes(delayInMinutes);

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
            Console.WriteLine("from: " + fromMilestoneId + " | to: " + toMilestoneId + " | number: "+number + " | planid: " +planId);

            //  F5/c/1
            if (!existsPlan(planId))
            {
                throw new ArgumentException("Plan doesn't exist");
            }
            TransportPlan plan = FindTransportplanById(planId);

            
            if (FindMilestoneById(toMilestoneId) == null || FindMilestoneById(fromMilestoneId) == null )
            {
                throw new ArgumentException("Non-existent milestone");
            }
            
            //  F5/c/2
            var MAX = plan.Sections.Count;
            Console.WriteLine(MAX);

            if (number < 0 || number > MAX)
            {
                throw new ArgumentException("Variable \"number\" is out of range");
            }


            //  F5/c/4
            if (plan.Sections.Any(x => x.Number > number)) {
                List<Section> sections = plan.Sections.Where(x => x.Number > number).ToList();
                Console.WriteLine(sections.Max(x=> x.Number));
                for (int i = 0; i < sections.Count; i++)
                {
                    sections[i].Number += 1;
                }
                Console.WriteLine(sections.Max(x => x.Number));
            }

            //  F5/c/3
            var newSection = new Section();
            newSection.Number = number;
            newSection.FromMilestoneId = fromMilestoneId;
            newSection.FromMilestone = FindMilestoneById(fromMilestoneId);
            Console.WriteLine(newSection.FromMilestone.ToString());
            newSection.ToMilestoneId = toMilestoneId;
            newSection.ToMilestone = FindMilestoneById(toMilestoneId);
            Console.WriteLine(newSection.ToMilestone.ToString());
            dbContext.SaveChanges();

            //  F5/c/5
            int prevNum = number - 1;
            int nextNum = number + 1;
            if (FindSectionByNumber(planId, prevNum) != null)
            {
                plan.Sections[prevNum].ToMilestone = newSection.FromMilestone;
                plan.Sections[prevNum].ToMilestoneId = newSection.FromMilestoneId;
            }
            if (FindSectionByNumber(planId,nextNum) != null)
            {
                plan.Sections[nextNum].FromMilestone = newSection.ToMilestone;
                plan.Sections[nextNum].FromMilestoneId = newSection.ToMilestoneId;
                plan.Sections.Insert(number,newSection);
                dbContext.SaveChanges();
                return;
            }
            else
            {
                FindTransportplanById(planId).Sections.Add(newSection);
                dbContext.SaveChanges();
            }
        }
    }
}
