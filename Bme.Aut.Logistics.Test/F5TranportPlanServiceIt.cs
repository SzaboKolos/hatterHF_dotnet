using Bme.Aut.Logistics.Dal;
using Bme.Aut.Logistics.Model;
using Bme.Aut.Logistics.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Bme.Aut.Logistics.Test
{
    [TestClass]
    public class F5TranportPlanServiceIt
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5GetMilestoneForNonExistingPlanThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                service.GetFirstAndLastMilestone(1L);
            }
        }

        [TestMethod]
        public void F5GetMilestoneForPlanWithNoSectionsReturnsEmptyList()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var plan = createAndAddEmptyTransportPlan(dbContext);

                var service = new TransportPlanService(dbContext);
                Assert.AreEqual(0, service.GetFirstAndLastMilestone(plan.Id).Count);
            }
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public void F5GetMilestoneForPlanWithNSectionsReturnsFirstAndLastMilestone(int n)
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var plan = createAndAddPlanWithSections(dbContext, n);

                var firstSection = plan.Sections.First();
                var lastSection = plan.Sections.Last();

                var service = new TransportPlanService(dbContext);
                CollectionAssert.AreEquivalent(new[] { firstSection.FromMilestone, lastSection.ToMilestone }, service.GetFirstAndLastMilestone(plan.Id));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5AddSectionForNonExistingPlanThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                service.AddSection(1L, 1, 1, 0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5AddSectionForNonExistingFromMilestoneIdThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddEmptyTransportPlan(dbContext);
                var milestoneId = createAndSaveNewMilestone(dbContext).Id;

                service.AddSection(plan.Id, -1, milestoneId, 0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5AddSectionForNonExistingToMilestoneIdThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddEmptyTransportPlan(dbContext);
                var milestoneId = createAndSaveNewMilestone(dbContext).Id;

                service.AddSection(plan.Id, milestoneId, -1, 0);
            }
        }

        [DataTestMethod]
        [DataRow(-11)]
        [DataRow(1)]
        [ExpectedException(typeof(ArgumentException))]
        public void F5AddSectionForInvalidNumberThrows(int number)
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddEmptyTransportPlan(dbContext);
                var fromMilestone = createAndSaveNewMilestone(dbContext);
                var toMilestone = createAndSaveNewMilestone(dbContext);

                service.AddSection(plan.Id, fromMilestone.Id, toMilestone.Id, number);
            }
        }

        [TestMethod]
        public void F5AddSectionForEmptyPlan()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddEmptyTransportPlan(dbContext);
                var fromMilestone = createAndSaveNewMilestone(dbContext);
                var toMilestone = createAndSaveNewMilestone(dbContext);

                service.AddSection(plan.Id, fromMilestone.Id, toMilestone.Id, 0);

                var planAfterAdd = fetchPlanFromDb(dbContext, plan.Id);
                Assert.IsNotNull(planAfterAdd);
                Assert.AreEqual(1, planAfterAdd.Sections.Count);

                assertSection(planAfterAdd.Sections[0], 0, fromMilestone, toMilestone);
            }
        }

        [TestMethod]
        public void F5AddSectionForEndOfNonEmptyPlan()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddPlanWithSections(dbContext, 1);
                var fromMilestone = createAndSaveNewMilestone(dbContext);
                var toMilestone = createAndSaveNewMilestone(dbContext);

                service.AddSection(plan.Id, fromMilestone.Id, toMilestone.Id, 1);

                var planAfterAdd = fetchPlanFromDb(dbContext, plan.Id);
                Assert.IsNotNull(planAfterAdd);
                Assert.AreEqual(2, planAfterAdd.Sections.Count);

                var oldSection = planAfterAdd.Sections[0];
                assertSection(oldSection, 0, plan.Sections[0].FromMilestone, fromMilestone);

                var newSection = planAfterAdd.Sections[1];
                assertSection(newSection, 1, fromMilestone, toMilestone);
            }
        }

        [TestMethod]
        public void F5AddSectionForMiddleOfPlan()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddPlanWithSections(dbContext, 2);
                var fromMilestone = createAndSaveNewMilestone(dbContext);
                var toMilestone = createAndSaveNewMilestone(dbContext);
                var originalSection1ToMilestone = plan.Sections[1].ToMilestone;

                service.AddSection(plan.Id, fromMilestone.Id, toMilestone.Id, 1);

                var planAfterAdd = fetchPlanFromDb(dbContext, plan.Id);
                Assert.IsNotNull(planAfterAdd);
                Assert.AreEqual(3, planAfterAdd.Sections.Count);

                var sections = planAfterAdd.Sections.OrderBy(s => s.Number).ToList();
                assertSection(sections[0], 0, plan.Sections[0].FromMilestone, fromMilestone);
                assertSection(sections[1], 1, fromMilestone, toMilestone);
                assertSection(sections[2], 2, toMilestone, originalSection1ToMilestone);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5RegisterDelayForNonExistingPlanThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                service.RegisterDelay(1L, 1L, 20);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5RegisterDelayForNonExistingMilestoneThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddEmptyTransportPlan(dbContext);

                service.RegisterDelay(plan.Id, 1L, 20);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void F5RegisterDelayForMilestoneNotAssignedToPlanThrows()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddEmptyTransportPlan(dbContext);
                var milestoneId = createAndSaveNewMilestone(dbContext).Id;

                service.RegisterDelay(plan.Id, milestoneId, 20);
            }
        }

        [TestMethod]
        public void F5RegisterDelayForPlannedTimeOfMilestoneIsIncreased()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddPlanWithSections(dbContext, 1);
                var originalMilestone = plan.Sections[0].FromMilestone;
                var originalPlannedTime = originalMilestone.PlannedTime;

                service.RegisterDelay(plan.Id, originalMilestone.Id, 20);

                var modifiedMilestone = fetchPlanFromDb(dbContext, plan.Id).Sections[0].FromMilestone;
                Assert.AreEqual(originalPlannedTime.AddMinutes(20), modifiedMilestone.PlannedTime);
            }
        }

        [TestMethod]
        public void F5RegisterDelayForPlannedTimeOfNextToMilestoneIsIncreasedInCaseOfFromMilestone()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddPlanWithSections(dbContext, 1);
                var originalMilestone = plan.Sections[0].FromMilestone;
                var originalPlannedTimeOfNextMilestone = plan.Sections[0].ToMilestone.PlannedTime;

                service.RegisterDelay(plan.Id, originalMilestone.Id, 20);

                var modifiedNextMilestone = fetchPlanFromDb(dbContext, plan.Id).Sections[0].ToMilestone;
                Assert.AreEqual(originalPlannedTimeOfNextMilestone.AddMinutes(20), modifiedNextMilestone.PlannedTime);
            }
        }

        [TestMethod]
        public void F5RegisterDelayForPlannedTimeOfNextFromMilestoneIsIncreasedInCaseOfToMilestone()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                var service = new TransportPlanService(dbContext);

                var plan = createAndAddPlanWithSections(dbContext, 2);
                var originalMilestone = plan.Sections[0].ToMilestone;
                var originalPlannedTimeOfNextMilestone = plan.Sections[1].FromMilestone.PlannedTime;

                service.RegisterDelay(plan.Id, originalMilestone.Id, 20);

                var modifiedNextMilestone = fetchPlanFromDb(dbContext, plan.Id).Sections[1].FromMilestone;
                Assert.AreEqual(originalPlannedTimeOfNextMilestone.AddMinutes(20), modifiedNextMilestone.PlannedTime);
            }
        }

        private static TransportPlan createAndAddEmptyTransportPlan(LogisticsDbContext dbContext)
        {
            var plan = new TransportPlan();
            dbContext.TransportPlans.Add(plan);
            dbContext.SaveChanges();
            return plan;
        }

        private TransportPlan createAndAddPlanWithSections(LogisticsDbContext dbContext, int count)
        {
            var plan = new TransportPlan();
            for (int i = 0; i < count; i++)
            {
                plan.Sections.Add(new Section()
                {
                    FromMilestone = createNewMilestone(),
                    ToMilestone = createNewMilestone(),
                    Number = i,
                });
            }

            dbContext.TransportPlans.Add(plan);
            dbContext.SaveChanges();
            return plan;
        }

        private TransportPlan fetchPlanFromDb(LogisticsDbContext dbContext, long id)
        {
            return dbContext.TransportPlans
                            .Include(p => p.Sections).ThenInclude(s => s.FromMilestone)
                            .Include(p => p.Sections).ThenInclude(s => s.ToMilestone)
                            .FirstOrDefault(p => p.Id == id);
        }

        private Milestone createNewMilestone()
        {
            return new Milestone()
            {
                PlannedTime = new DateTime(2020, 5, 12, 13, 0, 0, DateTimeKind.Local),
                Address = new Address(50, 19, "HU", "Budapest", "1111", "Vaci utca", "77"),
            };
        }

        private Milestone createAndSaveNewMilestone(LogisticsDbContext dbContext)
        {
            var milestone = createNewMilestone();
            dbContext.Milestones.Add(milestone);
            dbContext.SaveChanges();
            return milestone;
        }

        private void assertSection(Section section, int expectedNumber, Milestone expectedFromMilestone, Milestone expectedToMilestone)
        {
            Assert.AreEqual(expectedNumber, section.Number);
            Assert.AreEqual(expectedFromMilestone, section.FromMilestone);
            Assert.AreEqual(expectedToMilestone, section.ToMilestone);
        }
    }
}
