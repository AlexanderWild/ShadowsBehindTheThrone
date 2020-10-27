using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Headless_BeginTrial: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_HeadlessHorseman head = (Unit_HeadlessHorseman)u;

            Society soc = (Society)u.location.soc;

            VoteIssue_JudgeSuspect issue = new VoteIssue_JudgeSuspect(soc, u.location.person(), u.location.person());
            VoteSession session = new VoteSession();
            session.issue = issue;
            soc.voteSession = session;
            VoteOption option_0 = new VoteOption();
            option_0.index = 0;
            issue.options.Add(option_0);
            VoteOption option_1 = new VoteOption();
            option_1.index = 1;
            issue.options.Add(option_1);

            string add = "";
            if (u.location.person().shadow <= 0.3 && u.location.person().state == Person.personState.normal)
            {
                add = "\nAs they are innocent, their head will become a pumpkin the horseman can gather.";
            }
            u.location.map.world.prefabStore.popImgMsg("A trial has begun. " + u.location.person().getFullName() + " is facing judgement. If you can convince enough nobles to vote guilty they will be beheaded." + add,
                u.location.map.world.wordStore.lookup("ABILITY_HEADLESS_BEGIN_TRIAL"),img:3);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u.location.soc == null || (u.location.soc is Society == false)) { return false; }

            Society soc = (Society)u.location.soc;
            return soc.voteSession == null;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Causes the inhabitants of a society to put the noble at your current location on trial. If this trial succeeds (because the nobles hate/fear/suspect the accused), their head could serve as a pumpkin-head for the horseman."
                + "\n[Requires a society with no current voting in progress at your location]";
        }

        public override string getName()
        {
            return "Begin the Trial";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_pumpkin;
        }
    }
}