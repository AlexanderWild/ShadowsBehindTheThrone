using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class VoteIssue_EconomicRebalancing : VoteIssue
    {
        private EconTrait econFrom;
        private EconTrait econTo;

        public VoteIssue_EconomicRebalancing(Society soc,Person proposer,EconTrait from,EconTrait to) : base(soc,proposer)
        {
            this.econFrom = from;
            this.econTo = to;
        }

        public override string ToString()
        {
            try
            {
                return "Rebalance Economy (" + econFrom.name + "/" + econTo.name + ")";
            }catch(Exception e)
            {
                World.log("Null pointer in econ rebalance");
                return "Rebalance economy";
            }
        }

        public override string getLargeDesc()
        {
            string reply = "Vote to rebalance the economy. This process will increase the prestige of particular industries (Each province has an industry) at the cost of another.";
            reply += "\nVoting for an option will anger nobles who are harmed by the economic changes, while somewhat pleasing those who benefit. This creates divisions in societies as provinces compete for economic gain.";
            return reply;
        }

        public override double computeUtility(Person p, VoteOption option, List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(p);

            double advtangeToMe = 0;
            double advantageToAllies = 0;
            double advantageToEnemeies = 0;
            foreach (Person affected in society.people)
            {
                if (affected.title_land != null)
                {
                    double delta = 1;
                    if (affected.title_land.settlement.econTraits().Contains(option.econ_to)) { delta *= society.map.param.econ_multFromBuff; }
                    if (affected.title_land.settlement.econTraits().Contains(option.econ_from)) { delta /= society.map.param.econ_multFromBuff; }

                    delta = 1 - delta;
                    //Run off the base prestige, so all change is regarded the same, regardless of existing changes
                    if (affected == p)
                    {
                        advtangeToMe = delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffect;
                    }
                    else
                    {
                        if (p.getRelation(affected).getLiking() > 0)
                        {
                            advantageToAllies += delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffectOther;
                        }
                        else
                        {
                            advantageToEnemeies += delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffectOther;
                        }
                    }
                }
            }

            //If we're actually affected by this, don't care more about the wellbeing of others than of yourself
            if (Math.Abs(advtangeToMe) > 1)
            {
                if (Math.Abs(advantageToAllies) > Math.Abs(advtangeToMe)/3)
                {
                    if (advantageToAllies > 0) { advantageToAllies = Math.Abs(advtangeToMe)/3; }
                    if (advantageToAllies < 0) { advantageToAllies = -Math.Abs(advtangeToMe)/3; }
                }
                if (Math.Abs(advantageToEnemeies) > Math.Abs(advtangeToMe)/3)
                {
                    if (advantageToEnemeies > 0) { advantageToEnemeies = Math.Abs(advtangeToMe)/3; }
                    if (advantageToEnemeies < 0) { advantageToEnemeies = -Math.Abs(advtangeToMe)/3; }
                }
            }

            msgs.Add(new ReasonMsg("Advantage to me", advtangeToMe));
            u += advtangeToMe;
            msgs.Add(new ReasonMsg("Advantage to allies", advantageToAllies));
            u += advantageToAllies;
            msgs.Add(new ReasonMsg("Advantage to enemies", advantageToEnemeies));
            u += advantageToEnemeies;

            //World.log("Econ advantages " + advtangeToMe + " " + advantageToAllies + " " + advantageToEnemeies);

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            EconEffect effect = new EconEffect(society.map, option.econ_from, option.econ_to);
            World.log(society.getName() + " implements economic policy, moving focus from " + option.econ_from.name + " to " + option.econ_to.name);
            society.econEffects.Add(effect);
        }

        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}