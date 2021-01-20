using UnityEngine;

namespace Assets.Code
{
    public class Pr_MedicalAid : Property_Prototype
    {
        
        public Pr_MedicalAid(Map map,string name) : base(map,name)
        {
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.baseCharge = map.param.society_medAidDuration;
        }

        public override void turnTick(Property p,Location location)
        {
            foreach (Property p2 in location.properties){
                if (p2.proto.isDisease && p2.charge > 0)
                {
                    p2.charge -= 1;
                }
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_medicalAid;
        }

        internal override string getDescription()
        {
            return "This location is receiving medical aid. Diseases in this location have their durations reduced each turn, so they will expire far faster than usual.";
        }
    }
}
