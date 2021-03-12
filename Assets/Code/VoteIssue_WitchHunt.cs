using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_WitchHunt : VoteIssue
    {
        public double reluctanceModifier = 1;
        public VoteIssue_WitchHunt(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Execute a noble without trial";
        }

        public override string getLargeDesc()
        {
            string reply = "The nobles have decided that there exist traitors in their midst, and seek to summarily execute them, without going through normal justice systems." +
                "They will vote on which noble to declare guilty, based on personal suspicions, fears and political considerations." +
                "This will cause the society to kill the target noble on sight, if they are found guilty.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            //Option 1 is "Kill suspect"
            //Multiply values based on this, as they should be symmetric for most parts
            double parityMult = 1;
            if (option.index == 0) { parityMult = -1; }

            RelObj rel = voter.getRelation(option.person);
            double apathy = (1 - voter.shadow);

            /*
             * This is actually already covered by liking, but caps out at 100. This adds another bonus, purely from suspicion
             */
            double proKill = rel.suspicion * apathy * voter.map.param.utility_killSuspectFromSuspicion * parityMult;
            msgs.Add(new ReasonMsg("Suspicion towards " + option.person.getFullName(), proKill));
            u += proKill;

            //double reluctance = -voter.map.param.utility_killSuspectRelucatance* parityMult * reluctanceModifier;
            //msgs.Add(new ReasonMsg("Base reluctance to kill noble", reluctance));
            //u += reluctance;

            double liking = -rel.getLiking() * parityMult;
            msgs.Add(new ReasonMsg("Opinion of " + option.person.getFullName(), liking));
            u += liking;

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            society.crisisWitchHunt = false;
            KillOrder order = new KillOrder(option.person, "Judged guilty of conspiracy with dark forces");
            order.votedByNobles = true;
            society.killOrders.Add(order);
            bool isGood = (option.person.state != Person.personState.enthralled) && (option.person.state != Person.personState.broken) && (option.person.shadow < 0.5);
            society.map.turnMessages.Add(new MsgEvent(society.getName() + " has voted to execute " + option.person.getFullName() + ", having judged them guilty of conspiracy with dark forces",MsgEvent.LEVEL_RED,isGood));

            World.log("EXECUTION ORDER GONE THROUGH FOR SUSPICION OF DARKNESS " + society.getName() + " " + option.person.getFullName() + " true shadow (" + option.person.shadow + ")");

        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
