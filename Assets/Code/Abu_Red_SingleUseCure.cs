using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Red_SingleUseCure: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Property.addProperty(map, u.location, "Red Death");
            u.location.map.world.prefabStore.popImgMsg("SINGLE USE DESC.",
                u.location.map.world.wordStore.lookup("ABILITY_RED_SINGLE_USE_CURE"), 5);

            if (u.location.soc != null && u.location.soc is Society)
            {
                Society soc = (Society)u.location.soc;
                VoteSession sess = new VoteSession();
                VoteIssue issue = new VoteIssue_Crisis_SingleUseMedicine(soc,u.location.person());
                sess.issue = issue;
                soc.voteSession = sess;

                foreach (Location loc in map.locations)
                {
                    if (loc.soc != soc) { continue; }
                    if (loc.settlement != null && loc.person() != null)
                    {
                        bool diseased = false;
                        foreach (Property pr in loc.properties)
                        {
                            if (pr.proto.isDisease)
                            {
                                diseased = true;
                                break;
                            }
                        }

                        if (diseased)
                        {
                            VoteOption opt = new VoteOption();
                            opt.person = loc.person();
                            issue.options.Add(opt);
                        }
                    }
                }
            }
        }

        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            Society soc = (Society)u.location.soc;
            if (soc.voteSession != null) { return false; }
                foreach (Property pr in u.location.properties)
                {
                    if (pr.proto.isDisease)
                    {
                        return true;
                    }
                }
            return false;
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
            return World.staticMap.param.ability_redDeath_cureCooldown;
        }

        public override string getDesc()
        {
            return "Give a single dose of a miracle cure to a human society, forcing them to vote on which infected settlement to save"
                + "\n[Requires a diseased human settlement with a noble, in a society not currently voting]";
        }

        public override string getName()
        {
            return "Single Use Cure";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_redDeath;
        }
    }
}