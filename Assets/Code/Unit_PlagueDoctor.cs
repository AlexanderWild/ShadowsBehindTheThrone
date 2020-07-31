using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_NecroDoctor : Unit
    {

        public int corpses;
        public int maxCorpses;
        
        public Unit_NecroDoctor(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 3;
            hp = 3;

            maxCorpses = 100;

            abilities.Add(new Abu_Doctor_SowCorpseroot());
        }

        public override void turnTickInner(Map map)
        {
            corpses -= 1;
            if (corpses < 0) { corpses = 0; }
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
            return World.staticMap.world.textureStore.person_back_eyes;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_doctor;
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.4f, 0.8f, 0.1f);
        }

        public override string specialInfo()
        {
            return "Bodies " + corpses + "/" + maxCorpses;
        }

        public override string specialInfoLong()
        {
            return "Doctors gather corpses from locations (using various abilities), for storage in corpseroot fields. They can raise the dead once you feel you have enough for a army to attack the human nations.";
        }

        public override string getTitleM()
        {
            return "Doctor";
        }

        public override string getTitleF()
        {
            return "Doctor";
        }

        public override string getDesc()
        {
            return "Necromantic Doctors pose as plague doctors to gain access to corpses, which they load onto their wagon for disposal. Once sufficient are gathered, they can raise armies of the dead.";

        }
    }
}
