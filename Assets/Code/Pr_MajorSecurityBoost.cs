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
        }

        public override void turnTick(Property p,Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_forgottenSecret;
        }

        internal override string getDescription()
        {
            return "This location has increased its security level in a major way.";
        }
    }
}
