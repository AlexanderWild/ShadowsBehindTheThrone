using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_LegalRemoval: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_NecroDoctor doc = (Unit_NecroDoctor)u;

            Property.addProperty(map, u.location, "Empty Graves");


            doc.corpses += map.param.unit_doctor_lootBodiesAmount;
            if (doc.corpses > doc.maxCorpses) { doc.corpses = doc.maxCorpses; }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " removes the bodies from " + u.location.getName() + ", using official documents (forged or otherwise) to justify their actions." +
                " Therefore no evidence is left behind. They have " + doc.corpses + "/" + doc.maxCorpses 
                + " corpses, these should be carried to a corpseroot field before they decay.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_LEGAL_REMOVAL"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.settlement.infiltration < World.staticMap.param.unit_doctor_legalInfiltrationReq) { return false; }

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
            return "Uses official paperwork of an infiltrated location to legally exhume bodies, piling them on the cart (" + World.staticMap.param.unit_doctor_lootBodiesAmount  
                +" bodies). They can be carried to a corpseroot field to grow the army of the dead."
                + "\n[Requires a human settlement which hasn't been looted recently and infiltration > " + (int)(100*World.staticMap.param.unit_doctor_legalInfiltrationReq) + "%]";
        }

        public override string getName()
        {
            return "Legal Removal";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_corpseroot;
        }
    }
}