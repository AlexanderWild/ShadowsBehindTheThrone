using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Headless_GatherPumpkin: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_HeadlessHorseman head = (Unit_HeadlessHorseman)u;
            head.heads += 1;

            Property removed = null;
            foreach (Property pr in u.location.properties)
            {
                if (pr.proto is Pr_Pumpkin)
                {
                    removed = pr;
                }
            }
            if (removed != null)
            {
                u.location.properties.Remove(removed);
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " has gathered a new pumpkin-head to add to their collection. They now have " + head.heads + ".",
                u.location.map.world.wordStore.lookup("ABILITY_HEADLESS_GATHER_PUMPKIN"),img:3);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u is Unit_HeadlessHorseman == false) { return false; }

            foreach (Property prop in u.location.properties)
            {
                if (prop.proto is Pr_Pumpkin)
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
            return "Collects a pumpkin-head which was created by a society voting to execute an innocent noble (not enshadowed or enthralled)"
                + "\n[Requires a Pumpkin Head at your location]";
        }

        public override string getName()
        {
            return "Gather Pumpkin Head";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_pumpkin;
        }
    }
}