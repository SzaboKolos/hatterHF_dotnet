using System.Collections.Generic;

namespace Bme.Aut.Logistics.Model
{
    public class Section
    {
        public long Id { get; set; }
        public Milestone FromMilestone { get; set; }
        public long FromMilestoneId { get; set; }
        public Milestone ToMilestone { get; set; }
        public long ToMilestoneId { get; set; }
        public TransportPlan TransportPlan { get; set; }
        public long TransportPlanId { get; set; }
        public int Number { get; set; }

    }
}