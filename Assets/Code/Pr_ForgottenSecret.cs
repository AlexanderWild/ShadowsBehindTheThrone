using UnityEngine;

namespace Assets.Code
{
    public class Pr_ForgottenSecret : Property_Prototype
    {
        
        public Pr_ForgottenSecret(Map map,string name) : base(map,name)
        {
            this.decaysOverTime = false;
            this.stackStyle = stackStyleEnum.NONE;
        }

        public override void turnTick(Property p,Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_letter;
        }

        internal override string getDescription()
        {
            return "This location has a forgotten secret. A seeker could uncover it, to piece together a fragment of the dark truth.";
        }
    }
}
