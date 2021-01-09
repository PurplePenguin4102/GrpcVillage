using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcVillage.Engine
{
    public class Villager
    {
        public bool IsMale { get; set; }
        public bool IsFertile { get => Age > 15; }
        public bool IsPregnant { get; set; }
        public int MonthsUntilBirth { get; set; }
        public bool DeathRoll { get => Age > 28; }
        public int Age { get; set; }
        public int LifeExpectancy = 35;
        public int MonthBorn { get; set; }
    }
}
