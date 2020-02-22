using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_DismissFromCourt : VoteIssue
    {

        public VoteIssue_DismissFromCourt(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Dismiss an unlanded noble from court";
        }

        public override string getLargeDesc()
        {
            string reply = "Vote to remove a person from court, taking away their noble titles and dismissing them permanently. This action is taken if the court has too many unlanded nobles.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);
            RelObj rel = voter.getRelation(option.person);

            double liking = -rel.getLiking() * World.staticMap.param.utility_dismissFromCourt;
            msgs.Add(new ReasonMsg("Opinion of " + option.person.getFullName(), liking));
            u += liking;

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            Person target = option.person;
            bool isGood = (target.state != Person.personState.enthralled) && (target.state != Person.personState.broken) && (target.shadow < 0.5);
            society.map.turnMessages.Add(new MsgEvent(society.getName() + " has voted to dismiss " + target.getFullName() + " from court.", MsgEvent.LEVEL_RED, isGood));

            World.log("Dismiss from court");
            //World.staticMap.world.prefabStore.popMsg("Dismiss from court " + option.person.getFullName());
            option.person.removeFromGame("Dismissed from court");
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
