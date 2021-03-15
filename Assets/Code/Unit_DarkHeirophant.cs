using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_DarkHeirophant : Unit
    {
        
        public Unit_DarkHeirophant(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 3;
            hp = 3;


            abilities.Add(new Abu_Heir_EnthrallNoble());
            abilities.Add(new Abu_Heir_BreakNoble());
            abilities.Add(new Abu_Base_Infiltrate());
            abilities.Add(new Abu_Base_SocialiseAtCourt());
            abilities.Add(new Abu_Base_Recruit());
            abilities.Add(new Abu_Base_ChangeIdentity());
            abilities.Add(new Abu_Base_SetLoose());
            abilities.Add(new Abu_Base_Apoptosis());
        }

        public override void turnTickInner(Map map)
        {
        }

        public override bool checkForDisband(Map map)
        {
            return false;
        }
        public override void turnTickAI(Map map)
        {
        }

        public override bool definesBackground()
        {
            return true;
        }

        public override Sprite getPortraitBackground()
        {
            return World.staticMap.world.textureStore.person_back_shadow;
        }


        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_heirophant;
        }

        public override bool hasSpecialInfo()
        {
            return false;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.8f, 00.8f, 0.4f);
        }

        public override string specialInfo()
        {
            return "Enthraller";
        }

        public override string specialInfoLong()
        {
            return "The Dark Heirophant leads your cult, converting the nobles of the world to your cause. They can fully enthrall one, making them your direct instrument, and can break many, " +
                "allowing you to build political allies who will ignore the darkness.";
        }

        public override string getTitleM()
        {
            return "Heirophant";
        }

        public override string getTitleF()
        {
            return "Heirophant";
        }

        public override string getDesc()
        {
            return "The Dark Heirophant converts nobles to your cult, allowing you to enthrall a noble, and break the souls of others.";
        }
    }
}
