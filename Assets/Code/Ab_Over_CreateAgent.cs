using UnityEngine;

using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Ab_Over_CreateAgent: Ability
    {
        public static int VAMPIRE = 0;
        public static int DOCTOR = 1;
        public static int SEEKER = 2;
        public static string DESC_VAMPIRE = "Stealthy infiltrator and corruptor of the aristocracy." +
            "\nThe vampire is an agent able to partially conceal their actions. They must drink blood to survive, and to power their magics, but" +
            " can travel great distances between their places of feeding and the places where they act. This makes them good infiltrators and political agents, as they can" +
            " escape the consequences of their crimes for a great deal of time.";
        public static string DESC_DOCTOR = "Necromantic Doctors pose as plague doctors to gain access to corpses, which they load onto their wagon for disposal. " +
            "Once sufficient are gathered, they can raise armies of the dead."
            +"\nThey gather corpses from graveyards, and need to travel far to collect sufficient for large armies. This nomadic life may well leave evidence behind, if they are unable to infiltrate or find recent battlefields.";

        public static string DESC_SEEKER = "The Seeker is compelled to search the world for forgotten secrets about a race of creatures which ruled the world before humanity to gain dark power."
            + "\nThey are weak for most of the game, and may need other agents' help to avoid getting exiled, but after gathering their fragments they can enshadow or destroy entire empires.";

        public override void cast(Map map, Hex hex)
        {
            List<int> indices = new List<int>();
            List<string> titles = new List<string>();
            List<string> descs = new List<string>();
            List<Sprite> icons = new List<Sprite>();

            indices.Add(VAMPIRE);
            titles.Add("Vampire");
            descs.Add(DESC_VAMPIRE);
            icons.Add(map.world.textureStore.icon_vampire);

            indices.Add(DOCTOR);
            titles.Add("Necromantic Doctor");
            descs.Add(DESC_DOCTOR);
            icons.Add(map.world.textureStore.icon_doctor);

            indices.Add(SEEKER);
            titles.Add("Seeker");
            descs.Add(DESC_SEEKER);
            icons.Add(map.world.textureStore.icon_seeker);
            map.world.ui.addBlocker(map.world.prefabStore.getScrollSetAgents(indices,titles,descs,icons).gameObject);
        }


        public override bool castable(Map map, Person person)
        {
            return false;
        }

        public override bool castable(Map map, Unit unit)
        {
            return false;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_enthrallUnitCooldown;
        }

        public override bool castable(Map map, Hex hex)
        {
            int n = 0;
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled()) { n += 1; }
            }
            return hex.location != null && n < map.param.units_maxEnthralled;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_enthrallUnitCost;
        }
        public override string getDesc()
        {
            return "Create an agent for to use, as long as you are not at your maximum agent count."
                + "\n[Max " + World.staticMap.param.units_maxEnthralled + "]";
        }

        public override string getName()
        {
            return "Create Agent";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}