using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public class Overmind_Automatic : SelectClickReceiver
    {
        public Overmind overmind;
        public Map map;

        public enum aiMode { TESTDARK, FLESH,FLESH_ONLY };

        public aiMode currentMode = aiMode.TESTDARK;

        public Overmind_Automatic(Overmind overmind)
        {
            this.overmind = overmind;
            this.map = overmind.map;
        }

        public void turnTick()
        {
            if (currentMode == aiMode.TESTDARK)
            {
                ai_testDark();
            }
            if (currentMode == aiMode.FLESH)
            {
                ai_testFlesh();
            }
            if (currentMode == aiMode.FLESH_ONLY)
            {
                ai_testFlesh();
            }
        }

        public void ai_testDark()
        {
            if (overmind.power > 0)
            {
                foreach (Unit u in overmind.map.units)
                {
                    if (u is Unit_Investigator && u.task is Task_Investigate)
                    {
                        u.task = new Task_Disrupted();
                        overmind.power -= overmind.map.param.ability_disruptAgentCost;
                        break;
                    }
                }
            }
        }

        public void ai_testFlesh()
        {
            if (overmind.power <= 0)
            { return; }

            int c = 0;
            int c2 = 0;
            Location expandLoc = null;
            Location growLoc = null;
            Location parentExpand = null;
            foreach (Location loc in overmind.map.locations)
            {
                if (loc.soc is SG_UnholyFlesh)
                {
                    foreach (Location l2 in loc.getNeighbours())
                    {
                        if (l2.isOcean == false && l2.soc == null && l2.settlement == null)
                        {
                            c += 1;
                            if (Eleven.random.Next(c) == 0)
                            {
                                expandLoc = l2;
                                parentExpand = loc;
                            }
                        }
                        else if (l2.soc is Society && l2.soc.currentMilitary < loc.soc.currentMilitary)
                        {
                            if (loc.soc.isAtWar() == false)
                            {
                                overmind.map.declareWar(loc.soc, l2.soc);
                            }
                        }
                    }
                }
                else if (loc.isOcean == false && loc.settlement == null && loc.soc == null)
                {
                    c2 += 1;
                    if (Eleven.random.Next(c2) == 0)
                    {
                        growLoc = loc;
                    }
                }
            }
            if (expandLoc != null)
            {
                expandLoc.soc = parentExpand.soc;
                expandLoc.settlement = new Set_UnholyFlesh_Ganglion(expandLoc);
                expandLoc.soc.temporaryThreat += overmind.map.param.ability_growFleshThreatAdd;
                overmind.power -= map.param.ability_fleshGrowCost;
            }
            else if (growLoc != null)
            {

                SG_UnholyFlesh soc = null;
                foreach (SocialGroup sg in map.socialGroups)
                {
                    if (sg is SG_UnholyFlesh)
                    {
                        soc = (SG_UnholyFlesh)sg;
                    }
                }
                if (soc == null)
                {
                    map.socialGroups.Add(new SG_UnholyFlesh(map, growLoc));
                }
                else
                {
                    growLoc.soc = soc;
                }

                growLoc.settlement = new Set_UnholyFlesh_Seed(growLoc);
                overmind.power -= map.param.ability_fleshSeedCost;
            }
        }

        public void checkSpawnAgent()
        {
            int presentDarks = 0;
            foreach (Unit u in overmind.map.units)
            {
                if (u.isEnthralled())
                {
                    presentDarks += 1;
                }
            }

            if (presentDarks < overmind.map.param.overmind_maxEnthralled && overmind.map.overmind.availableEnthrallments > 0)
            {
                if (currentMode == aiMode.TESTDARK)
                {
                    agent_Standard();
                }
                else if (currentMode == aiMode.FLESH)
                {
                    agent_Flesh();
                }
                else if (currentMode == aiMode.FLESH_ONLY)
                {
                    //No agents, only kill
                }
            }
        }

        public void agent_Flesh()
        {
            int c = 0;
            Location spawn = null;
            foreach (Location loc in map.locations)
            {
                if (loc.soc is SG_UnholyFlesh)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        spawn = loc;
                    }
                }
            }
            if (spawn != null)
            {
                overmind.map.overmind.availableEnthrallments -= 1;
                World.log("Spawning tester dark at turn " + overmind.map.turn);
                foreach (Unit u in overmind.map.units)
                {
                    if (u is Unit_TesterDark)
                    {
                        spawn = u.location.getNeighbours()[0];
                    }
                }
                if (spawn == null)
                {
                    spawn = overmind.map.locations[Eleven.random.Next(overmind.map.locations.Count)];
                }

                Unit agent = new Unit_TesterDark(spawn, overmind.map.soc_dark);

                agent.person = new Person(overmind.map.soc_dark);
                agent.person.state = Person.personState.enthralledAgent;
                agent.person.unit = agent;
                overmind.map.units.Add(agent);

                Evidence ev = new Evidence(overmind.map.turn);
                ev.pointsTo = agent;
                ev.weight = 0.66;
                agent.location.evidence.Add(ev);

                agent.task = null;

                GraphicalMap.panTo(spawn.hex.x, spawn.hex.y);
            }
        }
        public void agent_Standard()
        {
            overmind.map.overmind.availableEnthrallments -= 1;
            World.log("Spawning tester dark at turn " + overmind.map.turn);
            Location spawn = null;
            foreach (Unit u in overmind.map.units)
            {
                if (u is Unit_TesterDark)
                {
                    spawn = u.location.getNeighbours()[0];
                }
            }
            if (spawn == null)
            {
                spawn = overmind.map.locations[Eleven.random.Next(overmind.map.locations.Count)];
            }

            Unit agent = new Unit_TesterDark(spawn, overmind.map.soc_dark);

            agent.person = new Person(overmind.map.soc_dark);
            agent.person.state = Person.personState.enthralledAgent;
            agent.person.unit = agent;
            overmind.map.units.Add(agent);

            Evidence ev = new Evidence(overmind.map.turn);
            ev.pointsTo = agent;
            ev.weight = 0.66;
            agent.location.evidence.Add(ev);

            agent.task = null;

            GraphicalMap.panTo(spawn.hex.x, spawn.hex.y);
        }

        public void popAIModeMessage()
        {
            List<string> msgs = new List<string>();
            msgs.Add("Standard");
            msgs.Add("Flesh");
            msgs.Add("Flesh Only");
            overmind.map.world.ui.addBlocker(overmind.map.world.prefabStore.getScrollSetText(msgs, false, this).gameObject);
        }

        public void selectableClicked(string text)
        {
            if (text == "Standard") { currentMode = aiMode.TESTDARK; }
            if (text == "Flesh") { currentMode = aiMode.FLESH; }
            if (text == "Flesh Only") { currentMode = aiMode.FLESH_ONLY; }
            World.log("AI mode is set to " + currentMode);

            map.world.prefabStore.popMsgTreeBackground("Welcome to the automatic testing interface. This allows games to be played automatically, with both the human and the dark sides played "
                + " by AI control. This is designed to allow rapid testing for rough balancing and for AI behaviour analysis. \n\nHold CTRL to end one turn per half second, hold ALT to progress turns as fast as possible.");
        }
    }
}
