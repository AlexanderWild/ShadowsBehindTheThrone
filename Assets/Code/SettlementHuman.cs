using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public abstract class SettlementHuman : Settlement
    {
        public int population;
        //public int infrastructure;

        public SettlementHuman(Location loc) : base(loc)
        {

        }

        public override void turnTick()
        {
            base.turnTick();
            statsTurnTick();
            humanTurnTick();
        }

        public abstract void humanTurnTick();

        public void statsTurnTick()
        {
            if (population < getMaxPopulation())
            {
                population += 1;
            }

            //if (infrastructure < population)
            //{
            //    infrastructure += 1;
            //}
        }

        public virtual int getMaxPopulation()
        {
            double multiplier = location.map.param.city_popMaxPerHabilitabilityMinor;
            int maxPop = (int)Math.Ceiling(0.1 + ((location.hex.getHabilitability() - location.map.param.mapGen_minHabitabilityForHumans) * multiplier));
            return maxPop;
        }

        internal string getStatsDesc()
        {
            string reply = "";
            reply += "Population Level:";
            reply += "\nPopulation Maximum:";
            //reply += "\nInfrastructure:";
            //reply += "\nDevelopment Level: " + Eleven.toFixedLen(getLevel(), 9);

            return reply;
        }

        public string getStatsValues()
        {
            string reply = "";
            reply += population;
            reply += "\n" + getMaxPopulation();
            //reply += "\n" + infrastructure;
            //reply += "\nDevelopment Level: " + Eleven.toFixedLen(getLevel(), 9);

            return reply;
        }


        internal override void takeAssault(SocialGroup sg, SocialGroup defender, double theirLosses)
        {
            int deltaP = Eleven.random.Next(location.map.param.combat_popDamageMax + 1);
            //int deltaI = Eleven.random.Next(location.map.param.combat_infraDamageMax + 1);
            //World.log("Damage received " + deltaP + " " + deltaI);
            population = Math.Max(0, population - deltaP);
            if (population == 0)
            {
                fallIntoRuin();
            }
            //infrastructure = Math.Max(1, infrastructure - deltaI);
        }
    }
}
