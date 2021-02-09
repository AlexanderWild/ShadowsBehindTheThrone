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
            this.milCapAdd = map.param.unit_rd_redDeathMilCapHit;
            this.isDisease = true;
        }

        public override void turnTick(Property p, Location location)
        {
            base.turnTick(p, location);

            bool underQuarantine = false;
            foreach (Property p2 in location.properties)
            {
                if (p2.proto is Pr_Quarantine) { underQuarantine = true;break; }
            }

            if (location.settlement != null && location.settlement is SettlementHuman)
            {
                SettlementHuman set = (SettlementHuman)location.settlement;
                set.population -= 1;
                if (set.population <= 0)
                {
                    location.map.addMessage(set.location.getName() + " has been eradicated by the Red Death", MsgEvent.LEVEL_RED, true);
                    if (set.title != null && set.title.heldBy != null)
                    {
                        set.title.heldBy.die("Died of the Red Death", true);
                    }
                    set.fallIntoRuin();
                }
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
                    double pSpread = World.staticMap.param.unit_rd_redDeathPlaguePSpread;
                    if (underQuarantine) { pSpread /= 2; }
                    if (canApply && Eleven.random.NextDouble() < pSpread)
                    {
                        Property.addProperty(World.staticMap, l2, "Red Death");
                        Society soc = (Society)l2.soc;
                        if (location.map.turn - soc.lastPlagueCrisis > location.map.param.society_crisis_plagueCrisisCooldown)
                        {
                            soc.crisisPlague = "The Red Death Plague";
                            soc.crisisPlagueLong = "The Red Death plague is spreading in our lands, we must deal with this new crisis. " +
                                "We can either quarantine the disased settlements, treat infected patients, or we could deploy our agents as disease curing specialists.";
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
            return "The disease known as the Red death is present in this settlement. The population will decrease by 1 each turn (the settlement will fall if it reaches 0), and the " +
                "military unit supported by this settlement has reduced maximum HP.";
        }
    }
}
