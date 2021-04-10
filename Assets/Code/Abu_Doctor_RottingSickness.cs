using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_RottingSickness: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Property.addProperty(map, u.location, "Rotting Sickness");
            u.location.map.world.prefabStore.popImgMsg("The first victim of this wave of the Rotting Sickness is in " + u.location.getName() + "."
                + " The disease will spread between cities (unless quarantined or immune), and will decrease the city's population by 1 every 2 turns.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_ROTTING_SICKNESS"));

            if (u.location.soc != null && u.location.soc is Society)
            {
                Society soc = (Society)u.location.soc;
                soc.crisisPlague = "Plagues have appeared in our lands";
                soc.crisisPlagueLong = "Diseases are spreading throughout our lands";
            }

            if (u is Unit_NecroDoctor doc){
                doc.corpses -= map.param.unit_doctor_rottingDeadReq;
            }

            double amount = map.param.unit_majorEvidence;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u is Unit_NecroDoctor doc)
            {
                if (doc.corpses >= map.param.unit_doctor_rottingDeadReq)
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
            return World.staticMap.param.ability_redDeath_originCooldown;
        }

        public override string getDesc()
        {
            return "Starts an outbreak of rotting sickness, which will spread between linked human settlements, reducing population over time. Weaker than the Red Death, but will decrease army strengths"
                + "\n[Requires a human settlement in a society and at least " + World.staticMap.param.unit_doctor_rottingDeadReq + "]";
        }

        public override string specialCost()
        {
            return "Major Evidence";
        }
        public override string getName()
        {
            return "Rotting Sickness";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_doctor;
        }
    }
}