using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_CondemnAgent : VoteIssue
    {
        public Unit target;

        public VoteIssue_CondemnAgent(Society soc,Unit target,Person proposer) : base(soc,proposer)
        {
            this.target = target;
        }

        public override string ToString()
        {
            return "Condemn " + this.target.getName();
        }

        public override string getLargeDesc()
        {
            string reply = "Vote to condemn " + target.getName() + " for their crimes against the nation, including association with dark forces." +
                "\n\nIf found guilty, this agent will be considered hostile by the nation of " + society.getName() + ", and be subject to attack by their agents and lose health while in their territory.";
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
                society.map.world.prefabStore.popMsg(society.getName() + " Found " + target.getName() + " guilty and condemned them. Their agents can now attack " + target.getName() + "," +
                    " and " + target.getName() + " will take damage each turn they are in the nation's borders.");
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
