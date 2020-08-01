using UnityEngine;


namespace Assets.Code
{
    public class SG_Undead : SocialGroup
    {
        public bool awakened = false;

        public SG_Undead(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            color = new Color(
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            color2 = new Color(
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            this.setName("The Dead");

            this.threat_mult = map.param.dark_evilThreatMult;
        }

        public override string getTypeName()
        {
            if (awakened)
            {
                return "Living Dead";
            }
            return "Sleeping Dead";
        }
        public override string getTypeDesc()
        {
            if (awakened)
            {
                return "An army of the dead, swarming from the grave to bring an end to all human life.";
            }
                return "Human dead, awaiting their return from the grave to serve as an army of death, attacking all life.";
        }

        public override void turnTick()
        {
            base.turnTick();
        }

        public override bool isDark()
        {
            return true;
        }

        public override void takeLocationFromOther(SocialGroup def, Location taken)
        {
            base.takeLocationFromOther(def, taken);

            if (def is Society && taken.settlement != null)
            {
                taken.settlement = new Set_Ruins(taken);
            }
        }
    }
}