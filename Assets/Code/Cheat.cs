﻿using System;
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


            //try
            {
                if (command == "power")
                {
                    map.overmind.power = 1024;
                    map.overmind.availableEnthrallments = 128;
                }
                if (command == "testsave")
                {
                    map.world.save("testSave.sv");
                    //map.world.prefabStore.popAutosave();
                }
                if (command == "testload")
                {
                    //map.world.load("testSave.sv");
                }
                if (command == "silence")
                {
                    World.self.displayMessages = !World.self.displayMessages;
                }
                if (command == "shadow")
                {
                    GraphicalMap.selectedHex.location.person().shadow = 1;
                }
                if (command == "music")
                {
                    map.world.ui.uiMusic.playTest();
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
                if (command == "hate")
                {
                    foreach (Person p in map.overmind.enthralled.society.people)
                    {
                        p.getRelation(map.overmind.enthralled).addLiking(-100, "Cheat hate", map.turn);
                    }
                }
                if (command == "resetSteamAchievements")
                {
                    SteamManager.reset_all_achievements();
                    World.staticMap.world.prefabStore.popMsg("All steam achievements reset");
                }
                if (command == "thetruth")
                {
                    foreach (Unit u in map.units)
                    {
                        if (u is Unit_Seeker seeker)
                        {
                            seeker.knowsTruth = true;
                        }
                    }
                }
                if (command == "insanity")
                {
                    GraphicalMap.selectedHex.location.person().goInsane();
                }
                if (command == "ruin")
                {
                    GraphicalMap.selectedHex.location.settlement.fallIntoRuin();
                }
                if (command == "redDeath")
                {
                    Property.addProperty(GraphicalMap.map, GraphicalMap.selectedHex.location, "Red Death");
                }
                if (command == "rotting" || command == "rotting sickness")
                {
                    Property.addProperty(GraphicalMap.map, GraphicalMap.selectedHex.location, "Rotting Sickness");
                }
                if (command == "fogSource")
                {
                    Property.addProperty(GraphicalMap.map, GraphicalMap.selectedHex.location, "Well of Fog");
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
                    //for (int i = 0; i < map.tempMap.Length; i++)
                    //{
                    //    for (int j = 0; j < map.tempMap[0].Length; j++)
                    //    {
                    //        map.tempMap[i][j] -= 0.1f;
                    //        if (map.tempMap[i][j] < 0) { map.tempMap[i][j] = 0; }
                    //    }
                    //}
                    foreach (Hex[] row in map.grid)
                    {
                        foreach (Hex h in row)
                        {
                            h.transientTempDelta -= 0.1f;
                        }
                    }
                    map.assignTerrainFromClimate();
                    map.world.ui.checkData();
                }
                if (command == "globalcooling")
                {
                    World.cheat_globalCooling = !World.cheat_globalCooling;
                }
                if (command == "min sanity")
                {
                    GraphicalMap.selectedHex.location.person().sanity = 0.01;
                }
                if (command == "die")
                {
                    GraphicalMap.selectedHex.location.person().die("Killed by console",true);
                }
                if (command == "inquisitor")
                {
                    Unit_Investigator inv = (Unit_Investigator)GraphicalMap.selectedSelectable;
                    inv.changeState(Unit_Investigator.unitState.inquisitor);
                }
                if (command == "civilWar")
                {
                    List<Person> rebels = new List<Person>();
                    Society soc = (Society)GraphicalMap.selectedHex.location.soc;
                    int c = 0;
                    foreach (Person p in soc.people)
                    {
                        if (p.title_land == null) { continue; }
                        if (p.getLocation().province != soc.getCapital().province)
                        {
                            rebels.Add(p);
                        }
                    }
                    soc.triggerCivilWar(rebels);
                }
                if (command == "evidence")
                {
                    GraphicalMap.selectedHex.location.person().evidence = 1;
                }
                if (command == "disrupt")
                {
                    GraphicalMap.selectedHex.location.person().action = new Act_Disrupted();
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
                if (command == "nextAge")
                {
                    World.staticMap.overmind.progressToNextAge();
                }
                if (command == "unit")
                {
                    Unit u = new Unit_Investigator(GraphicalMap.selectedHex.location,(Society)GraphicalMap.selectedHex.location.soc);
                    map.units.Add(u);
                    GraphicalMap.selectedHex.location.units.Add(u);
                }
                if (command == "victory")
                {
                    World.staticMap.overmind.victory();
                }
                if (command == "defeat")
                {
                    World.staticMap.overmind.defeat();
                }
                if (command == "course")
                {
                    World.staticMap.world.prefabStore.popEndgameCyclic();
                }
                if (command == "worm")
                {
                    SG_WormHive add = new SG_WormHive(map, GraphicalMap.selectedHex.location);
                    map.socialGroups.Add(add);
                }
                if (command == "placeevidence")
                {
                    GraphicalMap.selectedHex.location.evidence.Add(new Evidence(map.turn));
                    World.log("Placing evidence");
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
                if (command == "infiltrate")
                {
                    GraphicalMap.selectedHex.location.settlement.infiltration = 1;
                    World.log("Infiltrate");
                }
                if (command == "infiltratehalf")
                {
                    GraphicalMap.selectedHex.location.settlement.infiltration = 0.5;
                    World.log("Infiltrate half");
                }
                map.world.ui.checkData();
            }
            //catch(Exception e)
            //{
            //    World.log(e.Message);
            //}
        }
    }
}
