using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Seeker_LearnTruth: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Seeker seeker = (Unit_Seeker)u;

            seeker.secrets = 0;

            seeker.knowsTruth = true;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " pieces together the truth, from the secrets they have found, and gains dark power from the revelation.",
                u.location.map.world.wordStore.lookup("ABILITY_SEEKER_LEARN_TRUTH"), img: 2);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u is Unit_Seeker == false) { return false; }
            Unit_Seeker seeker = (Unit_Seeker)u;
            if (seeker.secrets < map.param.unit_seeker_nReqSecrets) { return false; }
            return true;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }
        public override string specialCost()
        {
            return " ";
        }

        public override string getDesc()
        {
            return "Collects together enough secrets for your seeker to learn the terrible truth, and gain new powers"
                + "\n[Requires at least " + World.staticMap.param.unit_seeker_nReqSecrets + " secrets]";
        }

        public override string getName()
        {
            return "Learn Truth";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_seeker;
        }
    }
}