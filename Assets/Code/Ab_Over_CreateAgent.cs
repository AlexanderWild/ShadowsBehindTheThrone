using UnityEngine;

using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Ab_Over_CreateAgent: Ability
    {
        public static int VAMPIRE = 0;
        public static string DESC_VAMPIRE = "Stealthy infiltrator and corruptor of the aristocracy." +
            "\nThe vampire is an agent able to partially conceal their actions. They must drink blood to survive, and to power their magics, but" +
            " can travel great distances between their places of feeding and the places where they act. This makes them good infiltrators and political agents, as they can" +
            " escape the consequences of their crimes for a great deal of time.";

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