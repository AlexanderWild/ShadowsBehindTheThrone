using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_HauntingSong : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            hex.location.person().sanity -= World.staticMap.param.ability_fishmanHauntingSongHit;
            if (hex.location.person().sanity < 0) { hex.location.person().sanity = 0; }

                map.world.prefabStore.popImgMsg("The song of the Deep Ones at " + hex.location.getName() + " grows to a maddening pitch, constant regardless of obstruction. " + hex.location.person().getFullName() + "'s sanity drops by "
                   + World.staticMap.param.ability_fishmanHauntingSongHit + " and is now " + hex.location.person().sanity + ".",
                map.world.wordStore.lookup("ABILITY_FISHMAN_HAUNTING_SONG"));

        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            foreach (Property p in hex.location.properties)
            {
                if (p.proto is Pr_DeepOneCult)
                {
                    return true;
                }
            }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fishmanHauntingSongCost;
        }

        public override string getDesc()
        {
            return "Aggravates the Deep One's song, causing damaging the sanity of those who hear it. Causes " + World.staticMap.param.ability_fishmanHauntingSongHit + " sanity damage."
                + "\n[Requires a location with active Deep One Cult]";
        }

        public override string getName()
        {
            return "Haunting Song";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}