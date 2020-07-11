using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Defend: Ability
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
                    hex.location.settlement = new Set_UnholyFlesh_Carapace(hex.location);
                    l.soc.temporaryThreat += map.param.ability_growFleshThreatAdd;

                    map.world.prefabStore.popImgMsg(
                        "You grow a new fleshy growth, this one heavily clad in armoured carapace, to resist attack. This growth has increased the temporary threat of the flesh.",
                        map.world.wordStore.lookup("UNHOLY_FLESH_DEFEND"));
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
            return 5;
        }

        public override string getDesc()
        {
            return "Forms a field of hardened spines, plates and scales, to allow the flesh to defend itself. Can absorb damage for the hive, protecting the flesh units."
                 + "\nAdds " + (int)(World.staticMap.param.ability_growFleshThreatAdd) + " temporary threat to the growing flesh."
                 + "\n[Requires a land location adjacent to an existing unholy flesh location]";
        }

        public override string getName()
        {
            return "Protect the Flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}