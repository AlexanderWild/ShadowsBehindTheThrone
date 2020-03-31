using UnityEngine;

namespace Assets.Code
{
    public class Pr_DeepOneAbyssalSirens : Property_Prototype
    {
        
        public Pr_DeepOneAbyssalSirens(Map map,string name) : base(map,name)
        {
            this.baseCharge = map.param.ability_FishmanCultDuration;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override void turnTick(Property p,Location location)
        {
            if (location.settlement == null) { return; }
            if (location.person() == null) { return; }
            if (location.person().state != Person.personState.normal) { return; }
            if (location.person().sanity != 0) { return; }
            location.person().shadow += location.map.param.ability_fishmanHauntingAbyssalSirensShadowPerTurn;
            if (location.person().shadow > 1)
            {
                location.person().shadow = 1;
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_fishmen;
        }

        internal override string getDescription()
        {
            return "The Deep One cult here is drawing on the madness caused by the song to enshadow the population and noble. If the noble here is insane, they gain " 
                + (int)(100*World.staticMap.param.ability_fishmanHauntingAbyssalSirensShadowPerTurn) + "% shadow per turn.";
        }
    }
}
