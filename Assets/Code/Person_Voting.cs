using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public partial class Person
    {
        public VoteSession forcedVoteSession;
        public VoteOption forcedVoteOption;
        public int electoralID = 0;
        public double electoralWeight;//How much of the vote we're winning
        public bool electoralWinner = false;
        public Person electoralRecipient = null;

        public void logVote(VoteIssue issue)
        {
            if (World.logging)
            {
                string line = "  " + issue.ToString() + " for soc " + issue.society.getName();
                log.takeLine(line);
                foreach (VoteOption opt in issue.options)
                {
                    line = "     " + opt.fixedLenInfo();
                    line += " U " + Eleven.toFixedLen(issue.computeUtility(this, opt, new List<ReasonMsg>()), 12);
                    log.takeLine(line);
                }
            }
        }

        public VoteIssue proposeVotingIssue()
        {
            double bestU = 25;
            VoteIssue bestIssue = null;
            bool forcedBest = false;
            bool existFreeTitles = false;

            if (World.logging) { log.takeLine("Proposing vote on turn " + map.turn); }


            foreach (Location loc in map.locations)
            {
                if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && loc.settlement.title.heldBy == null)
                {
                    existFreeTitles = true;
                }
            }


            VoteIssue issue;


            //Check for evidence submission crisis
            issue = checkForCrises();
            if (issue != null) { return issue; }

            //Unlanded titles can be distributed
            //Assignment of sovreign takes priority over any other voting except crises, in the minds of the lords and ladies
            foreach (Title t in society.titles)
            {
                //Can assign an unassigned title, or hold routine elections
                bool canHold = t.heldBy == null || (map.turn - t.turnLastAssigned >= map.param.society_minTimeBetweenTitleReassignments);
                //You can hold emergency elections in the event of upcoming civil war
                //if (society.data_societalStability < 0 && t == society.sovreign) { canHold = true; }
                
                //Can't hold elections if your society doesn't allow it
                if (t.heldBy != null && (!society.socType.periodicElection()))
                {
                    continue;
                }

                if (!canHold) { continue; }

                issue = new VoteIssue_AssignTitle(society, this, t);

                if (t is Title_Sovreign)
                {
                    List<Person> candidates = t.getEligibleHolders(society);
                    foreach (Person p in candidates)
                    {
                        VoteOption opt = new VoteOption();
                        opt.person = p;
                        issue.options.Add(opt);
                    }
                }else if (t is Title_ProvinceRuler)
                {
                    List<Person> candidates = t.getEligibleHolders(society);
                    foreach (Person p in candidates)
                    {
                        VoteOption opt = new VoteOption();
                        opt.person = p;
                        issue.options.Add(opt);
                    }
                }
                if (issue.options.Count == 0) { continue; }
                foreach (VoteOption opt in issue.options)
                {
                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU || (t == society.sovreign && t.heldBy == null))//Note we force them to vote on a sovereign if there is none
                    {
                        bestU = localU;
                        bestIssue = issue;
                        forcedBest = true;
                    }
                }
                logVote(issue);
            }

            if (society.getSovreign() != null)
            {
                int oldestAssignment = 100000000;
                Location bestAssignable = null;
                /*
                if (society.billsSinceLastSettlementAssignment > map.param.society_billsBetweenLandAssignments)
                {
                    foreach (Location loc in map.locations)
                    {
                        if (loc.soc == this.society && loc.settlement != null && loc.settlement.title != null)
                        {
                            if (loc.settlement.lastAssigned < oldestAssignment)
                            {
                                oldestAssignment = loc.settlement.lastAssigned;
                                bestAssignable = loc;
                            }
                        }
                    }
                }
                */

                foreach (Location loc in map.locations)
                {
                    //If there are unhanded out titles, only consider those. Else, check all.
                    //Maybe they could be rearranged (handed out or simply swapped) in a way which could benefit you
                    //if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && ((!existFreeTitles) || (loc.settlement.title.heldBy == null)))

                    //We're now stopping them suggesting this on places with existing nobles, as that lead to undue amounts of swapping
                    //  *now ammended to allow a single swap every N bills, AND it must be the last swapped one
                    if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && (loc.settlement.title.heldBy == null || loc == bestAssignable))
                    {
                        //if (map.turn - loc.turnLastAssigned  < Params.society_minTimeBetweenLocReassignments) { continue; }
                        issue = new VoteIssue_AssignLandedTitle(society, this, loc.settlement.title);
                        // if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                        foreach (Person p in society.people)
                        {
                            if (p.title_land != loc.settlement.title && p.title_land != null) { continue; }//Again, to prevent constant shuffling
                            VoteOption opt = new VoteOption();
                            opt.person = p;
                            issue.options.Add(opt);
                        }

                        foreach (VoteOption opt in issue.options)
                        {
                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                                forcedBest = true;
                            }
                        }
                        logVote(issue);
                    }
                }

                if (society.needsToDecreasePopulation)
                {
                    issue = new VoteIssue_DismissFromCourt(society, this);
                    foreach (Person p in society.people)
                    {
                        if (p.title_land == null)
                        {
                            VoteOption opt = new VoteOption();
                            opt.person = p;
                            issue.options.Add(opt);
                        }
                    }
                    foreach (VoteOption opt in issue.options)
                    {
                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                            forcedBest = true;
                        }
                    }
                }

                if (!forcedBest)
                {
                    //Check to see if you want to economically rebalance the economy
                    if (this.title_land != null)
                    {
                        HashSet<EconTrait> mine = new HashSet<EconTrait>();
                        HashSet<EconTrait> all = new HashSet<EconTrait>();


                        foreach (EconTrait trait in title_land.settlement.econTraits())
                        {
                            mine.Add(trait);
                        }
                        foreach (Location loc in map.locations)
                        {
                            if (loc.soc == society && loc.settlement != null)
                            {
                                foreach (EconTrait trait in loc.settlement.econTraits())
                                {
                                    all.Add(trait);
                                }
                            }
                        }

                        foreach (EconTrait econ_from in all)
                        {
                            if (mine.Contains(econ_from)) { continue; }//Don't take from yourself
                            foreach (EconTrait econ_to in mine)
                            {
                                issue = new VoteIssue_EconomicRebalancing(society, this, econ_from, econ_to);
                                //Allow them to spam econ votes
                                //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                                bool present = false;
                                foreach (EconEffect effect in society.econEffects)
                                {
                                    if (effect.from == econ_from && effect.to == econ_to) { present = true; }
                                    if (effect.to == econ_from && effect.from == econ_to) { present = true; }
                                }
                                if (present) { continue; }

                                //We have our two options (one way or the other)
                                VoteOption opt1 = new VoteOption();
                                opt1.econ_from = econ_from;
                                opt1.econ_to = econ_to;
                                issue.options.Add(opt1);
                                VoteOption opt2 = new VoteOption();
                                opt2.econ_from = econ_to;
                                opt2.econ_to = econ_from;
                                issue.options.Add(opt2);

                                foreach (VoteOption opt in issue.options)
                                {
                                    //Random factor to prevent them all rushing a singular voting choice
                                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble() * map.param.society_votingEconHiddenBiasMult;
                                    if (localU > bestU)
                                    {
                                        bestU = localU;
                                        bestIssue = issue;
                                    }
                                }

                                logVote(issue);
                            }
                        }
                    }

                    //Check to see if you want to alter offensive military targetting
                    if (map.turn - society.lastOffensiveTargetSetting > 8)
                    {
                        issue = new VoteIssue_SetOffensiveTarget(society, this);
                        foreach (SocialGroup neighbour in map.socialGroups)
                        {
                            if (neighbour == this.society) { continue; }//Don't declare war on yourself

                            VoteOption option = new VoteOption();
                            option.group = neighbour;
                            issue.options.Add(option);
                        }
                        double localBest = 0;
                        VoteOption voteOpt = null;
                        foreach (VoteOption opt in issue.options)
                        {
                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > localBest)
                            {
                                localBest = localU;
                                voteOpt = opt;
                            }
                        }
                        if (voteOpt != null && voteOpt.group != society.offensiveTarget && localBest > bestU)
                        {
                            bestU = localBest;
                            bestIssue = issue;
                        }
                        logVote(issue);
                    }

                    {
                        //Check to see if you want to alter defensive military targetting
                        issue = new VoteIssue_SetDefensiveTarget(society, this);
                        foreach (ThreatItem item in threatEvaluations)
                        {
                            if (item.group == null) { continue; }
                            VoteOption option = new VoteOption();
                            option.group = item.group;
                            issue.options.Add(option);
                        }
                        double localBest = 0;
                        VoteOption voteOpt = null;
                        foreach (VoteOption opt in issue.options)
                        {
                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > localBest)
                            {
                                localBest = localU;
                                voteOpt = opt;
                            }
                        }
                        if (voteOpt != null && voteOpt.group != society.defensiveTarget && localBest > bestU)
                        {
                            bestU = localBest;
                            bestIssue = issue;
                        }
                        logVote(issue);
                    }

                    {
                        //Change military posture, to either improve defence, fix internal problems, or attack an enemy
                        issue = new VoteIssue_MilitaryStance(society, this);
                        for (int i = 0; i < 3; i++)
                        {
                            VoteOption opt = new VoteOption();
                            opt.index = i;
                            issue.options.Add(opt);
                        }
                        double localBest = 0;
                        VoteOption voteOpt = null;
                        foreach (VoteOption opt in issue.options)
                        {
                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > localBest)
                            {
                                localBest = localU;
                                voteOpt = opt;
                            }
                        }
                        int ourIndex = 0;
                        if (society.posture == Society.militaryPosture.defensive) { ourIndex = 0; }
                        if (society.posture == Society.militaryPosture.offensive) { ourIndex = 1; }
                        if (society.posture == Society.militaryPosture.introverted) { ourIndex = 2; }
                        if (voteOpt != null && voteOpt.index != ourIndex && localBest > bestU)
                        {
                            bestU = localBest;
                            bestIssue = issue;
                        }
                        logVote(issue);
                    }

                    //Check to see if you want to declare war
                    //You need to be in offensive posture to be allowed to do so
                    if (society.offensiveTarget != null && society.posture == Society.militaryPosture.offensive && society.getRel(society.offensiveTarget).state != DipRel.dipState.war)
                    {
                        if (map.burnInComplete == true || map.socialGroups.Count > 4)
                        {

                            issue = new VoteIssue_DeclareWar(society, society.offensiveTarget, this);
                            VoteOption option_0 = new VoteOption();
                            option_0.index = 0;
                            issue.options.Add(option_0);

                            VoteOption option_1 = new VoteOption();
                            option_1.index = 1;
                            issue.options.Add(option_1);

                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                            //Random factor to prevent them all rushing a singular voting choice
                            double uWar = issue.computeUtility(this, option_1, new List<ReasonMsg>());
                            double uPeace = issue.computeUtility(this, option_0, new List<ReasonMsg>());
                            double localU = (uWar - uPeace) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                            logVote(issue);
                        }
                    }

                    //Check to see if you want to defensively vassalise yourself
                    //You need to be in defensive posture to be allowed to do so
                    if (society.offensiveTarget != null && society.posture == Society.militaryPosture.defensive && (society.isAtWar() == false) && society.lastTurnLocs.Count < World.staticMap.param.voting_maxLocationsForVassalisation)
                    {
                        foreach (SocialGroup sg in society.getNeighbours())
                        {
                            if (sg is Society == false) { continue; }
                            if (sg == this.society) { continue; }
                            Society other = (Society)sg;
                            if (other.defensiveTarget == this.society.defensiveTarget)
                            {
                                issue = new VoteIssue_Vassalise(society, other, this);
                                VoteOption option_0 = new VoteOption();
                                option_0.index = 0;
                                issue.options.Add(option_0);

                                VoteOption option_1 = new VoteOption();
                                option_1.index = 1;
                                issue.options.Add(option_1);

                                //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                                //Random factor to prevent them all rushing a singular voting choice
                                double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                                if (localU > bestU)
                                {
                                    bestU = localU;
                                    bestIssue = issue;
                                }
                                logVote(issue);
                            }
                        }
                    }
                    if (map.param.useAwareness == 1 && map.worldPanic >= map.param.panic_canAlly && this.awareness >= map.param.awareness_canProposeLightAlliance)
                    {
                        issue = new VoteIssue_LightAlliance(society, null, this);
                        VoteOption option_0 = new VoteOption();
                        option_0.group = null;
                        issue.options.Add(option_0);

                        foreach (SocialGroup sg in society.getNeighbours())
                        {
                            if (sg is Society == false) { continue; }
                            if (sg == this.society) { continue; }
                            Society other = (Society)sg;
                            if (other.isDarkEmpire) { continue; }

                            VoteOption option_1 = new VoteOption();
                            option_1.group = sg;
                            issue.options.Add(option_1);

                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                            logVote(issue);
                        }
                    }

                    //Check if you want to execute someone
                    if (society.posture == Society.militaryPosture.introverted)
                    {
                        foreach (Person p in society.people)
                        {
                            if (p == this) { continue; }
                            issue = new VoteIssue_JudgeSuspect(society, p, this);
                            VoteOption option_0 = new VoteOption();
                            option_0.index = 0;
                            issue.options.Add(option_0);

                            VoteOption option_1 = new VoteOption();
                            option_1.index = 1;
                            issue.options.Add(option_1);

                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                            logVote(issue);
                        }
                    }
                    foreach (Unit u in map.units)
                    {
                        if (u.person == null) { continue; }
                        if (getRelation(u.person).getLiking() >= 0) { continue; }
                        if (society.enemies.Contains(u)) { continue; }
                        issue = new VoteIssue_CondemnAgent(society, u, this);
                        VoteOption option_0 = new VoteOption();
                        option_0.index = 0;
                        issue.options.Add(option_0);

                        VoteOption option_1 = new VoteOption();
                        option_1.index = 1;
                        issue.options.Add(option_1);

                        //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                        logVote(issue);
                    }
                }

            }

            if (bestIssue != null)
            {
                if (World.logging)
                {
                    log.takeLine("CHOSE: " + bestIssue.ToString());
                }
            }
            //lastProposedIssue = bestIssue;
            return bestIssue;
        }

        public VoteOption getVote(VoteSession voteSession)
        {
            if (World.logging) { this.log.takeLine("Voting on " + voteSession.issue); }
            double highestWeight = 0;
            VoteOption bestChoice = null;
            foreach (VoteOption option in voteSession.issue.options)
            {
                List<ReasonMsg> msgs = new List<ReasonMsg>();
                double u = voteSession.issue.computeUtility(this, option, msgs);

                if (forcedVoteSession == voteSession && option == forcedVoteOption)
                {
                    ReasonMsg msg = new ReasonMsg("Obligated to vote for this option", 0);
                    msgs.Add(msg);
                }
                if (u > highestWeight || bestChoice == null)
                {
                    bestChoice = option;
                    highestWeight = u;
                }
                if (World.logging)
                {
                    log.takeLine(" " + option.fixedLenInfo() + "  " + u);
                    foreach (ReasonMsg msg in msgs)
                    {
                        log.takeLine("     " + Eleven.toFixedLen(msg.value, 5) + msg.msg);
                    }
                }
            }

            if (this.forcedVoteSession == voteSession)
            {
                return forcedVoteOption;
            }
            return bestChoice;
        }
    }

}
