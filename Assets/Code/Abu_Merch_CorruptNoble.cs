using UnityEngine;


namespace Assets.Code
{
    public class Abu_Merch_CorruptNoble: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            ((Unit_Merchant)u).cash -= map.param.unit_merchant_corruptGoldCost;

            Person person = u.location.person();
            for (int i = 0; i < person.traits.Count; i++)
            {
                if (person.traits[i].groupCode == Trait.CODE_POLITICS)
                {
                    foreach (Trait t in map.globalist.allTraits)
                    {
                        if (t is Trait_Political_Corrupt)
                        {
                            person.traits[i] = t;
                        }
                    }
                }
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " invests their wealth in causing " + u.location.person().getFullName() + " to grow decadent, selfish and egotistical. Through flatteries,"
                + " lavish parties and carefully worded arguments they convince " + u.location.person().getFullName() + " to reject all duty to their fellow human. They " +
                " now have the trait: " + u.location.person().traits[0].name
                , u.location.map.world.wordStore.lookup("ABILITY_MERCHANT_CORRUPT_NOBLE"), 1);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc is Society == false) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u is Unit_Merchant == false) { return false; }
            if (((Unit_Merchant)u).cash < World.staticMap.param.unit_merchant_corruptGoldCost) { return false; }
            foreach (Trait t in u.location.person().traits)
            {
                if (t is Trait_Political_Corrupt) { return false; }
            }
            return true;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }
        public override string specialCost()
        {
            return World.staticMap.param.unit_merchant_corruptGoldCost + " Wealth";
        }

        public override string getDesc()
        {
            return "Spends wealth. Increases liking of local noble towards your merchant, as well as increasing the noble's prestige and resupplying the military of the nation if depleted."
                + "\n[Requires a human settlement wtih a noble and at least " + World.staticMap.param.unit_merchant_corruptGoldCost + " wealth]";
        }

        public override string getName()
        {
            return "Corrupt Noble";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_coins;
        }
    }
}