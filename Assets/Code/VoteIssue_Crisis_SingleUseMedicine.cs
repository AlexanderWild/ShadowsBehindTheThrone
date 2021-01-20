using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_Crisis_SingleUseMedicine : VoteIssue
    {
        public Title title;
        public int lastAssigned = 0;

        public VoteIssue_Crisis_SingleUseMedicine(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Cure a single noble's disease";
        }

        public override string getLargeDesc()
        {
            string reply = "Cure a single settlement of disease." +
                "\nWe have been given a miracle cure, which can remove all disease from a single settlement. But which to choose? Who to save?";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            Person p = option.person;


            double benefitU = voter.getRelation(p).getLiking() * voter.map.param.utility_singleUseCure;
            msgs.Add(new ReasonMsg("Benefit to " + p.getFullName(), benefitU));
            u += benefitU;

            bool hasDisease = false;
            foreach (Property pr in voter.getLocation().properties)
            {
                if (pr.proto.isDisease)
                {
                    hasDisease = true;
                    break;
                }
            }
            if (hasDisease && p != voter)
            {
                //We're considering giving this cure to another

                //This option doesn't help me. If I'm corrupt, I'll naturally be angry at not being given help
                double localU = voter.getSelfInterest() * voter.threat_plague.threat * World.staticMap.param.utility_selfInterestFromThreat * 1.5;
                if (localU != 0)
                {
                    msgs.Add(new ReasonMsg("Does not help me personally", localU));
                    u += localU;
                }
            }

            return u;
        }

        public override void implement(VoteOption option)
        {
            if (society.people.Contains(option.person) == false) { World.log("Invalid option. Person cannot hold title."); return; }
            base.implement(option);

            List<Property> rems = new List<Property>();
            foreach (Property pr in option.person.getLocation().properties)
            {
                if (pr.proto.isDisease)
                {
                    rems.Add(pr);
                }
            }
            foreach (Property pr in rems)
            {
                pr.proto.endProperty(option.person.getLocation(), pr);
                option.person.getLocation().properties.Remove(pr);
            }

        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
