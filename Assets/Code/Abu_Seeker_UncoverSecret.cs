using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Seeker_UncoverSecret: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_UncoverSecret();
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " beings uncovering the Forgotten Secret hidden at " + u.location.getName() + "," +
                " a task which will take " + u.location.map.param.unit_seeker_uncoverTime + " turns, after which they will gain 1 secret.",
                u.location.map.world.wordStore.lookup("ABILITY_SEEKER_UNCOVER_SECRET_START"),img:2);

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
        public override string specialCost()
        {
            return "Major Evidence";
        }


        public override string getDesc()
        {
            return "Uncovers a forgotten secret, adding one to your total discovered. Leaves minor evidence behind."
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