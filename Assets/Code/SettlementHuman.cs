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
        public int infrastructure;

        public SettlementHuman(Location loc) : base(loc)
        {

        }

        public abstract int getMaxPopulation();

        internal string getStatsDesc()
        {
            string reply = "";
            reply += "Population Level:";
            reply += "\nPopulation Maximum:";
            reply += "\nInfrastructure:";
            //reply += "\nDevelopment Level: " + Eleven.toFixedLen(getLevel(), 9);

            return reply;
        }

        public string getStatsValues()
        {
            string reply = "";
            reply += population;
            reply += "\n" + getMaxPopulation();
            reply += "\n" + infrastructure;
            //reply += "\nDevelopment Level: " + Eleven.toFixedLen(getLevel(), 9);

            return reply;
        }
    }
}
