using UnityEngine;

namespace Assets.Code
{
    public class Pr_MajorSecurityBoost : Property_Prototype
    {
        
        public Pr_MajorSecurityBoost(Map map,string name) : base(map,name)
        {
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.securityIncrease = map.param.society_securityBuffMajor;
            this.baseCharge = map.param.society_securityBuffDuration;
        }

        public override void turnTick(Property p,Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_securityMajor;
        }

        internal override string getDescription()
        {
            return "This location has increased its security level in a major way. Infiltration actions in this area will have far lower effect.";
        }
    }
}
