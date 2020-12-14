using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_ChangeIdentity : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Adopting New Identity";
        }
        public override string getLong()
        {
            return "This agent is changing their identity, adopting a new name and fake history, to hide from the authorities. This will not protect against agents, who are skilled at tracking criminals.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.person == null) { unit.task = null;return; }

            string prevName = unit.getName();
            dur += 1;
            if (dur >= unit.location.map.param.unit_newIdentityTime)
            {
                unit.person.house = null;
                unit.person.lastName = TextStore.getName(false);

                int nHunters = 0;
                foreach (Unit u in unit.location.map.units)
                {
                    if (u.person != null && u is Unit_Investigator && u.person.getRelation(unit.person).suspicion > 0)
                    {
                        nHunters += 1;
                    }
                }
                foreach (SocialGroup sg in unit.location.map.socialGroups)
                {
                    if (sg is Society)
                    {
                        Society soc = (Society)sg;
                        soc.enemies.Remove(unit);
                        foreach (Person p in soc.people)
                        {
                            p.purgeRelObj(unit.person.index);
                        }
                    }
                }
                unit.location.map.world.prefabStore.popMsg(prevName + " has changed their identity, they are now known as " + unit.getName() +". This removes noble suspicion, and allows" +
                    " them to enter nations they were exiled from, but agents who are suspicious of them (" + nHunters + " agents) will not be fooled, and may well immediately begin warning the nobles again.");
                unit.task = null;

                Evidence e = new Evidence(unit.location.map.turn);
                e.pointsTo = unit;
                e.weight = unit.location.map.param.unit_majorEvidence;
                unit.location.evidence.Add(e);
            }
        }
    }
}