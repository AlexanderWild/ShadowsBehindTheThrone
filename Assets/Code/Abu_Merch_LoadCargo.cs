using UnityEngine;


namespace Assets.Code
{
    public class Abu_Merch_LoadCargo: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_LoadCargo();

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins loading cargo, ready to be sold for profit in distant towns.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_LOAD_CARGO"),1);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.soc != u.society) { return false; }
            if (u is Unit_Merchant == false) { return false; }
            if (((Unit_Merchant)u).cargo >= 100) { return false; }
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

        public override string getDesc()
        {
            return "Begins loading cargo into your mechant's reserves. You load twice as fast in a metropole, city, town or village."
                + "\n[Requires a location in your agent's nation with a settlement]";
        }

        public override string getName()
        {
            return "Load Cargo";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_coins;
        }
    }
}