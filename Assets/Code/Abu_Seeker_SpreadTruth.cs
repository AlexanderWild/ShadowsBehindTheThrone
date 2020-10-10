using UnityEngine;


namespace Assets.Code
{
    public class Abu_Seeker_SpreadTruth: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            double amount = 1;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

            u.location.person().shadow += map.param.unit_seeker_truthShadow;
            if (u.location.person().shadow > 1)
            {
                u.location.person().shadow = 1;
            }
            u.location.person().sanity = 0;
            u.location.person().goInsane();

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " spreads the truth to " + u.location.person().getFullName() + ", driving them insane, and spreading your shadow to them."+
                " Their shadow is now " + (int)(100*u.location.person().shadow) + "%. They are " + u.location.person().madness.name +".",
                u.location.map.world.wordStore.lookup("ABILITY_SEEKER_SPREAD_TRUTH"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.person() == null) { return false; }
            if (u is Unit_Seeker == false) { return false; }
            Unit_Seeker seeker = (Unit_Seeker)u;
            return seeker.knowsTruth;
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
            return "Tells the terrible truth to the local noble, driving them insane and increasing their shadow by " + (int)(100*World.staticMap.param.unit_seeker_truthShadow) +". Leaves major evidence."
                + "\n[Requires a location with a noble, and for the seeker to know the truth]";
        }

        public override string getName()
        {
            return "Spread Truth";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_seeker;
        }
    }
}