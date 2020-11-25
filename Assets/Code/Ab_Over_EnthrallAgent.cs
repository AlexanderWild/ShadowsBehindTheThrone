using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_EnthrallAgent: Ability
    {
        public override void castInner(Map map, Unit other)
        {

            map.overmind.availableEnthrallments -= 1;

            string addMsg = "\n\nYou have " + map.overmind.availableEnthrallments + " enthrallment uses remaining.You will regain one every " + 
                map.param.overmind_enthrallmentUseRegainPeriod + " turns if you are below maximum.";

            map.world.prefabStore.popImgMsg(
                "You take " + other.getName() + " under your control. You may vote through them, convince others using their goodwill towards them, and use abilities relating to noble enthralled. " + addMsg,
                map.world.wordStore.lookup("ABILITY_ENTHRALL_AGENT"));

            other.person.state = Person.personState.enthralledAgent;
            Evidence ev = new Evidence(map.turn);
            ev.pointsTo = other;
            ev.weight = 0.66;
            other.location.evidence.Add(ev);

            other.task = null;
            other.movesTaken += 1;
            map.overmind.computeEnthralled();
        }
        public override bool castable(Map map, Person person)
        {
            if (person.unit == null) { return false; }
            if (person.state == Person.personState.enthralledAgent) { return false; }
            if (map.overmind.availableEnthrallments < 1) { return false; }
            if (map.overmind.nEnthralled >= map.overmind.maxEnthralled) { return false; }

            int nOwned = 0;
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled()) { nOwned += 1; }
            }
            return nOwned < map.param.overmind_maxEnthralled;
        }

        public override bool castable(Map map, Unit unit)
        {
            if (unit.person == null) { return false; }
            if (unit is Unit_Simple_Paladin) { return false; }
            if (unit is Unit_Investigator && ((Unit_Investigator)unit).state == Unit_Investigator.unitState.paladin) { return false; }
            return castable(map,unit.person);
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_enthrallUnitCooldown;
        }

        public override bool castable(Map map, Hex hex)
        {
            return false;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_enthrallUnitCost;
        }
        public override string getDesc()
        {
            return "Enthrall an agent to turn it to your control. Places a piece of evidence pointing to the guilt of your new agent."
                + "\n[Requires a unit with a character. Max " + World.staticMap.param.overmind_maxEnthralled + " enthralled. You require an enthrallment use]";
        }

        public override string getName()
        {
            return "Enthrall Unit";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}