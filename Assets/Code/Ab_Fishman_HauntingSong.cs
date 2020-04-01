using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_HauntingSong: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            Property.addProperty(map, hex.location, "Haunting Song");

            map.world.prefabStore.popImgMsg("The song of the Deep Ones at " + hex.location.getName() + " grows to a maddening pitch, constant regardless of obstruction. " + hex.location.person().getFullName() + "'s sanity will drop over time as the song persists.",
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
            return "Aggravates the Deep One's song, causing damaging the sanity of those who hear it. Causes the noble present at this location to have a 50% chance per turn to lose a point of sanity."
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