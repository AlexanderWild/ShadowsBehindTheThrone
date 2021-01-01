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
                        if (location.turnLastTaken - soc.lastPlagueCrisis > location.map.param.society_crisis_plagueCrisisCooldown)
                        {
                            soc.crisisPlague = "The Red Death plague is spreading in our lands";
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
