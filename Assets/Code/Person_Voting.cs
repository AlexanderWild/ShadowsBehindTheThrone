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

        public void logVote(VoteIssue issue)
        {
            if (World.logging)
            {
                string line = "  " + issue.ToString() + " for soc " + issue.society.getName();
                log.takeLine(line);
                foreach (VoteOption opt in issue.options)
                {
                    line = "     " + opt.fixedLenInfo();
                    line += " U " + Eleven.toFixedLen(issue.computeUtility(this, opt, new List<ReasonMsg>()),12);
                    log.takeLine(line);
                }
            }
        }

        public VoteIssue proposeVotingIssue()
        {
            //Note we start at 1 utility. This means no guaranteed negative/near-zero utility options will be proposed
            double bestU = 1;
            VoteIssue bestIssue = null;

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

            //Unlanded titles can be distributed
            //Assignment of sovreign takes priority over any other voting, in the minds of the lords and ladies
            foreach (Title t in society.titles)
            {
                if (t.heldBy != null && (map.turn - t.turnLastAssigned < map.param.society_minTimeBetweenTitleReassignments)) { continue; }
                issue = new VoteIssue_AssignTitle(society, this, t);

                //Everyone is eligible
                foreach (Person p in society.people)
                {
                    VoteOption opt = new VoteOption();
                    opt.person = p;
                    issue.options.Add(opt);
                }
                foreach (VoteOption opt in issue.options)
                {
                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU || (t == society.sovreign && t.heldBy == null))//Note we force them to vote on a sovereign if there is none
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }
                logVote(issue);
            }

            if (society.getSovreign() != null)
            {
                foreach (Location loc in map.locations)
                {
                    //If there are unhanded out titles, only consider those. Else, check all.
                    //Maybe they could be rearranged (handed out or simply swapped) in a way which could benefit you
                    //if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && ((!existFreeTitles) || (loc.settlement.title.heldBy == null)))

                    //We're now stopping them suggesting this on places with existing nobles, as that lead to undue amounts of swapping
                    if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && (loc.settlement.title.heldBy == null))
                    {
                        //if (map.turn - loc.turnLastAssigned  < Params.society_minTimeBetweenLocReassignments) { continue; }
                        issue = new VoteIssue_AssignLandedTitle(society, this, loc.settlement.title);
                       // if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                                                                                                                   //Everyone is eligible
                        foreach (Person p in society.people)
                        {
                            if (p.title_land != null) { continue; }//Again, to prevent constant shuffling
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
                            }
                        }
                        logVote(issue);
                    }
                }

                if (!existFreeTitles)
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
                                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
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
                    issue = new VoteIssue_SetOffensiveTarget(society, this);
                    foreach (SocialGroup neighbour in map.getExtendedNeighbours(society))
                    {
                        VoteOption option = new VoteOption();
                        option.group = neighbour;
                        issue.options.Add(option);
                    }
                    foreach (VoteOption opt in issue.options)
                    {
                        //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                    }
                    logVote(issue);

                      //Check to see if you want to alter defensive military targetting
                        issue = new VoteIssue_SetDefensiveTarget(society, this);
                    foreach (ThreatItem item in threatEvaluations)
                    {
                        if (item.group == null) { continue; }
                        VoteOption option = new VoteOption();
                        option.group = item.group;
                        issue.options.Add(option);
                    }
                    foreach (VoteOption opt in issue.options)
                    {
                        //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                    }
                    logVote(issue);

                      //Change military posture, to either improve defence, fix internal problems, or attack an enemy
                        issue = new VoteIssue_MilitaryStance(society, this);
                    for (int i = 0; i < 3; i++)
                    {
                        VoteOption opt = new VoteOption();
                        opt.index = i;
                        issue.options.Add(opt);
                    }
                    foreach (VoteOption opt in issue.options)
                    {
                        //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                    }
                    logVote(issue);

                    //if (map.turn < 100)
                    if (true)
                    { //ANWSAVE
                      //Check to see if you want to declare war
                      //You need to be in offensive posture to be allowed to do so
                        if (society.offensiveTarget != null && society.posture == Society.militaryPosture.offensive && society.getRel(society.offensiveTarget).state != DipRel.dipState.war)
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
                            double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                            logVote(issue);
                        }
                    }

                    if (true) { //ANWSAVE

                        //Check to see if you want to defensively vassalise yourself
                        //You need to be in defensive posture to be allowed to do so
                        if (society.offensiveTarget != null && society.posture == Society.militaryPosture.defensive && (society.isAtWar() == false))
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
