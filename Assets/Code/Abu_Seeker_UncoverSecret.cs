using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Seeker_UncoverSecret: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Seeker seeker = (Unit_Seeker)u;

            seeker.secrets += 1;

                List<Property> rems = new List<Property>();
                foreach (Property pr in u.location.properties)
                {
                    if (pr.proto is Pr_ForgottenSecret)
                    {
                        rems.Add(pr);
                    }
                }
                foreach (Property pr in rems)
                {
                    u.location.properties.Remove(pr);
                }

            string msg = u.getName() + " discovers a forgotten secret, they now have " + seeker.secrets + " and need " + seeker.reqSecrets + " to discover the truth.";
            if (seeker.secrets == seeker.reqSecrets)
            {
                msg = u.getName() + " learns a forgotten secret, and now knows enough to piece together the truth they have been seeking. Use their ability to do so.";
            }
            u.location.map.world.prefabStore.popImgMsg(msg,u.location.map.world.wordStore.lookup("ABILITY_SEEKER_UNCOVER_SECRET"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u is Unit_Seeker == false) { return false; }
            Unit_Seeker seeker = (Unit_Seeker)u;

            foreach (Property prop in u.location.properties)
            {
                if (prop.proto is Pr_ForgottenSecret)
                {
                    return true;
                }
            }

            return false;
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

        public override string getDesc()
        {
            return "Uncovers a forgotten secret, adding one to your total discovered"
                + "\n[Requires a Forgotten Secret at your location]";
        }

        public override string getName()
        {
            return "Uncover Secret";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_seeker;
        }
    }
}