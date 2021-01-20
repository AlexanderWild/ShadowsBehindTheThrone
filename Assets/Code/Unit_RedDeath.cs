using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_RedDeath : Unit
    {

        
        public Unit_RedDeath(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 4;
            hp = 4;

            abilities.Add(new Abu_Red_Origin());
            abilities.Add(new Abu_Red_Outbreak());
            abilities.Add(new Abu_Red_SingleUseCure());
            abilities.Add(new Abu_Base_Recruit());
            abilities.Add(new Abu_Base_ChangeIdentity());
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


        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_redDeath;
        }

        public override bool definesForeground()
        {
            return true;
        }

        public override Sprite getPortraitForeground()
        {
            return World.staticMap.world.textureStore.icon_redDeath;
        }
        public override bool hasSpecialInfo()
        {
            return false;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.8f, 00.8f, 0.4f);
        }

        public override string getTitleM()
        {
            return "Masque of the Red Death";
        }

        public override string getTitleF()
        {
            return "Masque of the Red Death";
        }

        public override string getName()
        {
            return "Masque of the Red Death";
        }

        public override string getDesc()
        {
            return "The Masque of the Red Death is the spirit of a lethal disease, the embodiment of pestillence.";
        }
    }
}
