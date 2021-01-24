using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_LethalStrain : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Increasing Lethality";
        }
        public override string getLong()
        {
            return "This agent is increasing the severity of the infection in this and neighbouring locations, causing additional population drops per turn.";
        }

        public override void turnTick(Unit unit)
        {
            List<Location> locs = new List<Location>();
            locs.Add(unit.location);
            foreach (Location loc in unit.location.getNeighbours())
            {
                locs.Add(loc);
            }

            bool hadEffect = false;
            foreach (Location loc in locs)
            {
                if (loc.settlement == null || (loc.settlement is SettlementHuman == false)) { continue; }
                bool hasDisease = false;
                foreach (Property pr in loc.properties)
                {
                    if (pr.proto.isDisease)
                    {
                        hasDisease = true;
                    }
                }
                if (hasDisease)
                {
                    hadEffect = true;
                    SettlementHuman set = (SettlementHuman)loc.settlement;
                    set.population -= 1;
                    if (set.population <= 0)
                    {
                        if (set.title != null && set.title.heldBy != null)
                        {
                            set.title.heldBy.die("Died of a lethal strain of disease, caused by " + unit.getName(),true);
                            set.title.heldBy = null;
                        }
                        set.fallIntoRuin();
                    }
                }
            }
            if (!hadEffect)
            {
                unit.location.map.world.prefabStore.popMsgAgent(unit, unit, unit.getName() + " can no longer increase the lethality of surrounding diseases, " +
                    "as there are no longer any infected human settlements in their location or surrounding.");
                unit.task = null;
            }
        }
    }
}