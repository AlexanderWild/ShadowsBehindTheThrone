using UnityEngine;


namespace Assets.Code
{
    public class Abu_Merch_SellCargo: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Task_SellCargo task = new Task_SellCargo();
            u.task = task;

            Unit_Merchant merchant = (Unit_Merchant)u;
            merchant.hasSoldCargo = true;

            float profit = task.getSaleValue(u.society, u.location);
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins selling cargo, generating wealth. Every turn they will sell 10% of their cargo, with a proft of " +((int)(100 * profit))
                + "%. Metropoles, cities and towns give greater profit, and selling in your home nation generates less than selling in foreign lands."
                , u.location.map.world.wordStore.lookup("ABILITY_UNIT_SELL_CARGO"), 1);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u is Unit_Merchant == false) { return false; }
            if (((Unit_Merchant)u).cash >= 100) { return false; }
            if (((Unit_Merchant)u).cargo <= 0) { return false; }
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
            return "Begins selling your cargo. Profit is generated per turn, with more profit from foreign nations, as well as larger metropoles/cities/towns"
                + "\n[Requires a human settlement]";
        }

        public override string getName()
        {
            return "Sell Cargo";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_coins;
        }
    }
}