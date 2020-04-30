using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_ExileUnit : VoteIssue
    {
        public Unit target;

        public VoteIssue_ExileUnit(Society soc,Unit target,Person proposer) : base(soc,proposer)
        {
            this.target = target;
        }

        public override string ToString()
        {
            return "Exile " + this.target.getName();
        }

        public override string getLargeDesc()
        {
            string reply = "Vote to exile " + target.getName() + " for the crime of association with dark forces." +
                "\n\nIf passed, this unit will be forcibly expelled from this society.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            //Option 1 is "Kill suspect"
            //Multiply values based on this, as they should be symmetric for most parts

            RelObj rel = voter.getRelation(target.person);
            double apathy = (1 - voter.shadow);

            if (option.index == 0)
            {
                double liking = rel.getLiking();
                msgs.Add(new ReasonMsg("Opinion of " + target.getName(), liking));
                u += liking;
            }
            else if (option.index == 1)
            {
                /*
                 * This is actually already covered by liking, but caps out at 100. This adds another bonus, purely from suspicion
                 */
                double proKill = rel.suspicion * apathy * voter.map.param.utility_exileSuspectFromSuspicion;
                msgs.Add(new ReasonMsg("Suspicion towards " + target.getName(), proKill));
                u += proKill;

                double liking = -rel.getLiking();
                msgs.Add(new ReasonMsg("Opinion of " + target.getName(), liking));
                u += liking;

                double reluctance = -voter.map.param.utility_killSuspectRelucatance;
                msgs.Add(new ReasonMsg("Base reluctance to exile agent", reluctance));
                u += reluctance;
            }



            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 1)
            {
                society.map.world.prefabStore.popMsg(society.getName() + " Found " + target.getName() + " guilty and exiled them");
                society.enemies.Add(target);
            }
            else
            {
                World.log("Failure to find guilty " + target.getName());
            }
        }
        public override bool stillValid(Map map)
        {
            map.units.Contains(target);
            return true;
        }
    }
}
