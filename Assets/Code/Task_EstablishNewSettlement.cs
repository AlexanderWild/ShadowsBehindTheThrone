using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_EstablishNewSettlement : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Establishing New Settlement " + dur  + "/" + World.staticMap.param.unit_establishNewSettlementTime;
        }
        public override string getLong()
        {
            return "This agent is establishing a new human settlement, as they have found a habitable area with no existing settlement.";
        }

        public override void turnTick(Unit unit)
        {
            dur += 1;
            if (dur >= unit.location.map.param.unit_establishNewSettlementTime)
            {
                unit.task = null;

                Location loc = unit.location;
                int c = 0;
                Society receiver = null;
                foreach (Location l2 in loc.getNeighbours())
                {
                    if (l2.soc != null && l2.soc is Society)
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            receiver = (Society)l2.soc;
                        }
                    }
                }
                if (receiver == null)
                {
                    //foreach (SocialGroup sg in loc.map.socialGroups)
                    //{
                    //    if (sg is Society)
                    //    {
                    //        c += 1;
                    //        if (Eleven.random.Next(c) == 0)
                    //        {
                    //            receiver = (Society)sg;
                    //        }
                    //    }
                    //}
                    //Start a fully new colony.
                    //Maybe in time this could have a cool name, like "Merchant republic" and be a free port city or something
                    receiver = new Society(unit.location.map, loc);
                    receiver.setName(loc.shortName);
                    loc.soc = receiver;
                    unit.location.map.socialGroups.Add(receiver);
                }

                if (receiver != null)
                {
                    if (loc.isMajor)
                    {
                        loc.settlement = new Set_City(loc);
                    }
                    else
                    {
                        int q = 0;
                        double[] weights = new double[] { 2, 1, 2 };
                        double roll = 0;
                        for (int i = 0; i < weights.Length; i++) { roll += weights[i]; }
                        roll *= Eleven.random.NextDouble();
                        for (int i = 0; i < weights.Length; i++) { roll -= weights[i]; if (roll <= 0) { q = i; break; } }

                        if (q == 0)
                        {
                            loc.settlement = new Set_Abbey(loc);
                        }
                        else if (q == 1)
                        {
                            loc.settlement = new Set_University(loc);
                        }
                        else
                        {
                            loc.settlement = new Set_Fort(loc);
                        }
                    }

                    loc.soc = receiver;
                    SettlementHuman set = (SettlementHuman)loc.settlement;
                    set.population = 1;//Start at the start
                    loc.map.addMessage(loc.soc.getName() + " expands, add new settlement: " + loc.getName(), MsgEvent.LEVEL_RED, false);
                }
            }
        }
    }
}