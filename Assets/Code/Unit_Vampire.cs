using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Vampire : Unit
    {
        public int maxBlood = 27;
        public int blood = 27;

        
        public Unit_Vampire(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 5;
            hp = 5;

            maxBlood = loc.map.param.unit_vampire_maxBlood;
            blood = maxBlood/2;

            abilities.Add(new Abu_Vamp_Drink());
            abilities.Add(new Abu_Vamp_Infiltrate());
            abilities.Add(new Abu_Vamp_Insanity());
            abilities.Add(new Abu_Base_SpreadShadow());
            abilities.Add(new Abu_Base_Recruit());
        }

        public override void turnTickInner(Map map)
        {
            blood -= 1;
            if (blood <= 0)
            {
                this.die(map, "Died from lack of blood");
            }
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
            return world.textureStore.unit_vampire;
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
            return "Blood " + blood + "/" + maxBlood;
        }

        public override string specialInfoLong()
        {
            return "Vampires must drink the blood of the living to survive. Every turn their blood will drop by 1, and they will die if it reaches 0. They can replenish blood by drinking,"
                + " but this will leave evidence. It is best to feed in one nation and work in another to keep the secrets far away.";
        }

        public override string getTitleM()
        {
            return "Vampire";
        }

        public override string getTitleF()
        {
            return "Vampire";
        }

        public override string getDesc()
        {
            return "Vampires are powerful and stealthy while taking actions, but depend on a constant supply of blood to survive, which leaves a trail of evidence.";
        }
    }
}
