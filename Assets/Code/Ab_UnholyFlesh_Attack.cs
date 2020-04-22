using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Attack: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && (l.soc is SG_UnholyFlesh))
                {
                    if (l.soc.getRel(hex.location.soc).state == DipRel.dipState.war) { continue; }
                    map.declareWar(l.soc, hex.location.soc);

                    map.world.prefabStore.popImgMsg(
                        "The unholy flesh suddenly turns hostile, and begins its attack on " + hex.location.soc.getName(),
                        map.world.wordStore.lookup("UNHOLY_FLESH_ATTACK"));
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
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is SG_UnholyFlesh) { return false; }

            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && (l.soc is SG_UnholyFlesh))
                {
                    if (l.soc.getRel(hex.location.soc).state == DipRel.dipState.war) { continue; }
                    return true;
                }
            }
            return false;
        }

        public override int getCost()
        {
            return 1;
        }

        public override string getDesc()
        {
            return "Causes flesh from all neighbouring hives to rise up and attack the inhabitants of the selected location."
                 + "\n[Requires a land location adjacent to an existing unholy flesh location]";
        }

        public override string getName()
        {
            return "Rally the Flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}