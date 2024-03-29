﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public abstract class Property_Prototype
    {
        public string name;

        public int baseCharge = 10;
        public bool decaysOverTime = false;
        public int prestigeChange = 0;
        public enum stackStyleEnum { NONE, ADD_CHARGE, TO_MAX_CHARGE };
        public stackStyleEnum stackStyle = stackStyleEnum.NONE;

        public double milCapAdd;
        public double informationAvailabilityMult = 1;
        public int securityIncrease = 0;
        public double plagueThreat = 0;
        public bool isDisease = false;
        public bool isImmunity = false;

        public Property_Prototype(Map map, string name)
        {
            this.name = name;
        }
        public virtual void turnTick(Property p, Location location)
        {
        }

        public abstract Sprite getSprite(World world);

        public static void loadProperties(Map map)
        {
            Property_Prototype proto = new Pr_MilitaryAid(map, "Military Aid");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_MilitaryAidOutgoing(map, "Military Aid Outgoing");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_InformationBlock(map, "Information Blackout");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_DeepOneCult(map, "Cult of the Deep");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_DeepOneAbyssalSirens(map, "Abyssal Sirens");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_RecentHumanBattle(map, "Recent Human Battle");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_DeepOneMadness(map, "Haunting Song");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_EmptyGraves(map, "Empty Graves");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_ForgottenSecret(map, "Forgotten Secret");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_MajorSecurityBoost(map, "Major Security Boost");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_MinorSecurityBoost(map, "Minor Security Boost");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_Lockdown(map, "Lockdown");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_Pumpkin(map, "Pumpkin");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_RedDeath(map, "Red Death");
            map.globalist.allProperties.Add(proto);
            proto.isDisease = true;
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_RedDeathImmunity(map, "Red Death Immunity");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_Quarantine(map, "Quarantine");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_MedicalAid(map, "Medical Aid");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_Fog_Source(map, "Well of Fog");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_Fog_Pinned(map, "Trapped Fog");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_Blizzard(map, "Blizzard");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_RottingSickness(map, "Rotting Sickness");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);

            proto = new Pr_RottingSicknessImmunity(map, "Rotting Sickness Immunity");
            map.globalist.allProperties.Add(proto);
            map.globalist.propertyMap.Add(proto.name, proto);
        }

        internal abstract string getDescription();

        public virtual void endProperty(Location location, Property p)
        {
        }
    }
}