using UnityEngine;


namespace Assets.Code
{
    public class Abu_Red_LethalStrain: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            if (u.location.soc != null && u.location.soc is Society)
            {
                Society soc = (Society)u.location.soc;
                soc.crisisPlague = "Plagues have appeared in our landsare increasing in ferocity";
                soc.crisisPlagueLong = "The diseases in our land have increased in intensity, and are devastating our populations";
            }

            double amount = map.param.unit_majorEvidence;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

            u.task = new Task_LethalStrain();

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins increasing the diseases in and around " + u.location.getName() + "."
                + " Every turn they spend will drop the human population in this location and all connected which have a disease.",
                u.location.map.world.wordStore.lookup("ABILITY_RED_LETHAL_STRAIN"), 5);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.settlement is SettlementHuman == false) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.settlement.infiltration <= 0.99) { return false; }
            foreach (Property pr in u.location.properties)
            {
                if (pr.proto.isDisease)
                {
                    return true;
                }
            }
            return false;
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
            return "Increases the devastation of the disease in location and neighbouring locations with disease. Population damage is inflicted per turn as long as the agent remains stationary."
                + "\n[Requires a human settlement with disease and infiltration 100%]";
        }

        public override string specialCost()
        {
            return "Major Evidence";
        }
        public override string getName()
        {
            return "Lethal Strain";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_redDeath;
        }
    }
}