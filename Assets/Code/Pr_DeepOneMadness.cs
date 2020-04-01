using UnityEngine;

namespace Assets.Code
{
    public class Pr_DeepOneMadness : Property_Prototype
    {
        
        public Pr_DeepOneMadness(Map map,string name) : base(map,name)
        {
            this.baseCharge = map.param.ability_FishmanMadnessDuration;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override void turnTick(Property p,Location location)
        {
            if (location.settlement == null) { return; }
            if (location.person() == null) { return; }
            if (location.person().sanity == 0) { return; }
            if (Eleven.random.Next(2) == 0)
            {
                location.person().sanity -= 1;
            }
            if (location.person().sanity < 0)
            {
                location.person().sanity = 0;
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_fishmen;
        }

        internal override string getDescription()
        {
            return "The song of the Deep Ones is driving the population into madness. Each turn there is a 50% chance the noble will lose a point of sanity.";
        }
    }
}
