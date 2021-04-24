using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_AbyssalSirens: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            Property.addProperty(map, hex.location, "Abyssal Sirens");

            map.world.prefabStore.popImgMsg("The cult of the Deep at " + hex.location.getName() + " now takes over the minds of the population, including " + hex.location.person().getFullName() + "."
                    + " They will now slowly fall into shadow.",
                map.world.wordStore.lookup("ABILITY_FISHMAN_ABYSSAL_SIRENS"));

        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFishmen();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person().madness is Insanity_Sane) { return false; }
            if (hex.location.person().state != Person.personState.normal) { return false; }
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
            return World.staticMap.param.ability_fishmanHauntingAbyssalSirensCost;
        }

        public override string getDesc()
        {
            return "Preys upon those driven insane by the songs of the deep, causing them to gain shadow over time."
                + "\n[Requires a location with active Deep One Cult and an insane noble]";
        }

        public override string getName()
        {
            return "Abyssal Sirens";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}