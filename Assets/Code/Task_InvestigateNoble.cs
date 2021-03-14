using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_InvestigateNoble : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Investigating Noble " + dur  + "/" + World.staticMap.param.unit_investigateNobleTime;
        }
        public override string getLong()
        {
            return "This agent is trying to bring to light evidence of the local noble's enthrallment if they complete this task, the noble will gain evidence.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.person() == null)
            {
                unit.task = null;
                return;
            }
            
            dur += 1;
            if (dur >= unit.location.map.param.unit_investigateNobleTime)
            {
                Person person = unit.location.person();


                if (person.state == Person.personState.enthralled || person.state == Person.personState.broken)
                {
                    person.evidence += unit.location.map.param.unit_investigateNobleEvidenceGain;
                    if (person.evidence > 1)
                    {
                        person.evidence = 1;
                    }

                    unit.location.map.world.prefabStore.popMsgAgent(unit,unit,unit.getName() + " has investigated " + person.getFullName() + ", as they suspected they were under the influence of dark powers." +
                        " Their investigation has caused " + person.getFullName() + " to gain " + ((int)(unit.location.map.param.unit_investigateNobleEvidenceGain * 100))
                        + "% evidence, and they are now at " + ((int)(person.evidence * 100)) + "%.");
                }
                else
                {
                    bool isCorrupt = false;
                    foreach (Trait t in person.traits)
                    {
                        if (t is Trait_Political_Corrupt)
                        {
                            isCorrupt = true;
                        }
                    }
                    if (isCorrupt)
                    {
                        person.prestige *= 0.33;
                        unit.location.map.world.prefabStore.popMsgAgent(unit,unit,unit.getName() + " has investigated " + person.getFullName() + " and discovered that while they are not broken or enthralled, they "
                            + "are politically corrupt. They have exposed this to the other nobles, causing " + person.getFullName() + " to lose prestige, and be forced to change their ways." +
                            " Honorable nobles will dislike them for their former corruption.");

                        foreach (Person p in person.society.people)
                        {
                            if (p == person) { continue; }
                            bool isHonorable = false;
                            foreach (Trait t in p.traits)
                            {
                                if (t is Trait_Political_Honorable)
                                {
                                    isHonorable = true;
                                }
                            }
                            if (isHonorable)
                            {
                                p.getRelation(person).addLiking(-25, "Honorable dislikes corruption", person.map.turn);
                            }
                        }

                        for (int i = 0; i < person.traits.Count; i++)
                        {
                            int c = 0;
                            if (person.traits[i].groupCode == Trait.CODE_POLITICS)
                            {
                                foreach (Trait t in person.map.globalist.allTraits)
                                {
                                    if (t.groupCode != Trait.CODE_POLITICS) { continue; }
                                    if (t is Trait_Political_Corrupt)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        c += 1;
                                        if (Eleven.random.Next(c) == 0)
                                        {
                                            person.traits[i] = t;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                person.investigationLastTurn = unit.location.map.turn;

                unit.task = null;
            }
        }
    }
}