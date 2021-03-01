using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public partial class Map
    {
        public List<Location> majorLocations = new List<Location>();
        public List<Location> locations = new List<Location>();
        public Hex[][] grid;//MUST be a jagged implement. For serialisation
        public World world;
        public bool burnInComplete = false;
        public MapMaskManager masker;
        public Globalist globalist = new Globalist();
        public int turn;
        public List<SocialGroup> socialGroups = new List<SocialGroup>();
        public HashSet<string> permaDismissed = new HashSet<string>();
        //public MapEventManager eventManager;
        public StatRecorder stats;
        public float lastTurnTime;
        public Params param;
        public Overmind overmind;
        public List<MsgEvent> turnMessages = new List<MsgEvent>();
        public List<Unit> units = new List<Unit>();
        public double data_avrgEnshadowment;
        public int data_nSocietyLocations;
        public double data_awarenessSum;
        public int awarenessReportings;//We're populating by people's turn ticks, so we need to count how many
        public double data_globalTempSum = 0;
        public double data_globalTempInitial = 0;
        public UnitManager unitManager;
        public double worldPanic;
        public int personIndexCount = 0;
        public EventManager eventManager;
        public bool simplified = false;
        public bool agentsOnly = true;
        public bool automatic = false;
        public int automaticMode = 0;
        public bool hasEnthralledAnAgent = false;
        public bool hasBrokenSoul = false;
        public long seed;

        public Society soc_dark;
        public Society soc_light;

        public List<Culture> cultures = new List<Culture>();
        public List<Person> persons = new List<Person>();

        public Dictionary<string, string> compressionMap = new Dictionary<string, string>();

        public Map(Params param)
        {
            this.param = param;
            masker = new MapMaskManager(this);
            //overmind = new Overmind(this);
            //eventManager = new MapEventManager(this);
            stats = new StatRecorder(this);
            unitManager = new UnitManager(this);
            soc_dark = new SG_AgentDark(this);
            soc_light = new SG_AgentLight(this);
            eventManager = new EventManager(this);
        }

        public void turnTick()
        {
            turn += 1;
            //World.log("Turn " + turn);

            lastTurnTime = UnityEngine.Time.fixedTime;
            turnMessages.Clear();
            //eventManager.turnTick();
            //overmind.turnTick();
            //panic.turnTick();

            worldPanic = overmind.computeWorldPanic(new List<ReasonMsg>());

            //Then grid cells
            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    grid[i][j].turnTick();
                }
            }

            processUnits();
            processWars();
            processAwareness();
            //processMapEvents();


            //Finally societies
            //Use a duplication list so they can modify the primary society list (primarly adding a child soc)
            List<SocialGroup> duplicate = new List<SocialGroup>();
            foreach (SocialGroup group in socialGroups) { duplicate.Add(group); }
            foreach (SocialGroup group in duplicate)
            {
                group.turnTick();
            }

            List<SocialGroup> rems = new List<SocialGroup>();
            foreach (SocialGroup group in socialGroups)
            {
                if (group is SG_AgentLight) { continue; }
                if (group is SG_AgentDark) { continue; }
                if (group.checkIsGone()) { rems.Add(group); }
                else
                {
                    recomputeInformationAvailability(group);
                }
            }
            foreach (SocialGroup g in rems)
            {
                socialGroups.Remove(g);
            }

            overmind.turnTick();
            stats.turnTick();
            addEnthralledNextTurnMessages();
            assignTerrainFromClimate();
        }
        

        public void processAwareness()
        {
            if (param.useAwareness != 1) { return; }
            if (awarenessReportings > 0) { data_awarenessSum /= awarenessReportings; }

            double targetAwareness = worldPanic;
            targetAwareness = Math.Pow(targetAwareness, 2);//Square it, such that it reaches 100% at 100%, but dawdles before then
            if (data_awarenessSum < targetAwareness)
            {
                if (turn % 7 == 0)
                {
                    int c = 0;
                    Person target = null;
                    foreach (Location loc in locations)
                    {
                        if (loc.soc is Society && loc.person() != null && loc.settlement != null && loc.settlement is Set_University && loc.person().state == Person.personState.normal)
                        {
                            if (loc.person().awareness < 1)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    target = loc.person();
                                }
                            }
                        }
                    }
                    if (target != null)
                    {
                        target.awareness = 1;
                        addMessage(target.getFullName() + " has gained awareness, having studied the signs", MsgEvent.LEVEL_ORANGE, false);
                    }
                }
                else
                {
                    double sumGain = 0;
                    foreach (Location loc in locations)
                    {
                        if (loc.person() != null && loc.person().state == Person.personState.normal && loc.person().awareness > 0)
                        {
                            foreach (Location l2 in loc.getNeighbours())
                            {
                                if (l2.person() != null && l2.person().state == Person.personState.normal && l2.person().awareness < loc.person().awareness)
                                {
                                    double maxGain = loc.person().awareness - l2.person().awareness;
                                    maxGain *= 1 - l2.person().shadow;
                                    double prev = l2.person().awareness;
                                    l2.person().nextTurnawareness = l2.person().awareness + (maxGain * Eleven.random.NextDouble());
                                    if (l2.person().nextTurnawareness > 1) { l2.person().nextTurnawareness = 1; }//Should be impossible, but best not risk it
                                    sumGain += l2.person().nextTurnawareness - prev;
                                }
                            }
                        }
                    }
                    if (sumGain > 0.9)
                    {
                        addMessage("Awareness of the dark grows", MsgEvent.LEVEL_ORANGE, false);
                    }
                }
            }
            data_awarenessSum = 0;
            awarenessReportings = 0;
        }

        public void addMessage(string msg, int level = 1, bool positive = true)
        {
            turnMessages.Add(new MsgEvent(msg, level, positive));
        }

        public void addEnthralledNextTurnMessages()
        {
            if (overmind.enthralled == null)
            {
                if (param.overmind_allowDirectEnthralling == 1)
                {
                    turnMessages.Add(new MsgEvent("You may enthrall a noble. Use your power on a low-prestige noble", MsgEvent.LEVEL_BLUE, true));
                }
            }
            else
            {
                if (overmind.enthralled.society.voteSession != null)
                {
                    string msg = "Vote in session: " + overmind.enthralled.society.voteSession.issue.ToString();
                    turnMessages.Add(new MsgEvent(msg, MsgEvent.LEVEL_GREEN, true));
                }
            }
        }

        public void processUnits()
        {
            List<Unit> mutable = new List<Unit>();
            mutable.AddRange(units);
            foreach (Unit u in mutable)
            {
                if (u.isDead) { continue; }
                u.turnTick(this);

                //Combat action against unit on own square
                foreach (Unit u2 in u.location.units)
                {
                    if (u.hostileTo(u2))
                    {
                        if (u == u2) { throw new Exception(u.getName() + " was self-hostile"); }
                        combatAction(u, u2, u.location);
                        break;
                    }
                }
            }

            unitManager.turnTick();
        }

        public void remove(Unit unit)
        {
            units.Remove(unit);
        }


        public Culture sampleCulture(Location loc)
        {
            if (cultures.Count == 0) { return null; }
            if (loc == null) { return cultures[Eleven.random.Next(cultures.Count)]; }

            return loc.culture;
        }

        public Culture sampleCulture(SocialGroup sg)
        {
            if (cultures.Count == 0) { return null; }
            Location chosen = null;
            int c1 = 0;
            int c2 = 0;
            int c3 = 0;
            //Select, but with whole load of fallbacks so we at least find something
            foreach (Location l in locations)
            {
                c3 += 1;
                if (c1 == 0 && c2 == 0 && Eleven.random.Next(c3) == 0)
                {
                    chosen = l;
                }
                if (l.soc is Society)
                {
                    c2 += 1;
                    if (c1 == 0  && Eleven.random.Next(c2) == 0)
                    {
                        chosen = l;
                    }
                }
                if (l.soc  == sg)
                {
                    c1 += 1;
                    if (Eleven.random.Next(c1) == 0)
                    {
                        chosen = l;
                    }
                }
            }
            return sampleCulture(chosen);
        }
        public void processMapEvents()
        {
            /*
            int nEvils = 0;
            foreach (SocialGroup sg in socialGroups)
            {
                if (sg is Society == false)
                {
                    nEvils += 1;
                }
            }
            if (nEvils < 3)
            {
                if (turn % 5 == 0)
                {
                    Location chosen = null;
                    int c = 0;
                    foreach (Location loc in locations)
                    {
                        if (loc.isOcean) { continue; }
                        if (loc.soc != null) { continue; }
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            chosen = loc;
                        }
                    }
                    if (chosen != null)
                    {
                        SG_WormHive add = new SG_WormHive(this, chosen);
                        socialGroups.Add(add);
                    }
                }
            }
            */
        }

        public void compressForSave()
        {
            compressionMap.Clear();
            foreach (SocialGroup sg in socialGroups)
            {
                if (sg is Society)
                {
                    Society soc = (Society)sg;
                    foreach (Person p in soc.people)
                    {
                        compressPerson(p);
                    }
                }
            }
            foreach (Unit u in units)
            {
                if (u.person != null) { compressPerson(u.person); }
            }
            foreach (Location loc in locations)
            {
                loc.savedLinks.Clear();
                foreach (Link l in loc.links)
                {
                    loc.savedLinks.Add(l.other(loc).index);
                }
                loc.links.Clear();
            }

            for (int i = 0; i < persons.Count; i++)
            {
                if (persons[i] == null) { continue; }
                if (persons[i].unit == null && (persons[i].society.people.Contains(persons[i]) == false))
                {
                    persons[i] = null;
                }
            }
        }

        public void compressPerson(Person p)
        {

            List<RelObj> rems = new List<RelObj>();
            foreach (RelObj rel in p.relations.Values)
            {
                Person them = persons[rel.them];
                if (them == null) { return; }
                if (
                    Math.Abs(rel.getLiking()) < 10 &&
                    rel.suspicion < 0.1 &&
                    (them.state != Person.personState.enthralled || them.state != Person.personState.enthralledAgent))
                {
                    rems.Add(rel);
                }
                else
                {
                    foreach (RelEvent msg in rel.events)
                    {
                        if (!compressionMap.ContainsKey(msg.reason))
                        {
                            compressionMap.Add(msg.reason, "" + compressionMap.Count);
                        }
                        msg.reason = compressionMap[msg.reason];
                    }
                }
            }
            foreach (RelObj rel in rems)
            {
                p.relations.Remove(rel.them);
            }
        }

        public void decompressFromSave()
        {
            Dictionary<string, string> invertedDictionary = new Dictionary<string, string>();
            foreach (String key in compressionMap.Keys)
            {
                string val = compressionMap[key];
                invertedDictionary[val] = key;
            }
            foreach (SocialGroup sg in socialGroups)
            {
                if (sg is Society)
                {
                    Society soc = (Society)sg;
                    foreach (Person p in soc.people)
                    {
                        decompressPerson(invertedDictionary,p);
                    }
                }
            }
            foreach (Unit u in units)
            {
                if (u.person != null) { decompressPerson(invertedDictionary,u.person); }
            }
            compressionMap.Clear();

            foreach (Location loc in locations)
            {
                foreach (int i in loc.savedLinks)
                {
                    if (i > loc.index)
                    {
                        Link link = new Link(loc, locations[i]);
                        loc.links.Add(link);
                        locations[i].links.Add(link);
                    }
                }
            }
        }

        public void decompressPerson(Dictionary<string, string> invertedDictionary,Person p)
        {
            foreach (int other in p.relations.Keys)
            {
                RelObj rel = p.relations[other];
                foreach (RelEvent msg in rel.events)
                {
                    if (invertedDictionary.ContainsKey(msg.reason))
                    {
                        msg.reason = invertedDictionary[msg.reason];
                    }
                }
                rel.them = other;
            }

        }
        public void processWars()
        {

            foreach (SocialGroup group in socialGroups)
            {
                foreach (DipRel rel in group.getAllRelations())
                {
                    if (rel.state == DipRel.dipState.war && rel.war.canTimeOut)
                    {
                        if (turn - rel.war.startTurn > param.war_defaultLength)
                        {
                            declarePeace(rel);
                        }
                    }
                }
            }
        }

        public void takeLocationFromOther(SocialGroup att,SocialGroup def,Location taken)
        {
            World.log(att.getName() + " takes " + taken.getName() + " from " + def.getName());
            int priority = MsgEvent.LEVEL_YELLOW;
            bool benefit = !def.hasEnthralled();
            if (att.hasEnthralled())
            {
                priority = MsgEvent.LEVEL_GREEN;
            }else if (def.hasEnthralled())
            {
                priority = MsgEvent.LEVEL_RED;
            }
            else
            {
                priority = MsgEvent.LEVEL_YELLOW;
            }


            turnMessages.Add(new MsgEvent(att.getName() + " takes " + taken.getName() + " from " + def.getName(), priority,benefit));

            if (taken.settlement != null)
            {
                if (taken.settlement.isHuman == false)
                {
                    taken.settlement = null;//Burn it down
                }
                else if (taken.settlement is Set_Ruins)
                {
                    //Nothing to do if you take ruins
                }
                else if (taken.settlement.title != null && taken.settlement.title.heldBy != null)
                {
                    Person lord = taken.settlement.title.heldBy;
                    if (att is Society)
                    {
                        Society socAtt = (Society)att;
                        lord.prestige *= param.combat_prestigeLossFromConquest;
                        if (socAtt.getSovereign() != null)
                        {
                            lord.getRelation(socAtt.getSovereign()).addLiking(param.person_likingFromBeingInvaded, "Their nation invaded mine", turn);
                        }
                        foreach (Title t in lord.titles) { t.heldBy = null; }
                        lord.titles.Clear();


                        movePerson(lord, socAtt);
                    }
                    else
                    {
                        lord.die("Killed by " + att.getName() + " when " + taken.getName() + " fell",true);
                    }
                }
            }

            taken.soc = att;
            att.takeLocationFromOther(def, taken);

            bool hasRemainingTerritory = false;
            foreach (Location loc in locations)
            {
                if (loc.soc == def)
                {
                    hasRemainingTerritory = true;
                    break;
                }
            }
            if (!hasRemainingTerritory)
            {
                World.log("Last territory taken");
                addMessage(def.getName() + " has lost its last holdings to " + att.getName());

                /*
                if (att is Society && def is Society)
                {
                    Society sAtt = (Society)att;
                    Society sDef = (Society)def;
                    List<Person> toMove = new List<Person>();
                    foreach (Person p in sDef.people)
                    {
                        if (p.title_land == null)
                        {
                            toMove.Add(p);
                        }
                    }
                    foreach (Person p in toMove)
                    {
                        movePerson(p, sAtt);
                        addMessage(p.getFullName() + " is now part of the court of " + att.getName(), MsgEvent.LEVEL_GRAY, false);
                    }
                }
                */
            }
        }

        public void movePerson(Person lord,Society receiving)
        {
            if (lord.society.people.Contains(lord) == false) { throw new Exception("Person attempting to leave society they were not a part of"); }
            if (receiving.people.Contains(lord)) { throw new Exception("Lord already in group they are attempting to join " + lord.society.getName() + " to " + receiving.getName()); }

            lord.society.people.Remove(lord);
            receiving.people.Add(lord);
            lord.society = receiving;
            World.log(lord.getFullName() + " now under the rule of " + receiving.getName());
        }

        private void declarePeace(DipRel rel)
        {
            World.log("Peace breaks out between " + rel.a.getName() + " and " + rel.b.getName());

            turnMessages.Add(new MsgEvent("The war between " + rel.war.att.getName() + " and " + rel.war.def.getName() + " winds down", MsgEvent.LEVEL_YELLOW, false));

            rel.war = null;
            rel.state = DipRel.dipState.none;

            rel.a.addHistory("Now at peace with " + rel.b.getName());
            rel.b.addHistory("Now at peace with " + rel.a.getName());
        }

        public void declareWar(SocialGroup att,SocialGroup def)
        {
            World.log(att.getName() + " declares war on " + def.getName());
            int priority = MsgEvent.LEVEL_ORANGE;
            bool good = false;
            if (att.hasEnthralled()) { good = true;priority = MsgEvent.LEVEL_GREEN; }
            if (def.hasEnthralled()) { priority = MsgEvent.LEVEL_RED; }
            turnMessages.Add(new MsgEvent(att.getName() + " launches an offensive against " + def.getName(),priority,good));

            att.getRel(def).state = DipRel.dipState.war;
            att.getRel(def).war = new War(this,att, def);

            if (def.getRel(att).state != DipRel.dipState.war) { throw new Exception("Asymmetric war"); }

            foreach (Location loc in locations)
            {
                if (loc.soc == att || loc.soc == def)
                {
                    if (loc.settlement != null && loc.settlement.embeddedUnit != null)
                    {
                        loc.units.Add(loc.settlement.embeddedUnit);
                        units.Add(loc.settlement.embeddedUnit);
                        loc.settlement.embeddedUnit = null;
                    }
                }
            }

            if (att is Society)
            {
                Society sAtt = (Society)att;
                sAtt.crisisWarLong = "We are at war, attacking " + def.getName() + ". We can discuss how to deploy our forces.";
                sAtt.crisisWarShort = "Crisis: At War";
                //Get the agents moving early, or they'll stick around doing whatever nonsense they previously were
                foreach (Unit u in units)
                {
                    if (u.society == def &&
                        u is Unit_Investigator &&
                        (((Unit_Investigator)u).state == Unit_Investigator.unitState.basic || ((Unit_Investigator)u).state == Unit_Investigator.unitState.knight)
                        && (u.task is Task_Disrupted == false))
                    {
                        u.task = new Task_DefendHomeland();
                    }
                }
            }
            att.addHistory("We declared war on " + def.getName());
            if (def is Society)
            {
                Society sDef = (Society)def;
                sDef.crisisWarLong = att.getName() + " has attacked us, and we are at war. We must respond to this crisis.";
                sDef.crisisWarShort = "Crisis: At War";
                //Get the agents moving early, or they'll stick around doing whatever nonsense they previously were
                foreach (Unit u in units)
                {
                    if (u.society == def &&
                        u is Unit_Investigator &&
                        (((Unit_Investigator)u).state == Unit_Investigator.unitState.basic || ((Unit_Investigator)u).state == Unit_Investigator.unitState.knight)
                        && (u.task is Task_Disrupted == false))
                    {
                        u.task = new Task_DefendHomeland();
                    }
                }
            }
            def.addHistory(att.getName() + " declared war on us");
        }
    }
}
