using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Wander_Inquisitor : Task
    {
        public LinkedList<Location> visitBuffer = new LinkedList<Location>();
        public int bufferSize = 16;
        public int wanderDur = 8;
        public int wanderTimer = 0;

        public override string getShort()
        {
            return "Seeking Guilty Nobles";
        }
        public override string getLong()
        {
            return "This unit is patrolling their nation, looking for nobles who are suspected to be undermining the nation from within. If they find any, they will begin to investigate.";
        }

        public override void turnTick(Unit unit)
        {

            List<Location> neighbours = unit.location.getNeighbours();

            int c = 0;
            Location chosen = null;
            foreach (Location loc in neighbours)
            {
                if (loc.soc != unit.society) { continue; }
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


            //See if you want to warn this new person about anything
            if (unit.location.person() != null)
            {
                Person noble = unit.location.person();

                bool shouldInvestigate = noble.state == Person.personState.enthralled || noble.state == Person.personState.broken;
                if (noble.shadow > Eleven.random.NextDouble()) { shouldInvestigate = true; }
                foreach (Trait t in noble.traits)
                {
                    if (t is Trait_Political_Corrupt) { shouldInvestigate = true; }
                }

                if (shouldInvestigate)
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
                        if (noble.state == Person.personState.enthralled)
                        {
                            unit.location.map.world.prefabStore.popMsgAgent(unit, unit, unit.getName() + " is beginning an investigation regarding " + noble.getFullName() + ", as they suspected they were under the influence of dark powers." +
                                " If their investigation ends, " + noble.getFullName() + " will gain " + ((int)(unit.location.map.param.unit_investigateNobleEvidenceGain * 100))
                                + "% evidence. This task can be disrupted to prevent the evidence being generated, or the evidence can be given away to other friendly nobles.");
                        }

                        noble.investigationLastTurn = unit.location.map.turn;

                        unit.task = new Task_InvestigateNoble();
                        return;
                    }
                }
            }

            wanderTimer += 1;
            if (wanderTimer > wanderDur)
            {
                //if (unit.parentLocation != null)
                //{
                //    unit.task = new Task_GoToLocation(unit.parentLocation);
                //}
                //else
                //{
                //    //Done wandering, go home to ensure you don't wander too far for too long
                //    unit.task = new Task_GoToSocialGroup(unit.society);
                //}
                unit.task = null;
            }
        }
        public override bool isBusy()
        {
            return false;
        }
    }
}