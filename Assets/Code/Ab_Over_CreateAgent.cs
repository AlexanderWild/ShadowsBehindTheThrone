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
        public static int PUMPKIN = 3;
        public static int HEIROPHANT = 4;
        public static int REDDEATH = 5;
        public static int SAVIOUR = 6;

        public static string DESC_VAMPIRE = "Stealthy infiltrator and corruptor of the aristocracy." +
            "\nThe vampire is an agent able to partially conceal their actions. They must drink blood to survive, and to power their magics, but" +
            " can escape immediate detection by travelling between their places of feeding and the places where they act. This makes them good infiltrators and political agents, as they can" +
            " escape the consequences of their crimes for a great deal of time.";
        public static string DESC_DOCTOR = "Necromantic Doctors pose as plague doctors to gain access to corpses, which they load onto their wagon for disposal. " +
            "Once sufficient are gathered, they can raise armies of the dead."
            +"\nThey gather corpses from graveyards, and need to travel far to collect sufficient for large armies. This nomadic life may well leave evidence behind, if they are unable to infiltrate or find recent battlefields.";

        public static string DESC_SEEKER = "The Seeker is compelled to search the world for forgotten secrets about a race of creatures which ruled the world before humanity to gain dark power."
            + "\nThey are weak for most of the game, and may need other agents' help to avoid getting exiled, but after gathering their fragments they can enshadow or destroy entire empires.";

        public static string DESC_PUMPKIN = "The Headless Horseman is the ghost of a noble who was beheaded for a crime they did not commit. They seek vengence for this against the living, by" +
            " collecting the heads of the living (in the form of pumpkins). They must collect as many as possible before their timer runs out."
            + "\nUse other agents to create madness in societies, then use the horseman to start trials against innocent (no shadow, not enthralled) nobles who are hated/suspected." +
            " Use power to swing close votes in the voting screen.";

        public static string DESC_HEIROPHANT = "The Dark Heirophant is the preacher of your blasphemous faith. They convert nobles to your cult, once you have infiltrated their locations."
            + " They allow you to enthrall a single noble, to act as your political instrument, and can break the souls of other nobles, preventing them from seeing evidence of the darkness," +
            "and making them your accomplices in your rise to power.";

        public static string DESC_REDDEATH = "The Masque of the Red Death is a spirit of the dread disease. The disease will slowly eradicate human settlements by decreasing their population," +
            " and cripples any army which relied on the infected settlements for support, leaving the nations weakened in the face of outside threats. If played as a political tool, however, " +
            " it is possible the nobles' panic and selfishness will harm them more than the disease ever could, as they squabble over who and where to protect, and over limited cures.";

        public static string DESC_SAVIOUR = "The Saviour is an agent who `saves' the human nations from disasters of your own creation. If forces under your control are winning a war against a human" +
            " nation, The Saviour can cause the humans to win the war instead, earning them the trust and adoration of all nobles in that land and nobles neighbouring that land. They can also cure diseases" +
            " rapidly, winning over nobles who feared disease. This liking can be used to boost your enthralled noble's prestige, to influence politics or to greatly boost infiltration effectiveness.";


        public override void cast(Map map, Hex hex)
        {
            List<int> indices = new List<int>();
            List<string> titles = new List<string>();
            List<string> descs = new List<string>();
            List<Sprite> icons = new List<Sprite>();


            if (World.useHorseman)
            {
                bool hasHorseman = false;
                foreach (Unit u in map.units)
                {
                    if (u is Unit_HeadlessHorseman) { hasHorseman = true; }
                }
                if (!hasHorseman)
                {
                    indices.Add(PUMPKIN);
                    titles.Add("The Headless Horseman");
                    descs.Add(DESC_PUMPKIN);
                    icons.Add(map.world.textureStore.icon_pumpkin);
                }
            }

            indices.Add(VAMPIRE);
            titles.Add("Vampire");
            descs.Add(DESC_VAMPIRE);
            icons.Add(map.world.textureStore.icon_vampire);

            indices.Add(DOCTOR);
            titles.Add("Necromantic Doctor");
            descs.Add(DESC_DOCTOR);
            icons.Add(map.world.textureStore.icon_doctor);


            bool hasSeeker = false;
            foreach (Unit u in map.units)
            {
                if (u is Unit_Seeker) { hasSeeker = true; break; }
            }
            if (!hasSeeker)
            {
                indices.Add(SEEKER);
                titles.Add("Seeker");
                descs.Add(DESC_SEEKER);
                icons.Add(map.world.textureStore.icon_seeker);
            }

            if (map.simplified == false)
            {
                indices.Add(HEIROPHANT);
                titles.Add("Dark Heirophant");
                descs.Add(DESC_HEIROPHANT);
                icons.Add(map.world.textureStore.icon_heirophant);
            }


            bool hasRedDeath = false;
            foreach (Unit u in map.units)
            {
                if (u is Unit_RedDeath) { hasRedDeath = true; break; }
            }
            if (!hasRedDeath)
            {
                indices.Add(REDDEATH);
                titles.Add("Masque of the Red Death");
                descs.Add(DESC_REDDEATH);
                icons.Add(map.world.textureStore.icon_redDeath);
            }


            indices.Add(SAVIOUR);
            titles.Add("Saviour");
            descs.Add(DESC_SAVIOUR);
            icons.Add(map.world.textureStore.icon_saviour);

            map.world.ui.addBlocker(map.world.prefabStore.getScrollSetAgents(indices, titles, descs, icons).gameObject);
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
            return hex.location != null && map.overmind.nEnthralled < map.param.overmind_maxEnthralled && map.overmind.availableEnthrallments > 0;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_enthrallUnitCost;
        }
        public override string getDesc()
        {
            return "Create an agent for to use, as long as you are not at your maximum agent count and have at least one enthrallment use available."
                + "\n[Enthralled under your control: "+ World.staticMap.overmind.nEnthralled + "/" 
                + World.staticMap.param.overmind_maxEnthralled + " Enthrallment Uses: " + World.staticMap.overmind.availableEnthrallments + "]";
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