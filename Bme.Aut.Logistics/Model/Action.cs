using System;
using System.Collections.Generic;

namespace Bme.Aut.Logistics.Model
{
    public class Action
    {
        public long Id { get; set; }
        public DateTime? DateTime { get; set; } = null;
        public ActionType Type { get; set; }


        public Milestone Milestone { get; set; }
    }

    public enum ActionType
    {
        BEPAKK,
        KIPAKK,
        VAMKEZELES,
        ELLENORZES,
    }
}