using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Zeit
    {
        public string eventDesc;
        public List<ZeitOpt> opts = new List<ZeitOpt>();
        public bool exploitedByPlayer;
        public bool exploitedBySociety;
        public int turnMade;
        public ThreatItem threatItem;

        //Severity ascends. Irrelevant is 0 (no news), foreign action is 1, battle is 2, settlement lost is 3, city lost if 4
        public int severity;

        //How much the political sliders will move based on this
        public float weight = 0.1f;

        public zeitType type = zeitType.ROUTINE;
        public enum zeitType { ROUTINE, BATTLE, THREAT, LOST_SETTLEMENT, TOOK_SETTLEMENT,INVESTIGATION ,GEOPOLITICS};

        public Zeit(Map map)
        {
            this.turnMade = map.turn;
        }

        /*
        public static Zeit zeitParahumans(Map map)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.THREAT;
            z.severity = 2;
            z.weight = 0.5f;

            z.eventDesc = "Parahumans, twisted and deformed creatures which make a mockery of the human form, have begun to flood our streets and cities. Something must be done.";

            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Honestly, despite their nature, they continue to pay taxes with regularity. We have other issues to attend to.";
                opt.reduceTargetThreat = true;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Other issues must now take a back seat. We must rally the people against this corruption.";
                opt.amplifyTargetThreat = true;
                opt.reduceOtherThreats = true;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We must devote our resources to this threat, rather than waste time worrying about the world outside our borders.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }

        public static Zeit zeitThreatIsWeaker(Map map, ThreatItem threat)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.THREAT;
            z.threatItem = threat;
            z.severity = 1;
            z.weight = 0.3f;

            z.eventDesc = "We, collectively, estimate that " + threat.group.getName() + " is our greatest threat. They are noticable weaker than we are, militarily.";
            
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "They clearly are not such a threat. We should take this time to evaluate other threats to our safety.";
                opt.reduceTargetThreat = true;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We are unchallenged, and can take whatever we please with force of superior arms.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = 1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitThreatIsStronger(Map map, ThreatItem threat)
        {
            Zeit z = new Zeit(map);
            z.threatItem = threat;
            z.type = zeitType.THREAT;
            z.severity = 2;
            z.weight = 0.5f;

            z.eventDesc = "We, collectively, estimate that " + threat.group.getName() + " is our greatest threat. It is very concerning that they possess a greater military force than us.";
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Business as usual, sadly. There isn't much to be done, other than hope we are spared by fate.";
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "All other threats are secondary, we must focus on this one.";
                opt.amplifyTargetThreat = true;
                opt.reduceOtherThreats = true;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We should look to our defences, and possibly seek alliances with any neighbours who feel the same as we do.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitInBattle(Map map, Unit u, Unit other)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.BATTLE;
            z.severity = 1;
            z.weight = 0.3f;

            z.eventDesc = "Our military forces have engaged the enemy, as " + u.getName() + " clashed with " + other.getName() + " in combat.";
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "Clearly this means we must invest in our armed forces, to gain whatever foothold we can in our present conflict.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "The outside world is hostile, and full of danger, we must look inwards to protect ourselves.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "The outside world is hostile, and full of danger, we must look outwards, to face these challenges head-on.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = 1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitUnitLost(Map map, Unit lost)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.BATTLE;
            z.severity = 2;
            z.weight = 0.4f;

            z.eventDesc = "Unit lost! Our " + lost.getName() + " has fallen.";
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "This failure calls our leader's capabilities into question.";
                opt.dissentEffect = ZeitOpt.DISSENT_KING;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "This clearly indicates that war is not the answer, we should attempt diplomatic solutions above all others.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "We must look to our defences in these violent times, rather than expansion.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitSettlementLost(Map map, Settlement set, Unit other)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.LOST_SETTLEMENT;
            z.severity = 3;
            z.weight = 0.8f;

            if (other.soc == null)
            {
                z.eventDesc = "Our holdings fall. " + other.getName() + " has pillaged our " + set.getComplexName() + ". This is grim news.";

            }
            else
            {
                z.eventDesc = "Our holdings fall. " + other.soc.getName() + " has pillaged our " + set.getComplexName() + ". This is grim news.";
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Our leader has failed us, maybe it is time for a replacement?";
                opt.dissentEffect = ZeitOpt.DISSENT_KING;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Our entire leadership is wretched. They all must go.";
                opt.dissentEffect = ZeitOpt.DISSENT_RULERS;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "This clearly indicates that war is not the answer, we should attempt diplomatic solutions above all others.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "We must look to our defences in these violent times, rather than expansion.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitCityLost(Map map, Settlement set, Unit other)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.LOST_SETTLEMENT;
            z.severity = 4;
            z.weight = 1f;

            z.eventDesc = "The " + set.getComplexName() + " has fallen to the enemy. This is a critial loss, and a blow to our nation as a whole.";

            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Our leader has failed us, maybe it is time for a replacement?";
                opt.dissentEffect = ZeitOpt.DISSENT_KING;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Our entire leadership is wretched. They all must go.";
                opt.dissentEffect = ZeitOpt.DISSENT_RULERS;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "This clearly indicates that war is not the answer, we should attempt diplomatic solutions above all others.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "This is proof that we have never funded our militaries sufficiently, and allowed ourselves to be defeated by unworthy foes through greed and cowardice.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "We must look to our defences in these violent times, rather than expansion.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitSettlementGained(Map map, Settlement set, Unit other)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.TOOK_SETTLEMENT;
            z.severity = 2;
            z.weight = 0.4f;

            z.eventDesc = "Our military forces have taken a " + set.getComplexName() + " and expanded our borders. Our empire grows.";

            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "This is enough for now. War has given our a bounty which peace should capitalise upon.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "Our forces are victorious once again, and will continue to be victorious as long as we are faithful to them.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "There is always more to take, always more to gain. We should press outwards to further glories.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = 1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitCityGained(Map map, Settlement set, Unit other)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.TOOK_SETTLEMENT;
            z.severity = 3;
            z.weight = 0.8f;

            z.eventDesc = "We have seized the " + set.getComplexName() + ", adding new peoples and new lands to our nation. This is a day of celebration.";

            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "This is enough for now. War has given our a bounty which peace should capitalise upon.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "Our forces are victorious once again, and will continue to be victorious as long as we are faithful to them.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "There is always more to take, always more to gain. We should press outwards to further glories.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = 1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitNoNews(Map map)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.ROUTINE;
            z.severity = 0;
            z.weight = 0.2f;

            z.eventDesc = "Nothing much has occurred recently, the lands are at peace.";

            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "Peace is good, peace builds futures, we should invest in crops not swords.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = -1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "Clearly we are now prepared to begin an offensive to expand our empire.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "Time, therefore, to look outwards and begin growing our holdings.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z,map);
                opt.desc = "There is no need to expand any further if what we have provides for us sufficiently.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitEvidenceUncovered(Map map)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.INVESTIGATION;
            z.severity = 0;
            z.weight = 0.5f;

            z.eventDesc = "Investigators have uncovered evidence of dark dealings. Malevolent powers are at work in the world.";

            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Constant vigilance is our only sure defence. Any one of us could be a traitor, watch your so-called friends.";
                opt.dissentEffect = ZeitOpt.DISSENT_EVERYONE;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We must prepare our defences, they cannot catch us off guard.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We must look inwards, to our defences, if we are to survive whatever this new threat presents.";
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitNeighbourBecomesOffensive(Map map, Society neighbour)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.GEOPOLITICS;
            z.severity = 2;
            z.weight = 0.8f;

            z.eventDesc = "Our neighbour, " + neighbour.getName() + " has entered the \"Offensive\" societal stance, a possible prelude to war.";
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We must prepare our military for possible conflict.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Defence against this threat is paramount. We must look inwards, to our walls and swords.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitNeighbourBecomesDefensive(Map map, Society neighbour)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.GEOPOLITICS;
            z.severity = 2;
            z.weight = 0.3f;

            z.eventDesc = "Our neighbour, " + neighbour.getName() + " has entered the \"Defensive\" societal stance, they are increasing their defensive capabilities.";
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We must prepare our military for possible conflict.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "Perhaps they are wise to do so, and know something we do not. We should consider doing the same.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        public static Zeit zeitNeighbourDeclaresWar(Map map, Society neighbour)
        {
            Zeit z = new Zeit(map);
            z.type = zeitType.GEOPOLITICS;
            z.severity = 3;
            z.weight = 1f;

            z.eventDesc = "Our neighbour, " + neighbour.getName() + " has declared war on a third party other.";
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "We could possibly exploit this to our own ends?";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                z.opts.Add(opt);
            }
            {
                ZeitOpt opt = new ZeitOpt(z, map);
                opt.desc = "They may well attack us next. We should prepare ourselves for such a possibility.";
                opt.politicalDeltas[Person.POLITICS_MILITARISM] = 1;
                opt.politicalDeltas[Person.POLITICS_EXPANSIONISM] = -1;
                z.opts.Add(opt);
            }
            return z;
        }
        */
    }
}
