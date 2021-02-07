using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Grow: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }
            
            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && l.soc is SG_UnholyFlesh && l.settlement != null)
                {
                    hex.location.soc = l.soc;
                    hex.location.settlement = new Set_UnholyFlesh_Ganglion(hex.location);
                    l.soc.temporaryThreat += map.param.ability_growFleshThreatAdd;

                    map.world.prefabStore.popImgMsg(
                        "You grow a new fleshy growth, as it matures it will become capable of war against human forces.",
                        map.world.wordStore.lookup("UNHOLY_FLESH_GROW"));
                    return;
                }
            }
        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFlesh();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc != null) { return false; }
            if (hex.location.settlement != null) { return false; }
            if (hex.location.isOcean) { return false; }
            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && l.soc is SG_UnholyFlesh && l.settlement != null)
                {
                    return true;
                }
            }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fleshGrowCost;
        }

        public override string getDesc()
        {
            return "Grows the unholy flesh outwards, forming new structures able to provide combat-ready appendages and extrusions."
                 + "\nAdds " + (int)(World.staticMap.param.ability_growFleshThreatAdd) + " temporary threat to the growing flesh."
                 + "\n[Requires a land location adjacent to an existing unholy flesh location]";
        }

        public override string getName()
        {
            return "Expand the Flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}