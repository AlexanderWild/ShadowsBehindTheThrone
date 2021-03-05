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

        public enum aiMode { TESTDARK, FLESH,FLESH_ONLY ,FOG,COLD};

        public aiMode currentMode = aiMode.TESTDARK;

        public List<Ability> automatedAbilities = new List<Ability>();

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
            if (currentMode == aiMode.FOG)
            {
                ai_testFog();
            }
            if (currentMode == aiMode.COLD)
            {
                ai_testCold();
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

        public void ai_testFog()
        {
            if (overmind.power > 0)
            {
                foreach (Location loc in map.locations)
                {
                    if (loc.person() != null)
                    {
                        bool shouldDrop = false;
                        if (loc.person().state == Person.personState.broken) { shouldDrop = true; }
                        if (loc.person().state == Person.personState.enthralled) { shouldDrop = true; }
                        if (loc.person().shadow > 0.5) { shouldDrop = true; }
                        if (shouldDrop)
                        {
                            foreach (Property pr in loc.properties)
                            {
                                if (pr.proto is Pr_Fog_Source) { shouldDrop = false; break; }
                            }
                        }
                        if (shouldDrop)
                        {
                            overmind.power -= map.param.ability_fog_wellOfFogCost;
                            Property.addProperty(map, loc, "Well of Fog");
                            return;
                        }
                    }
                }
            }
            if (overmind.power > map.param.overmind_maxPower * 0.33) { 
                foreach (Location loc in map.locations)
                {
                    if (loc.hex.cloud is Cloud_Fog)
                    {
                        bool shouldDrop = true;
                        foreach (Property pr in loc.properties)
                        {
                            if (pr.proto is Pr_Fog_Source) { shouldDrop = false; break; }
                            if (pr.proto is Pr_Fog_Pinned) { shouldDrop = false; break; }
                        }
                        if (shouldDrop)
                        {
                            overmind.power -= map.param.ability_fog_trapCost;
                            Property.addProperty(map, loc, "Trapped Fog");
                            return;
                        }
                    }
                }
            }
            //Only spend reserve power on this boondoggle
            if (overmind.power > map.param.overmind_maxPower*0.66)
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

        public void ai_testCold()
        {
            if (automatedAbilities.Count == 0)
            {
                foreach (Ability a in new God_WintersScythe().getUniquePowers())
                {
                    if (a is Ab_Ice_IceBlood) { continue; }//Basically throwing away your gains if you don't know what you're doing. Which the AI doesn't
                    automatedAbilities.Add(a);
                }
            }
            if (overmind.power > 0)
            {
                Location target = null;
                int c = 0;
                Ability chosen = null;
                foreach (Ability a in automatedAbilities)
                {
                    if (map.turn - a.turnLastCast <= a.getCooldown()) { continue; }
                    if (overmind.power < a.getCost()) { continue; }
                    foreach (Location loc in map.locations)
                    {
                        if (a.castable(map, loc.hex))
                        {
                            c += 1;
                            if (Eleven.random.Next(c) == 0)
                            {
                                target = loc;
                                chosen = a;
                            }
                        }
                    }
                }
                if (chosen != null)
                {
                    chosen.cast(map, target.hex);
                }
            }
            //Only spend reserve power on this boondoggle
            if (overmind.power > map.param.overmind_maxPower * 0.66)
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
                else if (currentMode == aiMode.FOG)
                {
                    agent_Fog();
                }
                else if (currentMode == aiMode.COLD)
                {
                    agent_Standard();
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

        public void agent_Fog()
        {
            int c = 0;
            Location spawn = null;
            foreach (Location loc in map.locations)
            {
                if (loc.hex.cloud is Cloud_Fog)
                {
                    spawn = loc;
                }
            }
            if (spawn == null)
            {
                spawn = overmind.map.locations[Eleven.random.Next(overmind.map.locations.Count)];
            }
            if (spawn != null)
            {
                overmind.map.overmind.availableEnthrallments -= 1;
                World.log("Spawning tester dark at turn " + overmind.map.turn);

                Unit agent = new Unit_TesterDarkFog(spawn, overmind.map.soc_dark);

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
        public void popAIModeMessage()
        {
            List<string> msgs = new List<string>();
            msgs.Add("Standard");
            msgs.Add("Flesh");
            msgs.Add("Flesh Only");
            msgs.Add("Fog");
            msgs.Add("Cold");
            overmind.map.world.ui.addBlocker(overmind.map.world.prefabStore.getScrollSetText(msgs, false, this).gameObject);
        }

        public void selectableClicked(string text)
        {
            if (text == "Standard") { currentMode = aiMode.TESTDARK; }
            if (text == "Flesh") { currentMode = aiMode.FLESH; }
            if (text == "Flesh Only") { currentMode = aiMode.FLESH_ONLY; }
            if (text == "Fog") { currentMode = aiMode.FOG; }
            if (text == "Cold") { currentMode = aiMode.COLD; }
            World.log("AI mode is set to " + currentMode);

            map.world.prefabStore.popMsgTreeBackground("Welcome to the automatic testing interface. This allows games to be played automatically, with both the human and the dark sides played "
                + " by AI control. This is designed to allow rapid testing for rough balancing and for AI behaviour analysis. \n\nHold CTRL to end one turn per half second, hold ALT to progress turns as fast as possible.");
        }
    }
}
