using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_RobGraves: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_NecroDoctor doc = (Unit_NecroDoctor)u;

            Property.addProperty(map, u.location, "Empty Graves");


            double amount = map.param.unit_minorEvidence;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

            int security = 0;
            if (u.location.settlement != null) { security = u.location.settlement.getSecurity(new System.Collections.Generic.List<ReasonMsg>()); }
            int nStolen = map.param.unit_doctor_lootBodiesAmount - security;
            if (nStolen < 1) { nStolen = 1; }
            doc.corpses += nStolen;
            if (doc.corpses > doc.maxCorpses) { doc.corpses = doc.maxCorpses; }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " loots " + nStolen + " bodies from " + u.location.getName() + " (Maximum is " + map.param.unit_doctor_lootBodiesAmount
                + " location security is " + security + "). They have " + doc.corpses + "/" + doc.maxCorpses 
                + " corpses, these should be carried to a corpseroot field before they decay.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_ROB_GRAVES"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.settlement == null) { return false; }

            foreach (Property prop in u.location.properties)
            {
                if (prop.proto is Pr_EmptyGraves)
                {
                    return false;
                }
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

        public override string specialCost()
        {
            return "Minor Evidence";
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Steals the bodies from the local graveyard (" + World.staticMap.param.unit_doctor_lootBodiesAmount  
                +" bodies MINUS SECURITY LEVEL). They can be carried to a corpseroot field to grow the army of the dead. Leaves evidence behind."
                + "\n[Requires a human settlement which hasn't been looted recently]";
        }

        public override string getName()
        {
            return "Rob Graves";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_corpseroot;
        }
    }
}