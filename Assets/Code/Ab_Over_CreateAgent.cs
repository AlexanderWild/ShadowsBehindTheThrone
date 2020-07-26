using UnityEngine;

using System;
using System.Globalization;

namespace Assets.Code
{
    public class Ab_Over_CreateAgent: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Unit agent = new Unit_Vampire(hex.location, map.soc_dark);
            map.world.prefabStore.popImgMsg(
                "You draw upon the creatures of darkness, and choose one to serve as your instrument in this world",
                map.world.wordStore.lookup("ABILITY_CREATE_AGENT"));

            agent.person = new Person(map.soc_dark);
            agent.person.state = Person.personState.enthralledAgent;
            agent.person.unit = agent;
            map.units.Add(agent);

            Evidence ev = new Evidence(map.turn);
            ev.pointsTo = agent;
            ev.weight = 0.66;
            agent.location.evidence.Add(ev);

            agent.task = null;
            agent.movesTaken += 1;
        }


        public override bool castable(Map map, Person person)
        {
            return false;
        }

        public override bool castable(Map map, Unit unit)
        {
            return false;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_enthrallUnitCooldown;
        }

        public override bool castable(Map map, Hex hex)
        {
            int n = 0;
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled()) { n += 1; }
            }
            return hex.location != null && n < map.param.units_maxEnthralled;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_enthrallUnitCost;
        }
        public override string getDesc()
        {
            return "Create an agent for to use, as long as you are not at your maximum agent count."
                + "\n[Max " + World.staticMap.param.units_maxEnthralled + "]";
        }

        public override string getName()
        {
            return "Create Agent";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}