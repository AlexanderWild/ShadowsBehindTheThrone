using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Seeker_ChangeDirection: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Seeker seeker = (Unit_Seeker)u;

            seeker.secrets -= 1;

            foreach (Location loc in map.locations)
            {
                List<Property> rems = new List<Property>();
                foreach (Property pr in loc.properties)
                {
                    if (pr.proto is Pr_ForgottenSecret)
                    {
                        rems.Add(pr);
                    }
                }
                foreach (Property pr in rems)
                {
                    loc.properties.Remove(pr);
                }
            }


            Unit_Seeker.addForgottenSecrets(map);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " changes their research direction, to a more promising path, allowing new secrets to be used.",
                u.location.map.world.wordStore.lookup("ABILITY_SEEKER_CHANGE_DIRECTION"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u is Unit_Seeker == false) { return false; }
            Unit_Seeker seeker = (Unit_Seeker)u;
            if (seeker.secrets == 0) { return false; }
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

        public override string getDesc()
        {
            return "Loses a secret in exchange for removing all secrets on the map and creating " + World.staticMap.param.unit_seeker_nCreatedSecrets + " randomly placed new ones"
                + "\n[Requires at least one secret]";
        }

        public override string getName()
        {
            return "Change Research Direction";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_seeker;
        }
    }
}