using System;
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
        public enum stackStyleEnum { NONE,ADD_CHARGE,TO_MAX_CHARGE};
        public stackStyleEnum stackStyle = stackStyleEnum.NONE;

        public double milCapAdd;
        public double informationAvailabilityMult = 1;

        public Property_Prototype(Map map,string name)
        {
            this.name = name;
        }
        public virtual void turnTick(Location location)
        {
        }

        public abstract Sprite getSprite(World world);

        public static void loadProperties(Map map)
        {
            Property_Prototype proto = new Pr_MilitaryAid(map,"Military Aid");
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
        }

        internal abstract string getDescription();
    }
}
