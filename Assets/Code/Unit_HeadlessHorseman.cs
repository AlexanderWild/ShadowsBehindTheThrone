using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_HeadlessHorseman : Unit
    {
        public int heads = 0;
        public int turnsLeft = 50;


        public Unit_HeadlessHorseman(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 5;
            hp = maxHp;

            abilities.Add(new Abu_Headless_GatherPumpkin());
            abilities.Add(new Abu_Headless_BeginTrial());
            abilities.Add(new Abu_Headless_AlterInsanity());
            abilities.Add(new Abu_Base_Recruit());
        }

        public override void turnTickInner(Map map)
        {
            turnsLeft -= 1;
            if (turnsLeft < 0)
            {
                if (heads == 0)
                {
                    map.world.prefabStore.popMsg("The horseman has failed! They did not gather any heads during this haunting!");
                }
                else
                {
                    map.world.prefabStore.popPumpkinVictory("The horseman took " + heads + (heads > 1 ? " heads " : " head") + " during their haunting!");
                }
                map.units.Remove(this);
                location.units.Remove(this);
                GraphicalMap.selectedSelectable = null;
            }
        }

        public override bool checkForDisband(Map map)
        {
            return false;
        }
        public override void turnTickAI(Map map)
        {
        }

        public override bool definesForeground()
        {
            return true;
        }

        public override Sprite getPortraitForeground()
        {
            return World.staticMap.world.textureStore.icon_pumpkin;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_pumpkin;
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.8f, 0.6f, 0.1f);
        }

        public override string specialInfo()
        {
            return "Heads Taken: " + heads + "\nTurns Left: " + turnsLeft;
        }

        public override string specialInfoLong()
        {
            return "The headless horseman was decapitated for their crimes, and now seeks revenge on the nobles of the world. They must cause innocent (non-shadow) nobles to be executed, so they can" +
                " steal their heads (in the form of pumpkins). They must gather as many as possible before their time in this world ends again.";
        }

        public override string getName()
        {
            return "The Headless Horseman";
        }

        public override string getDesc()
        {
            return "The headless horseman must cause others to lose their heads, by spreading paranoia, then calling execution votes against nobles. Collect pumpkin heads before time runs out.";
        }

        public override string getTitleM()
        {
            return "Headless Horseman";
        }

        public override string getTitleF()
        {
            return "Headless Horseman";
        }
    }
}
