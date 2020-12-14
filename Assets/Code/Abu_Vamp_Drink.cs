using UnityEngine;


namespace Assets.Code
{
    public class Abu_Vamp_Drink: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Vampire vamp = (Unit_Vampire)u;

            vamp.blood = vamp.maxBlood;

            double amount = map.param.unit_minorEvidence;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " drinks from the blood of the living. They leave behind evidence of strength " +(int)(100*amount) + "%",
                u.location.map.world.wordStore.lookup("ABILITY_VAMP_DRINK"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.person().getRelation(u.person.index).getLiking() < map.param.unit_vampire_drinkReqLiking) { return false; }
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
            return "Minor Evidence";
        }

        public override string getDesc()
        {
            return "Drinks the blood of the living, filling up the vampire's blood reserves. Leaves behind minor evidence."
                + "\n[Requires a location with a noble who trusts the vampire (liking no lower than " + World.staticMap.param.unit_vampire_drinkReqLiking + ")]";
        }

        public override string getName()
        {
            return "Drink";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_vampire;
        }
    }
}