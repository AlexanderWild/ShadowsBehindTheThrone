using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Investigator : Unit
    {
        public int sinceHome = 0;
        public int lastTurnPromoted = 0;
        public int wanderDur = 8;

        public Unit victim;
        public Unit paladinTarget;
        public Location falseEvidenceDropLocation;
        public int victimUses = 0;
        public List<Evidence> evidenceCarried = new List<Evidence>();
        public List<Unit> huntableSuspects = new List<Unit>();
        public int paladinDuration = 0;

        public enum unitState { basic,investigator,paladin,knight };
        public unitState state = unitState.basic;

        public Unit_Investigator(Location loc,Society soc) : base(loc,soc)
        {
            maxHp = 5;
            hp = 2;
            abilities.Add(new Abu_Inv_FalseAccusation());
            abilities.Add(new Abu_Inv_ProduceFalseEvidence());
            abilities.Add(new Abu_Inv_Incriminate());
            abilities.Add(new Abu_Base_Infiltrate());
            abilities.Add(new Abu_Base_SocialiseAtCourt());
            abilities.Add(new Abu_Base_PleadCase());
            abilities.Add(new Abu_Base_Recruit());
            abilities.Add(new Abu_Base_ChangeIdentity());
            //abilities.Add(new Abu_Base_Disrupt());
            //if (loc.map.simplified == false)
            //{
            //    abilities.Add(new Abu_Base_EnthrallNoble());
            //}
            abilities.Add(new Abu_Base_SpreadShadow());
        }

        public override void turnTickInner(Map map)
        {
        }

        public override void turnTickAI(Map map)
        {
            checkSocietyPresence();
            checkHostilityGain();
            if (checkRetreat()) { return; }
            if (state == unitState.basic)
            {
                turnTickAI_Basic(map);
            }
            if (state == unitState.investigator)
            {
                turnTickAI_Investigator(map);
            }
            if (state == unitState.paladin)
            {
                turnTickAI_Paladin(map);
            }
        }

        public void checkSocietyPresence()
        {
            if (parentLocation.soc != this.society && parentLocation.soc is Society) { this.society = parentLocation.soc; }
        }

        public bool checkRetreat()
        {
            if (this.task  is Task_GoToSocialGroup || task is Task_Resupply)
            {
                task.turnTick(this);
                return true;
            }

            if (hp <= (maxHp / 2) + 1 && hp < maxHp)
            {
                //Damaged beyond safety margins, retreat advised
                if (this.location.soc == this.society)
                {
                    if (this.task is Task_Resupply)
                    {
                        return true;
                    }
                    else if (this.task is Task_Disrupted)
                    {
                        return true;
                    }
                    else
                    {
                        this.task = new Task_Resupply();
                        return true;
                    }
                }
                else
                {
                    if (this.task is Task_GoToSocialGroup)
                    {
                        return true;
                    }
                    else
                    {
                        this.task = new Task_GoToSocialGroup(society);
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public void checkHostilityGain()
        {
            foreach (RelObj rel in person.relations.Values)
            {
                if (rel.suspicion >= 1)
                {
                    Person p = person.map.persons[rel.them];
                    if (p == person) { throw new Exception("Badly implemented lookup"); }
                    if (p.unit != null)
                    {
                        //We're suspicious of this unit
                        if (hostility.Contains(p.unit) == false)
                        {
                            //We should become hostile to them, as we are now certain that they are evil
                            person.map.world.prefabStore.popMsgAgent(this, p.unit, this.getName() + " has become hostile to " + p.unit.getName());
                            hostility.Add(p.unit);
                        }
                    }
                }
            }
        }
        public void turnTickAI_Paladin(Map map)
        {
            paladinDuration -= 1;
            if (map.units.Contains(paladinTarget) == false)
            {
                this.task = null;
                this.state = unitState.investigator;
                map.world.prefabStore.popMsg(this.getName() + " has completed their quest to hunt down " + paladinTarget.getName() + ", and can now return to their normal duties.");
                return;

            }
            if (paladinDuration <= 0)
            {
                this.task = null;
                this.state = unitState.investigator;
                map.world.prefabStore.popMsg(this.getName() + " has run out of supplies to pursue " + paladinTarget.getName() + " and must end their quest.");
                return;
            }

            if (task == null)
            {
                task = new Task_HuntEnthralled_Investigator(this,paladinTarget);
            }

            task.turnTick(this);
        }

        public void turnTickAI_Investigator(Map map)
        {
            foreach (Evidence ev in evidenceCarried)
            {
                if (ev.pointsTo != null && ev.pointsTo != this)
                {
                    if (huntableSuspects.Contains(ev.pointsTo) == false)
                    {
                        huntableSuspects.Add(ev.pointsTo);
                    }
                }
            }

            if (this.location.soc == society)
            {
                sinceHome = 0;
                if (this.hp < maxHp && task == null && location.settlement != null)
                {
                    task = new Task_Resupply();
                }
            }
            else
            {
                sinceHome += 1;
            }

            //Scan local units
            if (map.param.unit_investigatorsSeeEnthralled == 1)
            {
                foreach (Unit u in location.units)
                {
                    if (u.isEnthralled() && u.person != null && this.person.getRelation(u.person).suspicion > 0)
                    {
                        if (this.person != null && u.person != null)
                        {
                            this.person.getRelation(u.person).suspicion = Math.Min(1, this.person.getRelation(u.person).suspicion + map.param.unit_suspicionFromProximity);
                            map.addMessage(this.getName() + " has gained suspicion of " + u.getName(), MsgEvent.LEVEL_ORANGE, false);
                        }
                    }
                }
            }


            if (task != null)
            {
                task.turnTick(this);
                return;
            }

            if (shouldBePaladin(map))
            {
                Unit target = null;
                double bestDist = -1;
                foreach (Unit u in huntableSuspects)
                {
                    if (map.units.Contains(u))
                    {
                        double dist = map.getStepDist(u.location, location);
                        if (bestDist == -1 || dist < bestDist)
                        {
                            bestDist = dist;
                            target = u;
                        }
                    }
                }

                task = new Task_HuntEnthralled_Investigator(this,target);
                paladinTarget = target;
                state = unitState.paladin;
                lastTurnPromoted = map.turn;
                if (hostility.Contains(paladinTarget) == false)
                {
                    hostility.Add(paladinTarget);
                }
                paladinDuration = map.param.unit_paladin_promotionDuration;
                map.world.prefabStore.popMsgAgent(this, target, this.getName() + " has found evidence that " + paladinTarget.getName() + " is in league with the darkness," +
                    " and has been granted the powers of the paladin, to hunt them down for a duration. They are currently in " + location.getName() + ".");

                //Update your graphic
                if (this.outer != null)
                {
                    this.outer.checkData();
                }
                return;
            }
            else if (location.evidence.Count > 0)
            {
                task = new Task_Investigate();
            }
            else if (sinceHome > wanderDur)
            {
                task = new Task_GoToSocialGroup(society);
            }
            else
            {
                task = new Task_Wander();
            }


            task.turnTick(this);
        }

        public void turnTickAI_Basic(Map map)
        {
            foreach (Evidence ev in evidenceCarried)
            {
                if (ev.pointsTo != null && ev.pointsTo != this)
                {
                    if (huntableSuspects.Contains(ev.pointsTo) == false)
                    {
                        huntableSuspects.Add(ev.pointsTo);
                    }
                }
            }

            if (this.location.soc == society)
            {
                sinceHome = 0;
                if (this.hp < maxHp && task == null && location.settlement != null)
                {
                    task = new Task_Resupply();
                }
            }
            else
            {
                sinceHome += 1;
            }


            if (task != null)
            {
                task.turnTick(this);
                return;
            }

            else if (location.evidence.Count > 0)
            {
                task = new Task_Investigate();
            }
            else if (sinceHome > wanderDur)
            {
                task = new Task_GoToSocialGroup(society);
            }
            else
            {
                task = new Task_Wander();
            }


            task.turnTick(this);
        }

        public bool shouldBePaladin(Map map)
        {
            if (location.soc != this.society) { return false; }
            if (map.simplified) { return false; }
            //if (map.automatic) { return false; }

            int nPaladins = 0;
            foreach (Unit u in map.units)
            {
                if (u is Unit_Investigator)
                {
                    Unit_Investigator inv = (Unit_Investigator)u;
                    if (inv.state == unitState.paladin)
                    {
                        nPaladins += 1;
                    }
                }
            }
            if (nPaladins >= map.unitManager.getTargetPaladins()) { return false; }

            foreach (Unit u in huntableSuspects)
            {
                if (map.units.Contains(u))
                {
                    return true;
                }
            }
            return false;
        }


        public static double getSwitchUtility(Person person,Unit_Investigator swappable,unitState hypo)
        {
            double value = 0;

            double worstMilitaryFear = 0;
            foreach (ThreatItem item in person.threatEvaluations)
            {
                if (item.responseCode == ThreatItem.RESPONSE_MILITARY)
                {
                    if (item.threat > worstMilitaryFear) { worstMilitaryFear = item.threat; }
                }
            }
            double[] weights = new double[] { person.threat_agents.threat, worstMilitaryFear };
            //Avoids excessive specialisation
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] += 0.2;
            }

            double[] currentAllocation = new double[2];
            double[] futureAllocation = new double[2];

            int specialisationWeight = 3;
            foreach (Unit u in person.map.units)
            {
                if (u.society == person.society && u is Unit_Investigator)
                {
                    Unit_Investigator existing = (Unit_Investigator)u;
                    if (existing.state == unitState.basic)
                    {
                        //Basic covers all jobs
                        for (int i = 0; i < currentAllocation.Length; i++)
                        {
                            currentAllocation[i] += 1;
                            if (u != swappable)
                            {
                                futureAllocation[i] += 1;
                            }
                        }
                    }
                    else if (existing.state == unitState.investigator || existing.state == unitState.paladin)
                    {
                        currentAllocation[0] += specialisationWeight;
                        if (u != swappable)
                        {
                            futureAllocation[0] += specialisationWeight;
                        }
                    }
                    else if (existing.state == unitState.knight)
                    {
                        currentAllocation[1] += specialisationWeight;
                        if (u != swappable)
                        {
                            futureAllocation[1] += specialisationWeight;
                        }
                    }
                }
            }
            //What would the future state look like if this agent swapped?
            if (hypo == unitState.basic)
            {
                for (int i = 0; i < currentAllocation.Length; i++)
                {
                   futureAllocation[i] += 1;
                }
            }
            else if (hypo == unitState.paladin || hypo == unitState.investigator)
            {
                futureAllocation[0] += specialisationWeight;
            }
            else if (hypo == unitState.knight)
            {
                futureAllocation[1] += specialisationWeight;
            }

            double normA = 0;
            double normB = 0;
            double normC = 0;
            for (int i = 0; i < currentAllocation.Length; i++)
            {
                normA += currentAllocation[i];
                normB += weights[i];
                normC += futureAllocation[i];
            }
            for (int i = 0; i < currentAllocation.Length; i++)
            {
                if (normA != 0) { currentAllocation[i] /= normA; }
                if (normB != 0) { weights[i] /= normB; }
                if (normC != 0) { futureAllocation[i] /= normC; }
            }

            //We now want to ask if swapping gets us closer to ideal distribution
            //Squared Euclidean metric
            double presentDistance = 0;
            double futureDistance = 0;
            //string msgCur = "";
            //string msgFuture = "";
            //string msgWeights = "";
            for (int i = 0; i < futureAllocation.Length; i++)
            {
                //msgFuture += Eleven.toFixedLen(futureAllocation[i], 4) + " ";
                //msgCur += Eleven.toFixedLen(currentAllocation[i], 4) + " ";
                //msgWeights += Eleven.toFixedLen(weights[i], 4) + " ";
                //World.log("Swap hypothesis " + i + " " + currentAllocation[i] + " " + futureAllocation[i]);
                presentDistance += (currentAllocation[i] - weights[i]) * (currentAllocation[i] - weights[i]);
                futureDistance += (futureAllocation[i] - weights[i]) * (futureAllocation[i] - weights[i]);
            }
            //World.log("Threat weights: " + msgWeights);
            //World.log("Hypotheatical : " + msgFuture + " dist " + Eleven.toFixedLen(futureDistance, 5));
            //World.log("Current       : " + msgCur + " dist " + Eleven.toFixedLen(presentDistance,5));

            return presentDistance - futureDistance;//We want to minimise the distance. If swapping moves us closer then futureDist is smaller than present dist, so this becomes positive
        }

        public override Sprite getSprite(World world)
        {
            if (state == unitState.basic)
            {
                return world.textureStore.unit_default;
            }
            if (state == unitState.investigator)
            {
                return world.textureStore.unit_lookingGlass;
            }
            if (state == unitState.paladin)
            {
                return world.textureStore.unit_paladin;
            }
            if (state == unitState.knight)
            {
                return world.textureStore.unit_knight;
            }
            return world.textureStore.unit_lookingGlass;
        }

        public override string getTitleM()
        {
            if (state == unitState.basic)
            {
                return "Agent";
            }
            if (state == unitState.investigator)
            {
                return "Investigator";
            }
            if (state == unitState.paladin)
            {
                return "Paladin";
            }
            if (state == unitState.knight)
            {
                return "Sir";
            }
            return "Investigator";
        }

        public override string getTitleF()
        {
            if (state == unitState.basic)
            {
                return "Agent";
            }
            if (state == unitState.investigator)
            {
                return "Investigator";
            }
            if (state == unitState.paladin)
            {
                return "Paladin";
            }
            if (state == unitState.knight)
            {
                return "Sir";
            }
            return "Investigator";
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return base.specialInfoColour();
        }

        public override string specialInfo()
        {
            if (victim == null)
            {
                return "No False Accusation Target";
            }
            else
            {
                return "Able to Accuse " + victim.getName() + "( " + victimUses + " uses)";
            }
        }

        public override string specialInfoLong()
        {
            return "Player-owned investigators can accuse another agent of being in league with the darkness. This will possibly get them condemned by " +
                "the society, and can reduce the risk of their own accusations being believed (to prevent an investigator with evidence against you being effective)";
        }

        public override string getDesc()
        {
            if (state == unitState.investigator)
            {
                return "Investigators are agents who wander near their home location searching for evidence of dark powers. They can analyse evidence and recognise both enthralled agents and enthralled nobles.";
            }
            if (state == unitState.paladin)
            {
                return "Paladins are promoted investigators, who are sent to hunt down the person they have evidence against.";
            }
            return "Investigators are agents who wander near their home location searching for evidence of dark powers. They can analyse evidence and recognise both enthralled agents and enthralled nobles.";
        }
    }
}
