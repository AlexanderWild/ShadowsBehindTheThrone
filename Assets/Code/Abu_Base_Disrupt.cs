using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_Disrupt: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {

            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = 0.1;
            u.location.evidence.Add(e2);

            int n = 0;
            foreach (Unit u2 in u.location.units)
            {
                if (u2 != u)
                {
                    u2.task = new Task_Disrupted();
                    n += 1;
                }
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " distrupts the activities of " + n + " other units in " + u.location.getName() + ", they will need time before they can act again.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_DISRUPT"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.units.Count < 2) { return false; }
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
            return World.staticMap.param.unit_disruptCooldown;
        }

        public override string getDesc()
        {
            return "Cancels all the actions of units in this location, disrupting them for " + World.staticMap.param.unit_disruptDuration + " turns. Leaves 10% evidence."
                + "\n[Requires a unit in the location]";
        }

        public override string getName()
        {
            return "Disrupt";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}