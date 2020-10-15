using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_TesterDark : Unit
    {

        
        public Unit_TesterDark(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 5;
            hp = 5;
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
            return World.staticMap.world.textureStore.person_back_vampire;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_enthralled;
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.8f, 0, 0);
        }

        public override string specialInfo()
        {
            return "Autonomous";
        }

        public override string specialInfoLong()
        {
            return "Automatic testing agent";
        }

        public override string getTitleM()
        {
            return "TestDark";
        }

        public override string getTitleF()
        {
            return "TestDark";
        }

        public override string getDesc()
        {
            return "Vampires are powerful and stealthy while taking actions, but depend on a constant supply of blood to survive, which leaves a trail of evidence.";
        }
    }
}
