using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Merchant : Unit
    {
        public int cash = 0;
        public int cargo = 0;


        public Unit_Merchant(Location loc,Society soc) : base(loc,soc)
        {
            maxHp = 2;
            hp = 2;
            abilities.Add(new Abu_Base_Infiltrate());
            abilities.Add(new Abu_Merch_LoadCargo());
            abilities.Add(new Abu_Merch_SellCargo());
            abilities.Add(new Abu_Merch_SpendWealth());
            abilities.Add(new Abu_Base_PleadCase());
            abilities.Add(new Abu_Base_Recruit());
            //abilities.Add(new Abu_Base_Disrupt());
            abilities.Add(new Abu_Base_SpreadShadow());
        }

        public override void turnTickInner(Map map)
        {
            if (map.socialGroups.Contains(society) == false)
            {
                disband(map, "Society is gone");
                return;
            }
        }

        public override void turnTickAI(Map map)
        {
            if (task != null)
            {
                task.turnTick(this);
                return;
            }

            if (this.cash >= 50)
            {
                if (location.soc == society)
                {
                    task = new Task_SpendWealth();
                    return;
                }
                else
                {
                    task = new Task_GoToSocialGroup(society);
                    return;
                }
            }
            if (this.cargo >= 50)
            {
                if (location.soc != null && location.soc != society)
                {
                    task = new Task_SellCargo();
                    return;
                }
                double bestV = 0;
                Location bestLoc = null;
                foreach (Location l2 in map.locations)
                {
                    if (l2.soc is Society && l2.soc != society && (l2.soc.hostileTo(this) == false))
                    {
                        double v = Eleven.random.NextDouble() * location.map.getDist(location.hex, l2.hex);
                        if (v > bestV)
                        {
                            bestV = v;
                            bestLoc = l2;
                        }
                    }
                }
                if (bestLoc != null)
                {
                    task = new Task_GoToLocation(bestLoc);
                    return;
                }

                //Otherwise
                if (location.soc is Society && (location.soc.hostileTo(this) == false) && location.settlement != null)
                {
                    //Couldn't find anywhere better
                    task = new Task_SellCargo();
                    return;
                }
            }
            else
            {
                if (location.soc == society)
                {
                    task = new Task_LoadCargo();
                    return;
                }
                else
                {
                    task = new Task_GoToSocialGroup(society);
                }
            }
        }
            

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_trader;
        }

        public override string getTitleM()
        {
            return "Merchant";
        }

        public override string getTitleF()
        {
            return "Merchant";
        }

        public override string getDesc()
        {
            return "Merchants are travelling traders, which collect cargo from their home nation and sell it in foreign lands.";
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }
        public override string specialInfoLong()
        {
            return "Merchants work using the trade mechanic. They load cargo in their home nation, then sell to gain wealth. This wealth can be spent to gain favour and increase prestige of nobles."
                + "\n\nMetropoles earn more profit when selling than cities, which earn more than towns, which earn more than other locations. Foreign nations earn more than the unit's own nation.";
        }
        public override string specialInfo()
        {
            return "Wealth: \t" + cash + "%" + "\nCargo:  \t" + cargo + "%";
        }
        public override Color specialInfoColour()
        {
            return new Color(0.8f, 0.65f, 0);
        }
    }
}
