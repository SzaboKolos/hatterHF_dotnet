using System.Collections.Generic;

namespace Bme.Aut.Logistics.Model
{
    public class TransportPlan
    {
        public long Id { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();
    }
}
