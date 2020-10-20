using UnityEngine;

namespace Assets.Code
{
    public class Pr_Lockdown : Property_Prototype
    {
        
        public Pr_Lockdown(Map map,string name) : base(map,name)
        {
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            //this.prestigeChange = map.param.society_lockdownPrestigeHit;
            this.baseCharge = map.param.society_lockdownBuffDuration;
        }

        public override void turnTick(Property p,Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_securityLockdown;
        }

        internal override string getDescription()
        {
            return "This location is fully locked down. Guards patrol, curfew is in effect. You may not take agent actions here, and no tasks may be accomplished until the end of the lockdown.";
        }
    }
}
