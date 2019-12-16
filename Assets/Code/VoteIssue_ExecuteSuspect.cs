using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_JudgeSuspect : VoteIssue
    {
        public Person target;

        public VoteIssue_JudgeSuspect(Society soc,Person target,Person proposer) : base(soc,proposer)
        {
            this.target = target;
        }

        public override string ToString()
        {
            return "Judge Suspect " + this.target.getFullName();
        }

        public override string getLargeDesc()
        {
            string reply = "Vote to execute a suspect for the crime of association with dark forces. This primarily happens if nobles have high suspicion. This will cause the society to kill the target noble on sight, if they are found guilty.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            //Option 1 is "Kill suspect"
            //Multiply values based on this, as they should be symmetric for most parts
            double parityMult = 1;
            if (option.index == 0) { parityMult = -1; }

            RelObj rel = voter.getRelation(target);
            double apathy = (1 - voter.shadow);

            /*
             * This is actually already covered by liking, but caps out at 100. This adds another bonus, purely from suspicion
             */
            double proKill = rel.suspicion * apathy * voter.map.param.utility_killSuspectFromSuspicion * parityMult;
            msgs.Add(new ReasonMsg("Suspicion towards " + target.getFullName(), proKill));
            u += proKill;

            double reluctance = -voter.map.param.utility_killSuspectRelucatance*parityMult;
            msgs.Add(new ReasonMsg("Base reluctance to kill noble", reluctance));
            u += reluctance;

            double liking = -rel.getLiking() * parityMult;
            msgs.Add(new ReasonMsg("Opinion of " + target.getFullName(), liking));
            u += liking;

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 1)
            {
                society.killOrders.Add(new KillOrder(target,"Judged guilty of conspiracy with dark forces"));
                bool isGood = (target.state != Person.personState.enthralled) && (target.state != Person.personState.broken) && (target.shadow < 0.5);
                //society.map.turnMessages.Add(new MsgEvent(society.getName() + " has voted to execute " + target.getFullName() + ", having judged them guilty of conspiracy with dark forces",MsgEvent.LEVEL_RED,isGood));

                World.log("EXECUTION ORDER GONE THROUGH FOR SUSPICION OF DARKNESS " + society.getName() + " " + target.getFullName() + " true shadow (" + target.shadow + ")");
            }
            else
            {
                World.log("Failure to find guilty " + target.getFullName());
            }
        }
        public override bool stillValid(Map map)
        {
            society.people.Contains(target);
            return true;
        }
    }
}
