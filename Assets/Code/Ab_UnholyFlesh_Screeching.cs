using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Screetching : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            hex.location.soc.temporaryThreat += World.staticMap.param.ability_fleshScreamThreatAdd;

            map.world.prefabStore.popImgMsg(
                "Your fields of unholy flesh start screaming, adding " +  World.staticMap.param.ability_fleshScreamThreatAdd + " temporary threat.",
                map.world.wordStore.lookup("UNHOLY_FLESH_SCREAM"));
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
            return "Causes flesh to start violently screaming, causing fear and panic in the locals. Adds " + World.staticMap.param.ability_fleshScreamThreatAdd + " Temporary Threat"
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