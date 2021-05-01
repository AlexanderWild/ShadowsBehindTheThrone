using System.Collections.Generic;

namespace Assets.Code
{
    public class Task_DefendHomeland : Task
    {

        public override string getShort()
        {
            return "Defending the Homeland";
        }
        public override string getLong()
        {
            return "This army intends to hold the border against invaders.";
        }

        public override void turnTick(Unit unit)
        {

            HashSet<Location> closed = new HashSet<Location>();
            HashSet<Location> open = new HashSet<Location>();
            HashSet<Location> open2 = new HashSet<Location>();

            closed.Add(unit.location);
            open.Add(unit.location);

            int c = 0;
            Location target = null;
            for (int tries = 0; tries < 128; tries++)
            {
                open2.Clear();
                foreach (Location loc in open)
                {
                    foreach (Location l2 in loc.getNeighbours())
                    {
                        if (!closed.Contains(l2))
                        {
                            closed.Add(l2);
                            open2.Add(l2);
                        }
                        if (l2.soc != null && l2.soc.getRel(unit.society).state == DipRel.dipState.war)
                        {
                            c += 1;
                            if (Eleven.random.Next(c) == 0)
                            {
                                target = loc;
                            }
                        }
                        foreach (Unit u2 in l2.units)
                        {
                            if (u2.isMilitary && unit.hostileTo(u2))
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    target = loc;
                                }
                            }
                        }
                    }
                }
                if (target != null) { break; }//Now found the closest unit set
                open = open2;
                open2 = new HashSet<Location>();
            }

            if (unit.society == null || (unit.society.isAtWar() == false)) { 
                unit.task = null;
            }
            else if (target != null && target != unit.location)
            {
                Location[] locations = unit.location.map.getPathTo(unit.location, target);
                if (locations == null || locations.Length < 2) { unit.task = null; return; }
                unit.location.map.adjacentMoveTo(unit, locations[1]);
            }
        }
    }
}