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
        public UnitManager unitManager;
        public double worldPanic;

        public Map(Params param)
        {
            this.param = param;
            masker = new MapMaskManager(this);
            //overmind = new Overmind(this);
            //eventManager = new MapEventManager(this);
           stats = new StatRecorder(this);
            unitManager = new UnitManager(this);
        }

        public void turnTick()
        {
            turn += 1;
            World.log("Turn " + turn);

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
            processMapEvents();


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
        }
        
        public void addMessage(string msg, int level = 1, bool positive = true)
        {
            turnMessages.Add(new MsgEvent(msg, level, positive));
        }

        public void addEnthralledNextTurnMessages()
        {
            if (overmind.enthralled == null)
            {
                turnMessages.Add(new MsgEvent("You may enthrall a noble. Use your power on a low-prestige noble", MsgEvent.LEVEL_BLUE, true));
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
                u.turnTick(this);
            }

            unitManager.turnTick();
        }

        public void remove(Unit unit)
        {
            units.Remove(unit);
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

        public void processWars()
        {
            //Every society decides which other to attack, assuming it is over threshold combat strength
            foreach (SocialGroup sg in socialGroups)
            {
                if (sg.lastBattle == turn) { continue; }//Only one battle permitted per social group (that they initiate, at least)

                if (checkDefensiveAttackHold(sg)) { continue; }//Should you stop attacking, to conserve strength?

                if (sg.currentMilitary < sg.maxMilitary * param.combat_thresholdAttackStrength) { continue; }//Below min strength

                sg.lastBattle = turn;

                int c = 0;
                Location attackFrom = null;
                Location attackTo = null;
                foreach (Location l in locations)
                {
                    if (l.soc == sg)
                    {
                        foreach (Link link in l.links)
                        {
                            if (link.other(l).soc != null && link.other(l).soc != sg && link.other(l).soc.getRel(sg).state == DipRel.dipState.war)
                            {
                                if (link.other(l).lastTaken == turn) { continue; }//Can't retake on this turn
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    attackFrom = l;
                                    attackTo = link.other(l);
                                }
                            }
                        }
                    }
                }
                if (attackFrom != null)
                {
                    SocialGroup defender = attackTo.soc;

                    //sg.lastBattle = turn;
                    //defender.lastBattle = turn;

                    World.log(sg.getName() + " attacking into " + attackTo.getName());
                    double myStr = sg.currentMilitary * Eleven.random.NextDouble();
                    double theirStr =  defender.currentMilitary * Eleven.random.NextDouble();
                    if (myStr < 1) { myStr = Math.Min(1, sg.currentMilitary); }
                    if (theirStr < 1) { theirStr = Math.Min(1, defender.currentMilitary); }

                    //Note the defensive fortifications only reduce losses, not increase chance of taking territory
                    double myLosses = theirStr * param.combat_lethality;
                    sg.currentMilitary -= myLosses;
                    if (sg.currentMilitary < 0) { sg.currentMilitary = 0; }
                    double theirLosses = myStr * param.combat_lethality;
                    theirLosses = computeDefensiveBonuses(theirLosses,sg,defender);
                    theirLosses = attackTo.takeMilitaryDamage(theirLosses);
                    defender.currentMilitary -= theirLosses;
                    if (defender.currentMilitary < 0) { defender.currentMilitary = 0; }

                    addMessage(sg.getName() + " attacks " + defender.getName() + ". Inflicts " + (int)(theirLosses) + " dmg, takes " + (int)(myLosses),
                        MsgEvent.LEVEL_YELLOW, false);

                    if (attackTo.settlement != null)
                    {
                        attackTo.settlement.takeAssault(sg, defender, theirLosses);
                    }
                    //Can only take land if there are no defenses in place
                    if (myStr > theirStr * param.combat_takeLandThreshold && (attackTo.getMilitaryDefence() <= 0.01))
                    {
                        takeLocationFromOther(sg, defender,attackTo);
                        attackTo.lastTaken = turn;
                    }

                    if (sg is Society && defender is Society)
                    {
                        Property.addProperty(this, attackTo, "Recent Human Battle");
                    }

                    world.prefabStore.particleCombat(attackFrom.hex, attackTo.hex);
                }
            }

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

        private bool checkDefensiveAttackHold(SocialGroup sg)
        {
            if (sg is Society == false) { return false; }
            Society soc = (Society)sg;
            if (soc.posture != Society.militaryPosture.defensive) { return false; }
            if (soc.defensiveTarget == null) { return false; }
            return soc.currentMilitary < soc.defensiveTarget.currentMilitary * 1.5;//If you completely overwhelm them attack, but otherwise hide
        }

        private double computeDefensiveBonuses(double dmg,SocialGroup att,SocialGroup def)
        {
            if (def is Society)
            {
                Society defender = (Society)def;
                if (defender.posture == Society.militaryPosture.defensive && defender.defensiveTarget == att)
                {
                    dmg *= param.combat_defensivePostureDmgMult;
                }
            }
            return dmg;
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
                else if (taken.settlement.title != null && taken.settlement.title.heldBy != null)
                {
                    Person lord = taken.settlement.title.heldBy;
                    if (att is Society)
                    {
                        Society socAtt = (Society)att;
                        lord.prestige *= param.combat_prestigeLossFromConquest;
                        if (socAtt.getSovreign() != null)
                        {
                            lord.getRelation(socAtt.getSovreign()).addLiking(param.person_likingFromBeingInvaded, "Their nation invaded mine", turn);
                        }
                        foreach (Title t in lord.titles) { t.heldBy = null; }
                        lord.titles.Clear();


                        movePerson(lord, socAtt);
                    }
                    else
                    {
                        lord.die("Killed by " + att.getName() + " when " + taken.getName() + " fell");
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
            if (receiving.people.Contains(lord)) { throw new Exception("Lord already in group they are attempting to join"); }

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
        }
    }
}
