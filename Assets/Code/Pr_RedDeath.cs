using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_RedDeath : Property_Prototype
    {
        
        public Pr_RedDeath(Map map,string name) : base(map,name)
        {
            this.name = "Red Death";
            this.baseCharge = map.param.unit_rd_redDeathPlagueDur;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.prestigeChange = map.param.society_prestigeFromPlagueRedDeath;
            this.plagueThreat = 8;
        }

        public override void turnTick(Property p, Location location)
        {
            base.turnTick(p, location);

            
            if (location.settlement != null && location.settlement is SettlementHuman)
            {
                SettlementHuman set = (SettlementHuman)location.settlement;
                set.population -= 1;
            }

            foreach (Location l2 in location.getNeighbours())
            {
                if (l2.settlement != null && l2.settlement.isHuman && l2.soc != null && l2.soc is Society)
                {
                    bool canApply = true;
                    foreach (Property p2 in l2.properties)
                    {
                        if (p2.proto is Pr_RedDeath || p2.proto is Pr_RedDeathImmunity)
                        {
                            canApply = false;
                            break;
                        }
                    }
                    if (canApply && Eleven.random.NextDouble() < World.staticMap.param.unit_rd_redDeathPlaguePSpread)
                    {
                        Property.addProperty(World.staticMap, l2, "Red Death");
                        Society soc = (Society)l2.soc;
                        if (location.map.turn - soc.lastPlagueCrisis > location.map.param.society_crisis_plagueCrisisCooldown)
                        {
                            soc.crisisPlague = "The Red Death Plague";
                            soc.crisisPlagueLong = "The Red Death plague is spreading in our lands, we must deal with this new crisis. We can deploy our army to help the crisis," +
                                " at the cost of military power, either by quarantine or helping treat patients, or we could deploy our agents to assist.";
                            soc.lastPlagueCrisis = location.map.turn;
                        }
                    }
                }
            }
        }

        public override void endProperty(Location location, Property p)
        {
            Property.addProperty(World.staticMap, location, "Red Death Immunity");
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_redPlague;
        }

        internal override string getDescription()
        {
            return "Red death";
        }
    }
}
