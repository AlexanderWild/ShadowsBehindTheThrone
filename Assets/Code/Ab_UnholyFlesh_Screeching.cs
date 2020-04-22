using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Screetching: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            hex.location.soc.temporaryThreat += World.staticMap.param.ability_fleshScreamThreatAdd;

            int nHit = 0;
            string nobleMsg = "";
            foreach (Location loc in hex.location.getNeighbours())
            {
                if (loc.person() != null)
                {
                    if (nHit > 0) { nobleMsg += ";"; }
                    nHit += 1;
                    loc.person().sanity -= map.param.ability_fleshScreamSanity;
                    if (loc.person().sanity < 0) { loc.person().sanity = 0; }
                    nobleMsg += loc.person().getFullName() + " (now " + loc.person().sanity + ") ";
                }
            }

            string msg = "Your fields of unholy flesh start screaming, adding " + World.staticMap.param.ability_fleshScreamThreatAdd + " temporary threat.";
            if (nHit > 0)
            {
                msg += "\nNobles hit by sanity drain: " + nobleMsg;
            }
            map.world.prefabStore.popImgMsg(msg        ,
                map.world.wordStore.lookup("UNHOLY_FLESH_SCREAM"));

        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFlesh();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (!(hex.location.soc is SG_UnholyFlesh)) { return false; }
            
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fleshScreamCost ;
        }

        public override string getDesc()
        {
            return "The flesh screams maddeningly. Adds " + World.staticMap.param.ability_fleshScreamThreatAdd + " Temporary Threat. Reduces all neighbour nobles sanity by " + World.staticMap.param.ability_fleshScreamSanity
                 + "\n[Requires an unholy flesh location]";
        }

        public override string getName()
        {
            return "Screaming Flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}