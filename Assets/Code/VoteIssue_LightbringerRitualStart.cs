using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_LightbringerRitualStart : VoteIssue
    {

        public VoteIssue_LightbringerRitualStart(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Begin Lightbringer Ritual";
        }
        public override string getLargeDesc()
        {
            string reply = "Vote to begin the Lightbringer Ritual, which will defeat the Darkness, if it is not interrupted during the casting.";
            return reply;
        }

        public override string getIndexInfo(int index)
        {
            if (index == 0)
            {
                return "Begin Ritual";
            }
            else
            {
                return "Do not start Ritual";
            }
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            double desire = voter.awareness * voter.map.worldPanic;
            double counter = 0;
            counter = voter.shadow;
            if (voter.state == Person.personState.broken) { counter = 1; }
            if (voter.state == Person.personState.enthralled) { counter = 1; }
            if (voter.state == Person.personState.enthralledAgent) { counter = 1; }

            double localU = 0;

            //Begin
            if (option.index == 0)
            {
                localU = desire*voter.map.param.utility_lightbringerRitual;
                msgs.Add(new ReasonMsg("Awareness and World Panic", localU));
                u += localU;

                localU = -1 * counter * voter.map.param.utility_lightbringerRitual;
                if (counter > 1)
                {
                    msgs.Add(new ReasonMsg("Enshadowment of own soul", localU));
                    u += localU;
                }

                localU = voter.map.param.utility_lightbringerRitualReluctance;
                msgs.Add(new ReasonMsg("Cost of Ritual", localU));
                u += localU;
            }
            else //No ritual
            {
                localU = -1 * desire * voter.map.param.utility_lightbringerRitual;
                msgs.Add(new ReasonMsg("Awareness and World Panic", localU));
                u += localU;

                localU = counter * voter.map.param.utility_lightbringerRitual;
                if (counter > 1)
                {
                    msgs.Add(new ReasonMsg("Enshadowment of own soul", localU));
                    u += localU;
                }

                localU = -1*voter.map.param.utility_lightbringerRitualReluctance;
                msgs.Add(new ReasonMsg("Cost of Ritual", localU));
                u += localU;
            }

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 0)
            {
                Overmind overmind = society.map.overmind;
                if (overmind.lightbringerCasters == null && society.getCapital() != null)
                {
                    overmind.lightbringerCasters = society;
                    overmind.lightRitualProgress = 0;
                    overmind.lightbringerLocations = new List<Location>();
                    overmind.lightbringerCapital = society.getCapital();

                    int nUniversities = 0;
                    int nTemples = 0;
                    List<Location> unis = new List<Location>();
                    List<Location> temples = new List<Location>();
                    foreach (Location loc in overmind.map.locations)
                    {
                        if (loc.soc == society)
                        {
                            if (loc.settlement is Set_University) { nUniversities += 1; unis.Add(loc); }
                            if (loc.settlement is Set_Abbey) { nTemples += 1; temples.Add(loc); }
                        }
                    }
                    if (nUniversities > nTemples || (nTemples == nUniversities && (Eleven.random.Next(2) == 0)))
                    {
                        overmind.lightbringerLocations = unis;
                    }
                    else
                    {
                        overmind.lightbringerLocations = temples;
                    }
                    if (overmind.firstLightbringer)
                    {

                    }
                    overmind.firstLightbringer = false;

                    overmind.map.world.prefabStore.popLightbringerMsg(society);
                }
            }
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
