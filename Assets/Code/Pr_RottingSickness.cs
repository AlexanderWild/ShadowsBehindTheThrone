using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_RottingSickness : Property_Prototype
    {
        
        public Pr_RottingSickness(Map map,string name) : base(map,name)
        {
            this.name = "Rotting Sickness";
            this.baseCharge = map.param.unit_doctor_rotSicknessDur;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.plagueThreat = 4;
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

            if (location.settlement != null && location.settlement is SettlementHuman & (location.map.turn % 2 == 0))
            {
                SettlementHuman set = (SettlementHuman)location.settlement;
                set.population -= 1;
                if (set.population <= 0)
                {
                    location.map.addMessage(set.location.getName() + " has been eradicated by the Rotting Sickness", MsgEvent.LEVEL_RED, true,location.hex);
                    if (set.title != null && set.title.heldBy != null)
                    {
                        set.title.heldBy.die("Died of Rotting Sickness", true);
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
                        if (p2.proto is Pr_RottingSickness || p2.proto is Pr_RottingSickness)
                        {
                            canApply = false;
                            break;
                        }
                    }
                    double pSpread = World.staticMap.param.unit_doctor_rotSicknessPSpread;
                    if (underQuarantine) { pSpread /= 2; }
                    if (canApply && Eleven.random.NextDouble() < pSpread)
                    {
                        Property.addProperty(World.staticMap, l2, "Rotting Sickness");
                        Society soc = (Society)l2.soc;
                        if (location.map.turn - soc.lastPlagueCrisis > location.map.param.society_crisis_plagueCrisisCooldown)
                        {
                            soc.crisisPlague = "The Rotting Sickness";
                            soc.crisisPlagueLong = "The Rotting Sickness is spreading in our lands, we must deal with this new crisis. " +
                                "We can either quarantine the disased settlements, treat infected patients, or we could deploy our agents as disease curing specialists.";
                            soc.lastPlagueCrisis = location.map.turn;
                        }
                    }
                }
            }
        }

        public override void endProperty(Location location, Property p)
        {
            Property.addProperty(World.staticMap, location, "Rotting Sickness Immunity");
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_rottingSickness;
        }

        internal override string getDescription()
        {
            return "The disease known as rotting sickness is present in this settlement. The population will decrease by 1 every 2 turns (the settlement will fall if it reaches 0), and the " +
                "military unit supported by this settlement has reduced maximum HP.";
        }
    }
}
