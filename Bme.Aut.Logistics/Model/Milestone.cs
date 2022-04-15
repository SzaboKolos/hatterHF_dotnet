using System;
using System.Collections.Generic;

namespace Bme.Aut.Logistics.Model
{
    public class Milestone
    {
        public long Id { get; set; }
        public Address Address { get; set; }
        public long AddressId { get; set; }
        public DateTime PlannedTime { get; set; }

        public List<Action> Action { get; set; }

        override
        public string ToString()
        {
            return ("MS: "+ Id + " [" + PlannedTime + "]");
        }
    }
}