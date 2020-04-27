using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fash_DenouncePacifists: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return; }

            castInner(map, hex.location.person());
        }
        public override void castInner(Map map, Person person)
        {
            Society soc = person.society;

            int nAffected = 0;
            foreach (Person p in soc.people)
            {
                bool affected = false;
                ThreatItem t = p.getGreatestThreat();
                if (t.threat >= map.param.person_fearLevel_terrified)
                {
                    foreach (Person p2 in soc.people)
                    {
                        if (p2 == p) { continue; }
                        if (p2.state == Person.personState.enthralled) { continue; }
                        if (p2.politics_militarism < 0)
                        {
                            p.getRelation(p2).addLiking(map.param.ability_denouncePacisfistsLiking, "Pacifists expose us to danger", map.turn);
                        }
                    }
                    if (affected)
                    {
                        nAffected += 1;
                    }

                }
            }

            map.world.prefabStore.popImgMsg(
                "You call on the nobles of " + soc.getName() + " to reject the pacifists in their midst, explaining that these people are exposing you all to danger by refusing to take up arms against your many foes.\n" +
                nAffected + " nobles are affected.",
                map.world.wordStore.lookup("ABILITY_DENOUNCE_PACIFISTS"));
        }

        public override bool castable(Map map, Person person)
        {
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return false; }
            return true;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_denouncePacifistsCooldown;
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Causes all members of a society to dislike each other (except your enthralled) if they do not share the same degree of fear towards their greatest threat, proportional to their fear difference."
                + "\n[Requires a society]";
        }

        public override string getName()
        {
            return "Polarise by Fear";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}