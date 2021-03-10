using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Saviour : Unit
    {

        public bool linkedFates = true;

        public Unit_Saviour(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 5;
            hp = 5;

            abilities.Add(new Abu_Save_Invasion());
            abilities.Add(new Abu_Base_Infiltrate());
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
            return world.textureStore.unit_saviour;
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.9f, 0.9f, 0.3f);
        }

        public override string specialInfo()
        {
            if (linkedFates)
            {
                return "Affecting Enthralled Noble's Prestige";
            }
            else
            {
                return "Not currently affecting noble prestige";
            }
        }

        public override string specialInfoLong()
        {
            return "The Saviour can choose to associate themselves with your enthralled noble's political career. This will grant your enthralled noble additional " +
                "prestige for every noble who likes The Saviour, but can cost them if they become suspicious of this agent.";
        }

        public override string getTitleM()
        {
            return "Saviour";
        }

        public override string getTitleF()
        {
            return "Saviour";
        }

        public override string getDesc()
        {
            return "The Saviour is a deceiver, who can 'save' a nation from either invasion by forces you control or from a plague, causing nearby nobles to love them, letting The Saviour infiltrate with ease.";

        }
    }
}
