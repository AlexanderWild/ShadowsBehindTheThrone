using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_Attack: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }
            
            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && (l.soc is SG_Fishmen))
                {
                    if (l.soc.getRel(hex.location.soc).state == DipRel.dipState.war) { continue; }
                    map.declareWar(l.soc, hex.location.soc);
                    l.soc.threat_mult = map.param.dark_evilThreatMult;
                    l.soc.setName("Deep Ones");
                    SG_Fishmen deepOnes = (SG_Fishmen)l.soc;
                    deepOnes.hasAttacked = true;
                    map.world.prefabStore.popImgMsg(
                        "The Deep Ones rise from the ocean and begin war on " + hex.location.soc.getName() + ". They are now revealed to the world.",
                        map.world.wordStore.lookup("ABILITY_FISHMAN_WAR"));
                }
            }
        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFishmen();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is SG_Fishmen) { return false; }

            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && (l.soc is SG_Fishmen))
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
            return "Causes the Deep Ones to rise from the deep and attack the inhabitants of the chosen location. Reveals the Deep Ones if they are not already, setting their threat perception to normal."
                 + "\n[Requires a human location adjacent to an existing deep one location]";
        }

        public override string getName()
        {
            return "War on the Surface";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}