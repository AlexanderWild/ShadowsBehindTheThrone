using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_CultOfTheDeep: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Property.addProperty(map, hex.location, "Cult of the Deep");
            World.log("Properties: " + hex.location.properties.Count);

            map.world.prefabStore.popImgMsg("You call to the land, starting a cult in " + hex.location.getName() + ". Humans will slowly convert and join your Deep One Forces, but the rest will fear them.",
                map.world.wordStore.lookup("ABILITY_FISHMAN_CALL"));
        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFishmen();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is Society == false) { return false; }
            if (hex.location.settlement.isHuman == false) { return false; }

            foreach (Location loc in hex.location.getNeighbours())
            {
                if (loc.soc != null && loc.soc is SG_Fishmen) { return true; }
            }
            return false;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fishmanCultOfTheDeep;
        }

        public override string getDesc()
        {
            return "Begins a deep one cult, which will slowly turn people in a human settlement into Deep Ones, adding to the Deep Ones' military might. Adds temporary threat to the Deep Ones and causes local noble to fear them."
                + "\n[Requires human settlement adjacent to a Deep One location]";
        }

        public override string getName()
        {
            return "Cult of the Deep";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}