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

        public override void turnTick(Property p,Location location)
        {
            if (location.settlement == null) { return; }
            if (location.soc == null || (location.soc is Society == false)) { return; }

            int c = 0;
            Unit target = null;
            foreach (Unit u in location.map.units)
            {
                if (u is Unit_Fishman)
                {
                    if (u.hp < u.maxHp)
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            target = u;
                        }
                    }
                }
            }

            if (target != null)
            {
                target.hp += location.map.param.ability_fishmanCultMilRegen;
                if (target.hp > target.maxHp) { target.hp = target.maxHp; }

                target.society.temporaryThreat += location.map.param.ability_fishmanCultTempThreat;
                if (location.person() != null)
                {
                    foreach (ThreatItem item in location.person().threatEvaluations)
                    {
                        if (item.group == target.society && item.temporaryDread < 100)
                        {
                            item.temporaryDread += location.map.param.ability_fishmanCultDread;
                        }
                    }
                }
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_fishmen;
        }

        internal override string getDescription()
        {
            return "The people in this area are drawn to the sea. Fishermen report constant song, drawing them towards the deep. Slowly resupplies your Deep One raiders with military force, but adds temporary threat"
                + " to this colony, and causes the local noble (if there is one) to dread the Deep Ones (adding dread to the noble's threat estimates).";
        }
    }
}
