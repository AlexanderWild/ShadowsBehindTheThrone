using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_LootBattlefield: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_NecroDoctor doc = (Unit_NecroDoctor)u;

            Property del = null;
            foreach (Property p in u.location.properties)
            {
                if (p.proto is Pr_RecentHumanBattle)
                {
                    del = p;
                }
            }

            double amount = 0.5;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

            if (del == null)
            {
                map.world.prefabStore.popMsg("No battlefield to loot");
                return;
            }


            doc.corpses = doc.maxCorpses;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " loots the bodies from the battlefield, piling high worthy corpses, their dead hands still gripping their weapons of war."
                + "\nThese should be carried to one or more corpseroot patches to preserve them until their next battle.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_ROB_BATTLEFIELD"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.settlement == null) { return false; }

            foreach (Property prop in u.location.properties)
            {
                if (prop.proto is Pr_RecentHumanBattle)
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
            return "Steals the bodies from a recent human battlefield. Fills the corpse cart completely. They can be carried to a corpseroot field to grow the army of the dead. Leaves evidence behind."
                + "\n[Requires a site of a recent human battle]";
        }

        public override string getName()
        {
            return "Desacrate Battlefield";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_corpseroot;
        }
    }
}