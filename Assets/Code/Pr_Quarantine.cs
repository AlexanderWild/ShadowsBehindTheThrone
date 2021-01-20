using UnityEngine;

namespace Assets.Code
{
    public class Pr_Quarantine : Property_Prototype
    {
        
        public Pr_Quarantine(Map map,string name) : base(map,name)
        {
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.baseCharge = map.param.society_quarantineDuration;
        }

        public override void turnTick(Property p,Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_quarantine;
        }

        internal override string getDescription()
        {
            return "This location is under quarantine. The probability of disease spreading out of this location to a neighbouring one is halved.";
        }
    }
}
