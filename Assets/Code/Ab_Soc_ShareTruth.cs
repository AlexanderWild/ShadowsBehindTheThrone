using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_ShareTruth: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person victim = hex.location.person();
            castInner(map, victim);
        }

        public override void castInner(Map map, Person victim)
        {
            Society soc = victim.society;

            victim.getRelation(map.overmind.enthralled).suspicion = 1;
            victim.sanity = 0;


            map.world.prefabStore.popImgMsg(
                "You share the truth of the world with " + victim.getFullName() + ", a terrible truth which ruins their mind. They are now at 0 sanity," +
                " and will turn insane, but are aware of your enthralled's dark nature.",
                map.world.wordStore.lookup("SOC_REVEAL_TRUTH"));
        }

        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (person == map.overmind.enthralled) { return false; }
            if (person.society != map.overmind.enthralled.society) { return false; }

            return true;
        }
        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map,hex.location.person());
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_shareTruthCooldown;
        }
        
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "The enthralled reveals the full truth of the world to a chosen noble. Their suspicion raises to 100%, but their sanity drops to 0."
                + "\n[Requires a noble in your enthralled's society]";
        }

        public override string getName()
        {
            return "Reveal Terrible Truth";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_eyes;
        }
    }
}