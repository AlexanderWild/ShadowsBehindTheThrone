using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fash_SpreadFear: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            

            ThreatItem item = hex.location.person().getGreatestThreat();
            if (item == null) { return; }
            
            string changes = "";
            foreach (Location loc in hex.location.getNeighbours())
            {
                if (loc.person() != null)
                {
                    Person other = loc.person();
                    foreach (ThreatItem t2 in other.threatEvaluations)
                    {
                        if (t2.isSame(item))
                        {
                            double delta = item.threat - t2.threat;
                            if (delta <= 0) { continue; }
                            double liking = other.getRelation(hex.location.person()).getLiking();
                            if (liking <= 0) { continue; }
                            liking /= 100;

                            double prev = t2.threat;
                            delta *= liking;
                            if (t2.threat + delta > 200)
                            {
                                delta = 200 - t2.threat;
                            }
                            t2.threat += delta;
                            t2.temporaryDread += delta;

                            changes += other.getFullName() + " " + (int)(prev) + " -> " + (int)(t2.threat) + "; ";
                        }
                    }
                }
            }
            if (changes.Length > 0)
            {
                changes = changes.Substring(0, changes.Length - 2);//Strip the last separator
            }

            string msgs = "You spread " + hex.location.person().getFullName() + "'s fears of " + item.getTitle() + " to any neighbour who will listen. ";
            if (changes.Length > 0)
            {
                msgs += "\n" + changes;
            }
            else
            {
                msgs += "\nBut there are none.";
            }
            map.world.prefabStore.popImgMsg(
                msgs,
                map.world.wordStore.lookup("ABILITY_SPREAD_FEAR"));
        }

        public override bool castable(Map map, Person person)
        {
            return false;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location.person() == null) { return false; }
            return castable(map, hex.location.person());
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_spreadFearCooldown;
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Causes the neighbours of a noble to gain dread towards that noble's greatest fear, proportional to how much they like the noble and how much that noble's fear exceeds theirs."
                + "\n[Requires a location belonging to a noble]";
        }

        public override string getName()
        {
            return "Spread Fear";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}