﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_MilitaryStance: VoteIssue
    {

        public VoteIssue_MilitaryStance(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Set Military Stance";
        }


        public override string getLargeDesc()
        {
            string reply = "Vote to change military stance.\nThree military stances exist: Offensive, Defensive and Introverted.";
            reply += "\nOffensive Stance allows for declaration of war against the offensive target a society has set.";
            reply += "\nDefensive Stance provides additional defence against a threat, should they declare war.";
            reply += "\nIntroverted Stance allows socities to defend against internal threats. It allows execution of nobles suspected of association with dark powers, and slightly reduces the risk of civil war.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);
            ThreatItem greatestThreat = voter.getGreatestThreat();


            double ourStr = 1 + (voter.society.currentMilitary + (voter.society.maxMilitary / 2));
            double offStr = 1;
            double defStr = 1;
            if (voter.society.offensiveTarget != null)
            {
                offStr = voter.society.offensiveTarget.currentMilitary + (voter.society.offensiveTarget.maxMilitary/2);
            }
            if (voter.society.defensiveTarget != null)
            {
                defStr = voter.society.defensiveTarget.currentMilitary + (voter.society.defensiveTarget.maxMilitary / 2);
            }

            double defUtility = 0;
            double defGreatestThreat = 0;
            if (voter.society.defensiveTarget != null)
            {
                //Negative if we are stronger than the one we fear
                defUtility += (defStr - ourStr) / (ourStr + defStr)*voter.map.param.utility_militaryTargetRelStrengthDefensive;

                if (defUtility < -50) { defUtility = -50; }

                if (greatestThreat != null)
                {
                    if (greatestThreat.group != null && greatestThreat.group.currentMilitary > voter.society.currentMilitary)
                    {
                        defGreatestThreat = World.staticMap.param.utility_greatestThreatDelta;
                        defUtility += defGreatestThreat;
                        if (option.index == 0)
                        {
                            msgs.Add(new ReasonMsg("Our greatest threat (" + greatestThreat.group.getName() + ") is stronger than us, we must defend", defGreatestThreat));
                        }
                    }
                }
            }
            double offUtility = 0;
            double offUtilityStr = 0;
            double offUtilityPersonality = 0;
            double offUtilityTerritory = 0;
            double offUtilityInstab = 0;
            double offGreatestThreat = 0;
            if (voter.society.offensiveTarget != null)
            {
                //Negative if the offensive target is stronger
                offUtilityStr += (ourStr - offStr) / (ourStr + offStr) * voter.map.param.utility_militaryTargetRelStrengthOffensive;
                if (voter.society.offensiveTarget is Society)
                {
                    offUtilityStr = Math.Min(offUtilityStr, voter.map.param.utility_militaryTargetRelStrengthOffensive);//Capped to prevent insanity snowballing
                }
                offUtilityPersonality += voter.getMilitarism() * voter.map.param.utility_militarism;
                
                //We want to expand into territory we already partially own
                bool hasOurTerritory = false;
                foreach (Location loc in voter.society.offensiveTarget.lastTurnLocs)
                {
                    if (loc.province == voter.getLocation().province)
                    {
                        hasOurTerritory = true;
                        break;
                    }
                }
                if (hasOurTerritory)
                {
                    offUtilityTerritory += society.map.param.utility_militaryTargetCompleteProvince*society.data_societalStability;
                }

                if (offUtilityStr > 0 && society.offensiveTarget is Society)
                {
                    //Counters expansion desire. Value always negative or zero
                    //u = off*stability
                    //u = off + (off*(stability-1))  (Because we are removing 1 from stability mult)
                    //If stab == 0 it's off - off
                    //If stab < 0 it's less than off
                    offUtilityInstab = offUtilityInstab * (society.data_societalStability-1);
                }

                if (greatestThreat != null)
                {
                    if (greatestThreat.group != null && greatestThreat.group.currentMilitary < voter.society.currentMilitary)
                    {
                        offGreatestThreat += World.staticMap.param.utility_greatestThreatDelta;
                        if (option.index == 1)
                        {
                            msgs.Add(new ReasonMsg("Our greatest threat (" + greatestThreat.group.getName() + ") is weaker than us, we must attack", offGreatestThreat));
                        }
                    }
                }
                offUtility += offGreatestThreat;
                offUtility += offUtilityStr;
                offUtility += offUtilityPersonality;
                offUtility += offUtilityTerritory;
                offUtility += offUtilityInstab;
            }
            double introUtility = 0;
            double introUtilityStability = 0;
            if (society.data_societalStability < 0.66)
            {
                introUtilityStability  = - (society.data_societalStability - 1);//0 if stability is 1, increasing to 1 if civil war is imminent, to 2 if every single person is a traitor
            }
            double introGreatestThreat = 0;
            if (greatestThreat != null)
            {
                if (greatestThreat.form == ThreatItem.formTypes.AGENTS || greatestThreat.form == ThreatItem.formTypes.ENSHADOWED_NOBLES)
                {
                    introGreatestThreat = World.staticMap.param.utility_greatestThreatDelta;
                    introUtility += introGreatestThreat;
                    if (option.index == 2)
                    {
                        msgs.Add(new ReasonMsg("Our greatest threat (" + greatestThreat.getTitle() + ") requires internal security", introGreatestThreat));
                    }
                }
            }
            introUtilityStability *= voter.map.param.utility_introversionFromInstability;
            double introFromInnerThreat = voter.threat_enshadowedNobles.threat*voter.map.param.utility_introversionFromSuspicion;
            introUtility += introUtilityStability;
            introUtility += introFromInnerThreat;
            introUtility += 10;

            //Option 0 is DEFENSIVE
            //Option 1 is OFFENSIVE
            //Option 2 is INTROSPECTIVE
            if (voter.society.posture == Society.militaryPosture.defensive && option.index != 0)
            {
                u -= defUtility;
                msgs.Add(new ReasonMsg("Switching away from defensive", -defUtility));
            }
            if (voter.society.posture == Society.militaryPosture.offensive && option.index != 1)
            {
                u -= offUtility;
                msgs.Add(new ReasonMsg("Switching away from offensive", -offUtility));
            }
            if (voter.society.posture == Society.militaryPosture.introverted && option.index != 2)
            {
                u -= introUtility;
                msgs.Add(new ReasonMsg("Switching away from introversion", -introUtility));
            }

            if (option.index == 0)
            {
                if (society.posture != Society.militaryPosture.defensive)
                {
                    u += defUtility;
                    msgs.Add(new ReasonMsg("Our relative strength against defensive target", defUtility));
                    if (voter.society.isDarkEmpire)
                    {
                        int value = (int)(-100 * voter.shadow);
                        u += value;
                        msgs.Add(new ReasonMsg("We must expand the Dark Empire", value));
                    }
                }
                else
                {
                    u += defUtility;
                    msgs.Add(new ReasonMsg("Our relative strength against defensive target", defUtility));
                    if (voter.society.isDarkEmpire)
                    {
                        int value = (int)(-100 * voter.shadow);
                        u += value;
                        msgs.Add(new ReasonMsg("We must expand the Dark Empire", value));
                    }
                    msgs.Add(new ReasonMsg("No need to change: It is our current stance", -u));
                    u = 0;
                }
            }
            if (option.index == 1)
            {
                if (society.posture != Society.militaryPosture.offensive)
                {
                    u += offUtility;
                    msgs.Add(new ReasonMsg("Our relative strength against offensive target", offUtilityStr));
                    msgs.Add(new ReasonMsg("Risk of instability from expansion", offUtilityInstab));
                    msgs.Add(new ReasonMsg("Militarism personality", offUtilityPersonality));
                    msgs.Add(new ReasonMsg("Desire for territory", offUtilityTerritory));
                    if (voter.society.isDarkEmpire)
                    {
                        int value = (int)(100 * voter.shadow);
                        u += value;
                        msgs.Add(new ReasonMsg("We must expand the Dark Empire", value));
                    }
                }
                else
                {
                    u += offUtility;
                    msgs.Add(new ReasonMsg("Our relative strength against offensive target", offUtilityStr));
                    msgs.Add(new ReasonMsg("Risk of instability from expansion", offUtilityInstab));
                    msgs.Add(new ReasonMsg("Militarism personality", offUtilityPersonality));
                    msgs.Add(new ReasonMsg("Desire for territory", offUtilityTerritory));
                    if (voter.society.isDarkEmpire)
                    {
                        int value = (int)(100 * voter.shadow);
                        u += value;
                        msgs.Add(new ReasonMsg("We must expand the Dark Empire", value));
                    }
                    msgs.Add(new ReasonMsg("No need to change: It is our current stance", -u));
                    u = 0;
                }
            }
            if (option.index == 2)
            {
                msgs.Add(new ReasonMsg("Default position", 10));
                if (society.posture != Society.militaryPosture.introverted)
                {
                    u += introUtility;
                    msgs.Add(new ReasonMsg("Instability internally", introUtilityStability));
                    msgs.Add(new ReasonMsg("Suspicion of nobles' darkness", introFromInnerThreat));
                    if (voter.society.isDarkEmpire)
                    {
                        int value = (int)(-100 * voter.shadow);
                        u += value;
                        msgs.Add(new ReasonMsg("We must expand the Dark Empire", value));
                    }
                }
                else
                {
                    u += introUtility;
                    msgs.Add(new ReasonMsg("Instability internally", introUtilityStability));
                    msgs.Add(new ReasonMsg("Suspicion of nobles' darkness", introFromInnerThreat));
                    if (voter.society.isDarkEmpire)
                    {
                        int value = (int)(-100 * voter.shadow);
                        u += value;
                        msgs.Add(new ReasonMsg("We must expand the Dark Empire", value));
                    }
                    msgs.Add(new ReasonMsg("No need to change: It is our current stance", -u));
                    u = 0;
                }
            }


            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 0)
            {
                society.posture = Society.militaryPosture.defensive;
            }
            else if (option.index == 1)
            {
                society.posture = Society.militaryPosture.offensive;
            }
            else if (option.index == 2)
            {
                society.posture = Society.militaryPosture.introverted;
            }
            society.cooldownLastMilitarySwitch = society.map.turn;
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
