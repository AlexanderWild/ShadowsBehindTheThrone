using UnityEngine;


namespace Assets.Code
{
    public class Abu_Heir_Sacrifice: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            if (u.location.person() != null)
            {
                u.location.map.world.prefabStore.popImgMsg(u.getName() + " sacrifices the corrupted soul of " + u.location.person().getFullName() + ", trading their spirit, blood and body for dark power." +
                    " You gain " + u.location.map.param.unit_heir_saccPowerGain + " power and refill all enthrallment usages.",
                    u.location.map.world.wordStore.lookup("ABILITY_UNIT_SACRIFICE_NOBLE"), 4);

                u.location.person().die("Killed in blasphemous ritual to empower the darkness", true);
                u.location.map.overmind.availableEnthrallments = u.location.map.param.overmind_maxEnthralled;
                u.location.map.overmind.power += u.location.map.param.unit_heir_saccPowerGain;
            }
            else
            {
                map.world.prefabStore.popMsg("No noble present in this location.");
                return;
            }

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.person == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.person().state != Person.personState.broken) { return false; }
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
            return "Sacrifices a broken noble to gain " + World.staticMap.param.unit_heir_saccPowerGain + " power and refill all enthrallment uses."
                + "\n[Requires a broken noble]";
        }

        public override string getName()
        {
            return "Sacrifice Sinner";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}