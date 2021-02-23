﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_AssignLandedTitle : VoteIssue
    {
        public TitleLanded title;

        public VoteIssue_AssignLandedTitle(Society soc,Person proposer,TitleLanded title) : base(soc,proposer)
        {
            this.title = title;
        }

        public override string ToString()
        {
            string add = "";
            //if (title.heldBy != null)
            //{
            //    add = " (now held by " + title.heldBy.getFullName() + ")";
            //}
            //else
            //{
            //    add = " (unassigned)";
            //}
            return "Assign Title: " + title.getName();
        }

        public override string getLargeDesc()
        {
            string reply =  "Assign hold over " + this.title.settlement.location.getName() + ".";
            reply += "\nLand allows for greater prestige (so greater voting capability), levies of men-at-arms and industrial interests.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            Person p = option.person;
            double existingValue = 0;
            if (p.title_land != null)
            {
                existingValue = p.title_land.settlement.getPrestige();
            }

            double newValue = title.settlement.getPrestige();

            double benefitToPerson = newValue - existingValue;

            //We know how much they would be advantaged. We now need to know how much we like them to determine
            //if this is a good thing or not

            double localU = benefitToPerson * voter.getRelation(p).getLiking() * voter.map.param.utility_landedTitleMult;
            msgs.Add(new ReasonMsg("Benefit to " + p.getFullName(), localU));
            u += localU;

            if (title.heldBy == null)
            {
                localU = voter.map.param.utility_landedTitleAssignBaseline;
                msgs.Add(new ReasonMsg("Need to assign the title", localU));
                u += localU;
            }

            if (option.person != voter.society.getSovereign() && voter != option.person)
            {
                Person wouldBeSuperior = title.settlement.location.getSuperiorInSociety(voter.society);
                Person currentSuperior = option.person.getDirectSuperiorIfAny();
                if (currentSuperior == voter && wouldBeSuperior != voter)
                {
                    //Follower would be leaving
                    double desirabilityAsFollower = 0;
                    foreach (Trait t in option.person.traits)
                    {
                        desirabilityAsFollower += t.desirabilityAsFollower();
                    }
                    if (wouldBeSuperior == voter)
                    {
                        localU = -desirabilityAsFollower;
                        msgs.Add(new ReasonMsg("Desirability as follower", localU));
                        u += localU;
                    }
                }
                else if (currentSuperior != voter && wouldBeSuperior == voter)
                {
                    //Follower would be arriving
                    double desirabilityAsFollower = 0;
                    foreach (Trait t in option.person.traits)
                    {
                        desirabilityAsFollower += t.desirabilityAsFollower();
                    }
                    if (wouldBeSuperior == voter)
                    {
                        localU = desirabilityAsFollower;
                        msgs.Add(new ReasonMsg("Desirability as follower", localU));
                        u += localU;
                    }
                }
            }

            //We need to know if someone's going to lose out here
            //(Note this is irrelevant if they're the person who's being voted on)
            if (title.heldBy != null && title.heldBy != p)
            {
                double damageToOther = title.settlement.getPrestige();
                localU = -damageToOther * voter.getRelation(title.heldBy).getLiking() * voter.map.param.utility_landedTitleMult;
                msgs.Add(new ReasonMsg(title.heldBy.getFullName() + " would lose title", localU));
                u += localU;
            }
            

            return u;
        }

        public override void implement(VoteOption option)
        {
            if (society.people.Contains(option.person) == false) { World.log("Invalid option. Person cannot hold title."); return; }
            if (title.settlement == null) { return; }
            if (title.settlement.location.settlement != title.settlement) { return; }//It was removed before we arrived


            base.implement(option);
            if (title.heldBy == option.person)
            {
                World.log("Title: " + title.getName() + " remains held by " + option.person.getFullName());
            }


            //Person already has a title
            if (option.person.title_land != null)
            {
                TitleLanded prev = option.person.title_land;
                prev.heldBy = null;
                option.person.title_land = null;
                World.log(prev.getName() + " has lost its lord as they have been reassigned");
            }
            //Title already has a person
            if (title.heldBy != null)
            {
                World.log(title.heldBy.getFullName() + " is losing title " + title.getName());
                title.heldBy.title_land = null;
                title.heldBy = null;
            }

            World.log(option.person.getFullName() + " has been granted the title of " + title.getName());
            title.heldBy = option.person;
            title.settlement.infiltration = 0;
            option.person.title_land = title;
            if (title.settlement != null)
            {
                title.settlement.lastAssigned = title.settlement.location.map.turn;
            }
        }
        public override bool stillValid(Map map)
        {
            return title.settlement != null && title.settlement.location.soc == society;
        }
    }
}
