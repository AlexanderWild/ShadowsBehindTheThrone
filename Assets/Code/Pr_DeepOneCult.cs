using UnityEngine;

namespace Assets.Code
{
    public class Pr_DeepOneCult : Property_Prototype
    {
        
        public Pr_DeepOneCult(Map map,string name) : base(map,name)
        {
            this.informationAvailabilityMult = 0.5;
            this.baseCharge = map.param.ability_FishmanCultDuration;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override void turnTick(Location location)
        {
            if (location.settlement == null) { return; }
            if (location.soc == null || (location.soc is Society == false)) { return; }

            SG_Fishmen target = null;
            foreach (SocialGroup sg in location.map.socialGroups)
            {
                if (sg is SG_Fishmen)
                {
                    target = (SG_Fishmen)sg;
                }
            }

            target.currentMilitary += location.map.param.ability_fishmanCultMilRegen;
            target.temporaryThreat += location.map.param.ability_fishmanCultTempThreat;

            if (location.person() != null)
            {
                foreach (ThreatItem item in location.person().threatEvaluations)
                {
                    if (item.group == target && item.temporaryDread < 100)
                    {
                        item.temporaryDread += location.map.param.ability_fishmanCultDread;
                    }
                }
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_lookingGlass;
        }

        internal override string getDescription()
        {
            return "The people in this area are drawn to the sea. Fishermen report constant song, drawing them towards the deep. Slowly adds Deep Ones to your colony, but adds temporary threat"
                + " to this colony, and causes the local noble (if there is one) to dread the Deep Ones (adding threat perception).";
        }
    }
}
