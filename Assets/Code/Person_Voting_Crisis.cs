using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public partial class Person
    {
        private VoteIssue checkForCrises()
        {
            if (map.simplified) { return null; }

            if (society.crisisWitchHunt)
            {
                VoteIssue_WitchHunt issue = new VoteIssue_WitchHunt(society, this);

                VoteOption opt;

                foreach (Person p in society.people)
                {
                    opt = new VoteOption();
                    opt.person = p;
                    issue.options.Add(opt);
                }
                GraphicalMap.panTo(this.getLocation().hex.x, this.getLocation().hex.y);

                return issue;
            }

            VoteIssue replyIssue = null;
            int c = 0;

            if (society.lastEvidenceSubmission > society.lastEvidenceResponse)
            {
                List<Evidence> unprocessedEvidence = new List<Evidence>();
                foreach (Evidence ev in society.evidenceSubmitted)
                {
                    if (society.handledEvidence.Contains(ev)) { continue; }
                    unprocessedEvidence.Add(ev);
                }


                VoteIssue_Crisis_EvidenceDiscovered issue = new VoteIssue_Crisis_EvidenceDiscovered(society, this, unprocessedEvidence);

                //Every province (even those unaffected) can have its security increased (egotism influenced)
                HashSet<Province> provinces = new HashSet<Province>();
                HashSet<Province> securedProvinces = new HashSet<Province>();
                bool hasRaisedSecurity = false;
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this.society) {
                        provinces.Add(loc.province);

                        foreach (Property pr in loc.properties)
                        {
                            if (pr.proto is Pr_MajorSecurityBoost || pr.proto is Pr_MinorSecurityBoost || pr.proto is Pr_Lockdown)
                            {
                                hasRaisedSecurity = true;
                                securedProvinces.Add(loc.province);
                            }
                        }
                    }
                }
                VoteOption opt;

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_EvidenceDiscovered.NO_RESPONSE;
                issue.options.Add(opt);

                bool existEnemies = false;
                foreach (Unit u in map.units)
                {
                    if (society.enemies.Contains(u) == false && u.society != society)
                    {
                        existEnemies = true;
                    }
                }
                if (existEnemies)
                {
                    opt = new VoteOption();
                    opt.index = VoteIssue_Crisis_EvidenceDiscovered.EXPELL_ALL_FOREIGN_AGENTS;
                    issue.options.Add(opt);
                }

                if (!hasRaisedSecurity)
                {
                    opt = new VoteOption();
                    opt.index = VoteIssue_Crisis_EvidenceDiscovered.NATIONWIDE_SECURITY;
                    issue.options.Add(opt);

                }

                foreach (Province prv in provinces)
                {
                    if (securedProvinces.Contains(prv) == false)
                    {
                        opt = new VoteOption();
                        opt.province = prv.index;
                        opt.index = VoteIssue_Crisis_EvidenceDiscovered.DEFEND_PROVINCE;
                        issue.options.Add(opt);

                        opt = new VoteOption();
                        opt.province = prv.index;
                        opt.index = VoteIssue_Crisis_EvidenceDiscovered.LOCKDOWN_PROVINCE;
                        issue.options.Add(opt);
                    }
                }
                foreach (Unit unit in map.units)
                {
                    if (unit is Unit_Investigator && unit.society == society)
                    {
                        Unit_Investigator inv = (Unit_Investigator)unit;
                        if (inv.canPromote() == false) { continue; }
                        if (inv.state == Unit_Investigator.unitState.investigator || inv.state == Unit_Investigator.unitState.paladin)
                        {
                            //Already in offensive anti-agent state, no need to intefere
                        }
                        else if  (inv.state == Unit_Investigator.unitState.basic)//Could upgrade to investigator if they've got a target
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.investigator;
                            opt.index = VoteIssue_Crisis_EvidenceDiscovered.AGENT_TO_INVESTIGATOR;
                            issue.options.Add(opt);
                        }
                        else
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.basic;
                            opt.index = VoteIssue_Crisis_EvidenceDiscovered.AGENT_TO_BASIC;
                            issue.options.Add(opt);

                        }
                    }
                }



                //bool hostilityPossible = false;
                //foreach (Evidence ev in unprocessedEvidence)
                //{
                //    World.log("Evidence. Discovered by " + ev.discoveredBy.getName() + " points to " + ev.pointsTo.getName() + " discSoc " + ev.discoveredBy.society.getName() + " us " + society.getName());
                //    if (ev.discoveredBy != null && ev.pointsTo != null && ev.discoveredBy.society == society)
                //    {
                //        if (ev.discoveredBy.hostileTo(ev.pointsTo)) { continue; }
                //        hostilityPossible = true;
                //    }
                //}

                //Decide if this is the crisis you're gonna deal with right now
                c += 1;
                if (Eleven.random.Next(c) == 0)
                {
                    replyIssue = issue;
                }
            }


            bool nobleCrisis = society.crisisNobles;
            if (
                this.awareness >= map.param.awarenessCanCallNobleCrisis 
                && map.turn - society.lastNobleCrisis >= map.param.awarenessMinNobleCrisisPeriod 
                && map.worldPanic >= map.param.panic_canCallNobleCrisis
                && this.state != personState.broken
                && this.state != personState.enthralled)
            {
                foreach (Person p in society.people)
                {
                    if (p == this) { continue; }
                    if (this.getRelation(p.index).suspicion > 0.5)
                    {
                        nobleCrisis = true;
                    }
                }
            }
            if (nobleCrisis)
            {
                List<Evidence> unprocessedEvidence = new List<Evidence>();
                foreach (Evidence ev in society.evidenceSubmitted)
                {
                    if (society.handledEvidence.Contains(ev)) { continue; }
                    unprocessedEvidence.Add(ev);
                }


                VoteIssue_Crisis_EnshadowedNobles issue = new VoteIssue_Crisis_EnshadowedNobles(society, this, unprocessedEvidence);

                VoteOption opt;

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_EnshadowedNobles.NO_RESPONSE;
                issue.options.Add(opt);

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_EnshadowedNobles.WITCH_HUNT;
                issue.options.Add(opt);

                foreach (Unit unit in map.units)
                {
                    if (unit is Unit_Investigator && unit.society == society)
                    {
                        Unit_Investigator inv = (Unit_Investigator)unit;
                        if (inv.canPromote() == false) { continue; }
                        if (inv.state == Unit_Investigator.unitState.investigator || inv.state == Unit_Investigator.unitState.paladin)
                        {
                            //Already in offensive anti-agent state, no need to intefere
                        }
                        else if (inv.state == Unit_Investigator.unitState.basic)//Could upgrade to investigator if they've got a target
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.investigator;
                            opt.index = VoteIssue_Crisis_EnshadowedNobles.AGENT_TO_INQUISITOR;
                            issue.options.Add(opt);
                        }
                        else
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.basic;
                            opt.index = VoteIssue_Crisis_EnshadowedNobles.AGENT_TO_BASIC;
                            issue.options.Add(opt);

                        }
                    }
                }

                c += 1;
                if (Eleven.random.Next(c) == 0)
                {
                    replyIssue = issue;
                }
            }
            if (society.crisisWarShort != null)
            {
                VoteIssue_Crisis_WarThreatens issue = new VoteIssue_Crisis_WarThreatens(society, this, society.crisisWarShort, society.crisisWarLong);
                VoteOption opt;

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_WarThreatens.NO_RESPONSE;
                issue.options.Add(opt);

                foreach (Unit unit in map.units)
                {
                    if (unit is Unit_Investigator && unit.society == society)
                    {
                        Unit_Investigator inv = (Unit_Investigator)unit;
                        if (inv.canPromote() == false) { continue; }
                        if (inv.state == Unit_Investigator.unitState.knight || inv.state == Unit_Investigator.unitState.paladin)
                        {
                            //Already doing something useful, ignore these
                        }
                        else if (inv.state == Unit_Investigator.unitState.basic)//Could upgrade to knight
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.knight;
                            opt.index = VoteIssue_Crisis_WarThreatens.AGENT_TO_KNIGHT;
                            issue.options.Add(opt);
                        }
                        else
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.basic;
                            opt.index = VoteIssue_Crisis_WarThreatens.AGENT_TO_BASIC;
                            issue.options.Add(opt);

                        }
                    }
                }


                //Decide if this is the crisis you're gonna deal with right now
                if (issue.options.Count > 1)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        replyIssue = issue;
                    }
                }

            }

            if (society.crisisPlague != null)
            {
                VoteIssue_Crisis_Plague issue = new VoteIssue_Crisis_Plague(society, this, society.crisisPlague, society.crisisPlagueLong);
                VoteOption opt;

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_Plague.NO_RESPONSE;
                issue.options.Add(opt);

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_Plague.QUARANTINE;
                issue.options.Add(opt);

                opt = new VoteOption();
                opt.index = VoteIssue_Crisis_Plague.TREATMENT;
                issue.options.Add(opt);

                foreach (Unit unit in map.units)
                {
                    if (unit is Unit_Investigator && unit.society == society)
                    {
                        Unit_Investigator inv = (Unit_Investigator)unit;
                        if (inv.canPromote() == false) { continue; }
                        if (inv.state == Unit_Investigator.unitState.medic || inv.state == Unit_Investigator.unitState.paladin)
                        {
                            //Already doing something useful, ignore these
                        }
                        else if (inv.state == Unit_Investigator.unitState.basic)//Could upgrade to medic
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.medic;
                            opt.index = VoteIssue_Crisis_Plague.AGENT_TO_MEDIC;
                            issue.options.Add(opt);
                        }
                        else
                        {
                            opt = new VoteOption();
                            opt.unit = unit;
                            opt.agentRole = Unit_Investigator.unitState.basic;
                            opt.index = VoteIssue_Crisis_Plague.AGENT_TO_BASIC;
                            issue.options.Add(opt);

                        }
                    }
                }


                //Decide if this is the crisis you're gonna deal with right now
                if (issue.options.Count > 1)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        replyIssue = issue;
                    }
                }

            }
            return replyIssue;
        }
    }

}
