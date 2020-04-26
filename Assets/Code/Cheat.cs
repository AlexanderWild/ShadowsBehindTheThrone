using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Cheat
    {
        public static bool controlAll = false;

        public static void takeCommand(Map map,string command)
        {
            World.log("cheat command registered: " + command);


            try
            {
                if (command == "power")
                {
                    map.overmind.power = 100;
                }
                if (command == "testsave")
                {
                    map.world.save("testSave.sv");
                }
                if (command == "testload")
                {
                    map.world.load("testSave.sv");
                }
                if (command == "shadow")
                {
                    GraphicalMap.selectedHex.location.person().shadow = 1;
                }
                if (command == "aware")
                {
                    GraphicalMap.selectedHex.location.person().awareness = 1;
                }
                if (command == "testproperty")
                {
                    Property.addProperty(map, GraphicalMap.selectedHex.location, "Military Aid");
                    World.staticMap.world.ui.checkData();
                }
                if (command == "playback")
                {
                    World.staticMap.world.ui.addBlocker(World.staticMap.world.prefabStore.getPlayback(World.staticMap.world, World.staticMap).gameObject);
                }
                if (command == "uivoting")
                {
                    World.staticMap.world.ui.uiVoting.populate((Society)GraphicalMap.selectedHex.location.soc, GraphicalMap.selectedHex.location.person());
                    World.staticMap.world.ui.setToVoting();
                }
                if (command == "100")
                {
                    World.staticMap.world.b100Turns();
                }
                if (command == "enthrall")
                {
                    if (GraphicalMap.selectedHex.location.person() == null) {
                        int c = 0;
                        Person choice = null;
                        foreach (Person p in ((Society)GraphicalMap.selectedHex.location.soc).people)
                        {
                            if (p.getLocation() == GraphicalMap.selectedHex.location)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    choice = p;
                                }
                            }
                        }
                        choice.state = Person.personState.enthralled;
                        map.overmind.enthralled = choice;
                    }
                    else
                    {
                        map.overmind.enthralled = GraphicalMap.selectedHex.location.person();
                        map.overmind.enthralled.state = Person.personState.enthralled;
                    }
                }
                if (command == "love")
                {
                    foreach (Person p in map.overmind.enthralled.society.people)
                    {
                        p.getRelation(map.overmind.enthralled).addLiking(100, "Cheat love", map.turn);
                    }
                }
                if (command == "insanity")
                {
                    GraphicalMap.selectedHex.location.person().goInsane();
                }
                if (command == "hot")
                {
                    for (int i = 0; i < map.tempMap.Length; i++)
                    {
                        for (int j = 0; j < map.tempMap[0].Length; j++)
                        {
                            map.tempMap[i][j] += 0.1f;
                            if (map.tempMap[i][j] > 1) { map.tempMap[i][j] = 1; }
                        }
                    }
                    map.assignTerrainFromClimate();
                    map.world.ui.checkData();
                }
                if (command == "cold")
                {
                    for (int i = 0; i < map.tempMap.Length; i++)
                    {
                        for (int j = 0; j < map.tempMap[0].Length; j++)
                        {
                            map.tempMap[i][j] -= 0.1f;
                            if (map.tempMap[i][j] < 0) { map.tempMap[i][j] = 0; }
                        }
                    }
                    map.assignTerrainFromClimate();
                    map.world.ui.checkData();
                }
                if (command == "min sanity")
                {
                    GraphicalMap.selectedHex.location.person().sanity = 0.01;
                }
                if (command == "die")
                {
                    GraphicalMap.selectedHex.location.person().die("Killed by console");
                }
                if (command == "evidence")
                {
                    GraphicalMap.selectedHex.location.person().evidence = 1;
                }
                if (command == "10 evidence")
                {
                    GraphicalMap.selectedHex.location.person().evidence += 0.1;
                    if (GraphicalMap.selectedHex.location.person().evidence > 1)
                    {
                        GraphicalMap.selectedHex.location.person().evidence = 1;
                    }
                }
                if (command == "refresh")
                {
                    World.staticMap.overmind.hasTakenAction = false;
                }
                if (command == "unit")
                {
                    Unit u = new Unit(GraphicalMap.selectedHex.location);
                    map.units.Add(u);
                    GraphicalMap.selectedHex.location.units.Add(u);
                }
                if (command == "victory")
                {
                    World.staticMap.overmind.victory();
                }
                if (command == "vote")
                {
                    Society soc = map.overmind.enthralled.society;
                    if (soc.voteSession != null) {
                        soc.voteSession.assignVoters();
                        World.log("Attempting to build blocker");
                        map.world.ui.addBlocker(map.world.prefabStore.getScrollSet(soc.voteSession, soc.voteSession.issue.options).gameObject);
                    }
                    
                }
                map.world.ui.checkData();
            }
            catch(Exception e)
            {
                World.log(e.Message);
            }
        }
    }
}
