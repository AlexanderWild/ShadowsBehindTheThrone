using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_LightAlliance : VoteIssue
    {

        public VoteIssue_LightAlliance(Society soc, Society target,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Ally against Dark";
        }


        public override string getLargeDesc()
        {
            string reply = "Vote to form alliance with a neighbour for protection against the dark.";
            reply += "\nIf passed, this motion will cause this society to be absorbed by another which is believed to be still safe from the shadow.";
            reply += "\nA last resort measure against a feared threat.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);
            double localU = 0;

            if (option.group == null)
            {
                //Null case is "remain independent"
                localU = -voter.map.param.utility_vassaliseReluctance;
                msgs.Add(new ReasonMsg("Inherent reluctance", localU));
                u += localU;

                return u;
            }
            
            localU = voter.map.param.utility_vassaliseReluctance;
            msgs.Add(new ReasonMsg("Inherent reluctance", localU));
            u += localU;

            Society socTarget = (Society)option.group;
            foreach (KillOrder order in socTarget.killOrders)
            {
                if (order.person == voter)
                {
                    localU = -1000;
                    msgs.Add(new ReasonMsg("Kill order against " + voter.getFullName(), localU));
                    u += localU;
                }
            }

            double awarenessU = society.map.param.utility_lightAllianceMult * voter.awareness;
            msgs.Add(new ReasonMsg("Awareness", awarenessU));
            u += awarenessU;


            double militaryU = option.group.currentMilitary - voter.society.currentMilitary;
            militaryU *= society.map.param.utility_lightAllianceMilMult*voter.awareness;
            msgs.Add(new ReasonMsg("Their military might", militaryU));
            u += militaryU;

            double totalSuspicion = 0;
            foreach (Person p in socTarget.people)
            {
                RelObj rel = voter.getRelation(p);
                totalSuspicion  += rel.suspicion;
            }

            double susU = -totalSuspicion;
            susU *= society.map.param.utility_lightAlliancSusMult;
            msgs.Add(new ReasonMsg("Suspicion of their nobles", susU));
            u += susU;

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.group != null)
            {
                Society receiver = (Society)option.group;
                bool canVassalise = false;
                List<Location> trans = new List<Location>();
                foreach (Location loc in society.map.locations)
                {
                    if (loc.soc == society)
                    {
                        trans.Add(loc);
                    }
                    if (loc.soc == receiver && loc.settlement != null)
                    {
                        canVassalise = true;
                    }
                }
                if (canVassalise)
                {
                    World.log(society.getName() + " ALLIES UNDER " + receiver.getName());
                    society.map.turnMessages.Add(new MsgEvent(society.getName() + " forms alliance under " + receiver.getName() + ", transferring all lands and landed nobles.",MsgEvent.LEVEL_RED,true,receiver.getCapitalHex()));
                    
                    foreach (Location loc in trans)
                    {
                        receiver.map.takeLocationFromOther(receiver, society, loc);
                    }

                    string message = society.getName() + " is allying itself under " + receiver.getName() + ", in response to their fears of the rising darkness. This alliance is made of nations "
                        + " believed to be safe from the shadow";
                    if (receiver.allianceName == null)
                    {
                        if (receiver.getCapital() != null)
                        {
                            string name = "";
                            int q = Eleven.random.Next(7);
                            if (q == 0){name = receiver.getCapital().shortName + " Alliance";}
                            if (q == 1){name = receiver.getCapital().shortName + " Concord"; }
                            if (q == 2) { name = receiver.getCapital().shortName + " Guardians"; }
                            if (q == 3) { name = "Alliance of " + receiver.getCapital().shortName; }
                            if (q == 4) { name = receiver.getCapital().shortName + " Entente"; }
                            if (q == 5) { name = "The " + receiver.getCapital().shortName + " League"; }
                            if (q == 6) { name = "The " + receiver.getCapital().shortName + " Compact"; }

                            message += "\nThis alliance will now go by the name of " + name;
                            receiver.allianceName = name;
                        }
                    }
                    society.map.world.prefabStore.popImgMsg(message, "The sheep naturally flock together in times of threat, seeking safety in number. But is the wolf already in their midst?");
                }
            }
        }

        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
