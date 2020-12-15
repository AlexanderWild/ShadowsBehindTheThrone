using UnityEngine;


namespace Assets.Code
{
    public class Abu_Merch_SpendWealth: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Task_SpendWealth task = new Task_SpendWealth();
            u.task = task;
            
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins spending their wealth, in lavish banquets, gifts and fine silks. They enjoy the company of the local noble, who" +
                " benefits from increased taxes and excellent festivals. The economy booms, to be spent on swords or favours."
                , u.location.map.world.wordStore.lookup("ABILITY_UNIT_SPEND_WEALTH"), 1);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc is Society == false) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u is Unit_Merchant == false) { return false; }
            if (((Unit_Merchant)u).cash <= 0) { return false; }
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
            return " ";
        }

        public override string getDesc()
        {
            return "Spends wealth. Increases liking of local noble towards your merchant, as well as increasing the noble's prestige and resupplying the military of the nation if depleted."
                + "\n[Requires a human settlement wtih a noble]";
        }

        public override string getName()
        {
            return "Spend Wealth";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_coins;
        }
    }
}