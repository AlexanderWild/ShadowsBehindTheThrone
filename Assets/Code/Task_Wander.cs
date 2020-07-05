using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Wander : Task
    {
        public LinkedList<Location> visitBuffer = new LinkedList<Location>();
        public int bufferSize = 16;
        public int wanderDur = 8;
        public int wanderTimer = 0;

        public override string getShort()
        {
            return "Wandering";
        }
        public override string getLong()
        {
            return "This unit is patrolling their nation and surrounding lands, looking for evidence. If they find any, they will begin to investigate it.";
        }

        public override void turnTick(Unit unit)
        {

            if (unit.location.evidence.Count > 0)
            {
                unit.task = new Task_Investigate();
                return;
            }
            List<Location> neighbours = unit.location.getNeighbours();

            int c = 0;
            Location chosen = null;
            foreach (Location loc in neighbours)
            {
                if (loc.soc != null && loc.soc.hostileTo(unit)) { continue; }
                if (loc.evidence.Count > 0)
                {
                    chosen = loc;
                    break;
                }
                if (visitBuffer.Contains(loc) == false)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        chosen = loc;
                    }
                }
            }
            if (chosen != null)
            {
                visitBuffer.AddLast(chosen);
            }
            else
            {
                int q = Eleven.random.Next(neighbours.Count);
                chosen = neighbours[q];
            }
            unit.location.map.adjacentMoveTo(unit, chosen);
            if (visitBuffer.Count > bufferSize)
            {
                visitBuffer.RemoveFirst();
            }

            //Start investigating the evidence you just moved to
            if (unit.location.evidence.Count > 0)
            {
                unit.task = new Task_Investigate();
                return;
            }

            //See if you want to warn this new person about anything
            if (unit.location.person() != null)
            {
                Person noble = unit.location.person();
                foreach (RelObj rel in unit.person.relations.Values)
                {
                    ////Goes negative if they suspect more than we do, reaches 1.0 if we suspect 1.0 and they suspect 0.0
                    // if ((rel.suspicion - noble.getRelation(rel.them).suspicion) > Eleven.random.NextDouble())
                    if ((rel.suspicion > noble.getRelation(rel.them).suspicion*1.1))
                    {
                        unit.task = new Task_ShareSuspicions();
                        return;
                    }
                }

                if (noble.state == Person.personState.enthralled)
                {
                    bool alreadyInv = false;
                    foreach (Unit u2 in unit.location.units)
                    {
                        if (u2 is Unit_Investigator && u2.task is Task_InvestigateNoble)
                        {
                            alreadyInv = true;
                            break;
                        }
                    }
                    if ((!alreadyInv) && unit.location.map.turn - noble.investigationLastTurn > unit.location.map.param.unit_investigateNobleCooldown)
                    {
                        unit.location.map.world.prefabStore.popMsg(unit.getName() + " is beginning an investigation regarding " + noble.getFullName() + ", as they suspected they were under the influence of dark powers." +
                            " If their investigation ends, " +  noble.getFullName() + " will gain " + ((int)(unit.location.map.param.unit_investigateNobleEvidenceGain * 100))
                            + "% evidence. This task can be disrupted to prevent the evidence being generated, or the evidence can be given away to other friendly nobles.");

                        unit.task = new Task_InvestigateNoble();
                        return;
                    }
                }
            }

            wanderTimer += 1;
            if (wanderTimer > wanderDur)
            {
                if (unit.parentLocation != null)
                {
                    unit.task = new Task_GoToLocation(unit.parentLocation);
                }
                else
                {
                    //Done wandering, go home to ensure you don't wander too far for too long
                    unit.task = new Task_GoToSocialGroup(unit.society);
                }
            }
        }
    }
}