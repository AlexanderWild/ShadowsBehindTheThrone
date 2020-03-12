using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_Fearmonger: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }

        public override void castInner(Map map, Person other)
        {

            ThreatItem item = other.threatEvaluations[0];
            foreach (Person p2 in other.society.people)
            {
                foreach (ThreatItem item2 in p2.threatEvaluations)
                {
                    if (item2.isSame(item))
                    {
                        item2.temporaryDread += map.param.ability_fearmongerTempThreat;
                    }
                }
                p2.computeThreats();
            }

            map.world.prefabStore.popImgMsg(
                "Your enthralled brings up the fear of " + item.getTitle() + ", causing the nobles of " + other.society.getName() + " to dread it, increasing threat temporarily.",
                map.world.wordStore.lookup("ABILITY_FEARMONGER"));
        }
        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (person.state == Person.personState.enthralled) { return false; }
            return person.society != map.overmind.enthralled.society;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (map.overmind.enthralled == null) { return false; }
            if (hex.location.soc != map.overmind.enthralled.society) { return false; }
            return true;
        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_fearmongerCooldown;
        }
        public override string getDesc()
        {
            return "Amplifies the highest threat of the chosen noble, causing all other nobles to gain " + World.staticMap.param.ability_fearmongerTempThreat + " dread towards the same threat."
                + "\n[Requires a noble in your enthralled's society]";
        }

        public override string getName()
        {
            return "Fearmonger";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}