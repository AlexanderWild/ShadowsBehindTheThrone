using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Overmind
    {
        public double power;
        public bool hasTakenAction;

        public List<Ability> abilities = new List<Ability>();
        public List<Ability> powers = new List<Ability>();
        public List<God> namesChosen = new List<God>();
        public Map map;
        public Person enthralled;
        public bool endOfGameAchieved = false;
        public bool hasEnthrallAbilities = false;
        public double panicFromPowerUse;
        public double panicFromCluesDiscovered;
        public int nStartingHumanSettlements;
        public bool isFirstEnthralledAgent = true;
        public int availableEnthrallments = 0;
        public int nEnthralled = 0;//Set by the UI. Probably not a great thing to have done, but oh well
        public int maxEnthralled = 3;
        public Overmind_Automatic autoAI;

        public int lightRitualProgress = 0;
        public Society lightbringerCasters;
        public bool firstLightbringer = true;
        public List<Location> lightbringerLocations = new List<Location>();
        public Location lightbringerCapital = null;

        public bool hintEnthrallNoble = false;
        public Overmind(Map map)
        {
            this.map = map;
            maxEnthralled = map.param.overmind_maxEnthralled;
            autoAI = new Overmind_Automatic(this);
            hasEnthrallAbilities = true;
            if (map.agentsOnly == false)
            {
                if (map.param.overmind_allowDirectEnthralling == 1)
                {
                    powers.Add(new Ab_Enth_Enthrall());
                }
                powers.Add(new Ab_Over_CreateAgent());
                powers.Add(new Ab_Over_EnthrallAgent());
                powers.Add(new Ab_Enth_DarkEmpire());
                powers.Add(new Ab_Over_HateTheLight());
            }
            else
            {
                powers.Add(new Ab_Over_CreateAgent());
                powers.Add(new Ab_Over_EnthrallAgent());
            }
            //abilities.Add(new Ab_TestAddShadow());
        }

        public void computeEnthralled()
        {
            nEnthralled = 0;
            if (enthralled != null) { nEnthralled += 1; }
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled() && (!u.automated))
                {
                    nEnthralled += 1;
                }
            }
        }

        public void addDefaultAbilities()
        {
            World.log("Agents Only: " + map.agentsOnly);
            if (map.agentsOnly == false)
            {
                World.log("ENTHRALLING STATUS: " + map.param.overmind_allowDirectEnthralling);
                if (map.param.overmind_allowDirectEnthralling == 1)
                {
                    powers.Add(new Ab_Enth_Enthrall());
                }
                powers.Add(new Ab_Enth_MiliaryAid());
                powers.Add(new Ab_Enth_TrustingFool());
                powers.Add(new Ab_Enth_Enshadow());
                powers.Add(new Ab_Enth_Apoptosis());
                powers.Add(new Ab_Over_DisruptAgent());
                powers.Add(new Ab_Over_DelayVote());
                //powers.Add(new Ab_Over_CancelVote());
                //powers.Add(new Ab_Over_InformationBlackout());
                powers.Add(new Ab_Over_SowDissent());
                powers.Add(new Ab_Over_UncannyGlamour());
                powers.Add(new Ab_Enth_AuraOfLunacy());

                //abilities.Add(new Ab_Soc_Vote());
                abilities.Add(new Ab_Soc_ProposeVote());
                abilities.Add(new Ab_Soc_SharedGlory());
                abilities.Add(new Ab_Soc_JoinRebels());
                abilities.Add(new Ab_Soc_JoinLoyalists());
                abilities.Add(new Ab_Soc_ShareEvidence());
                abilities.Add(new Ab_Soc_BoycottVote());
                abilities.Add(new Ab_Soc_Fearmonger());
                abilities.Add(new Ab_Soc_DenounceOther());
                abilities.Add(new Ab_Soc_ProvincialSentiments());
                //abilities.Add(new Ab_Soc_SwitchVote());
                abilities.Add(new Ab_Soc_ShareTruth());

                if (map.param.useAwareness == 1)
                {
                    powers.Add(new Ab_Over_DisruptAction());
                }
            }
            else
            {
                powers.Add(new Ab_Over_DisruptAgent());
                powers.Add(new Ab_Over_DelayVote());
            }
        }

        public void printHintInfiltration()
        {
            String msg = "Infiltration is a core element of agent gameplay, and a very good strategy is to begin building infiltration with your first few agents, before moving on to making use of this foundation with your second wave (possibly after the first have met an unfortunate end)."
                + "It represents your cultists, embedded in human society, working their way into all levels of human society."
                + "\n\nInfiltration can be seen on the location information, by a map view or by dark tendrils reaching into infiltrated locations' map icons. It is very hard for humans to remove your infiltration (it almost always requires moving or removing the location's noble), and many agents can benefit"
                + " from operating in infilrated locations."
                + "\n\nVampires and Merchants can infiltrate at first, then a Seeker or Plague Doctor can act unnoticed, once the initial work has been set up to let them operate efficiently.";
            map.world.prefabStore.popMsgHint(msg, "Hint: Infiltration");
        }
        public void printHintEnthrallNoble()
        {
            if (hintEnthrallNoble) { return; }
            hintEnthrallNoble = true;
            String msg = "Enthralled Nobles are complex tools, with a range of strategies. The primary strategy is the political career."
                + "\n\nYour enthralled noble often starts at a very low level, and must rise the ranks through careful ally building and sabotage of your rivals (using your agents or Dark Powers)."
                + "\n\nIf they reach a high rank in society, they can enshadow themselves, and spread shadow to all the nobles of lesser prestige."
                + "\n\nIf they reach the top of the society, they can declare a Dark Empire using an ability, which will spread shadow to all the society's nobles over time, advancing your progress to victory considerably, " +
                "and giving you a warlike collection of broken nobles to use to invade your weaker neighbours to spread the shadow still further.";
            map.world.prefabStore.popMsgHint(msg, "Hint: Enthralled Nobles");
        }

        public double computeWorldPanic(List<ReasonMsg> reasons)
        {
            double panic = 0;
            panic += panicFromPowerUse;
            reasons.Add(new ReasonMsg("Power use", panic * 100));

            double shadow = map.data_avrgEnshadowment * map.param.panic_panicAtFullShadow;
            panic += shadow;
            reasons.Add(new ReasonMsg("World Shadow", shadow * 100));

            panic += panicFromCluesDiscovered;
            reasons.Add(new ReasonMsg("Evidence Discovered", panicFromCluesDiscovered * 100));

            double nHumans = map.data_nSocietyLocations;
            if (nStartingHumanSettlements > 0)
            {
                double extinction = (nStartingHumanSettlements - nHumans) / nStartingHumanSettlements;
                extinction *= map.param.panic_panicAtFullExtinction;
                if (extinction < 0) { extinction = 0; }//In the off chance they reclaim something
                panic += extinction;
                reasons.Add(new ReasonMsg("Lost Settlements", extinction * 100));
            }

            if (panic > 1) { panic = 1; }
            if (panic < 0) { panic = 0; }
            return panic;
        }
        public void increasePanicFromPower(int cost, Ability ability)
        {
            if (cost == 0) { return; }

            panicFromPowerUse += cost * map.param.panic_panicPerPower;
            if (panicFromPowerUse > 1) { panicFromPowerUse = 1; }

            List<Person> allPeople = new List<Person>();
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is Society)
                {
                    allPeople.AddRange(((Society)sg).people);
                }
            }
            double sumWeighting = 0;
            foreach (Person p in allPeople)
            {
                double pv = p.getAwarenessMult();
                if (p.title_land == null) { continue; }
                if (p.awareness >= 1) { pv = 0; }
                if (p.awareness > 0) { pv *= map.param.awarenessInvestigationDetectMult; }
                pv *= pv;
                sumWeighting += pv;
            }
            Person detector = null;
            double roll = Eleven.random.NextDouble() * sumWeighting;
            foreach (Person p in allPeople)
            {
                double pv = p.getAwarenessMult();
                if (p.title_land == null) { continue; }
                if (p.awareness >= 1) { pv = 0; }
                if (p.awareness > 0) { pv *= map.param.awarenessInvestigationDetectMult; }
                pv *= pv;
                roll -= pv;
                if (roll <= 0)
                {
                    detector = p;
                    break;
                }
            }

            if (detector != null)
            {
                double gain = cost * map.param.awareness_increasePerCost * map.param.awareness_master_speed;
                gain *= detector.getAwarenessMult();
                detector.awareness += gain;
                if (detector.awareness > 1) { detector.awareness = 1; }
                map.turnMessages.Add(new MsgEvent(detector.getFullName() + " has noticed a sign of dark power. Gains " + (int)(100 * gain) + " awareness", MsgEvent.LEVEL_RED, false));
            }

            map.worldPanic = this.computeWorldPanic(new List<ReasonMsg>());
        }

        public void turnTick()
        {
            hasTakenAction = false;
            if (power < map.param.overmind_maxPower)
            {
                power += map.param.overmind_powerRegen;
            }
            //power = Math.Min(power, map.param.overmind_maxPower);


            panicFromPowerUse -= map.param.panic_dropPerTurn;
            if (panicFromPowerUse < 0) { panicFromPowerUse = 0; }

            panicFromCluesDiscovered -= map.param.panic_dropPerTurn;
            if (panicFromCluesDiscovered < 0) { panicFromCluesDiscovered = 0; }

            processEnthralled();
            int count = 0;
            double sum = 0;
            int nHumanSettlements = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.person() != null) { sum += loc.person().shadow; count += 1; }
                if (loc.soc != null && loc.settlement != null && (loc.settlement is Set_Ruins == false) && (loc.settlement is Set_CityRuins == false) && loc.soc is Society)
                {
                    nHumanSettlements += 1;
                }
            }
            if (count == 0) { map.data_avrgEnshadowment = 0; }
            else { map.data_avrgEnshadowment = sum / count; }
            if ((!endOfGameAchieved) && map.data_avrgEnshadowment > map.param.victory_targetEnshadowmentAvrg)
            {
                victory();
            }
            if (nHumanSettlements == 0)
            {
                victory();
            }
            map.data_nSocietyLocations = nHumanSettlements;

            if (map.automatic)
            {
                autoAI.turnTick();
            }

            if (map.burnInComplete && (map.turn - map.param.mapGen_burnInSteps) % map.param.overmind_enthrallmentUseRegainPeriod == 0)
            {
                if (availableEnthrallments < map.param.overmind_maxEnthralled)
                {
                    availableEnthrallments += 1;
                }
            }

            if (map.data_globalTempSum <= map.data_globalTempInitial / 2)
            {
                SteamManager.unlockAchievement(SteamManager.achievement_key.WORLD_UNDER_ICE);
            }
            map.data_globalTempSum = 0;
            computeLightbringer();
        }

        public void computeLightbringer()
        {
            if (lightbringerCasters != null && lightbringerCasters.isGone())
            {
                map.world.prefabStore.popMsg("The Lightbringer Ritual is interrupted and ruined, as the society which was casting it, " + lightbringerCasters.getName() + " is gone. You are safe for now.");
                lightRitualProgress = 0;
                lightbringerCasters = null;
                lightbringerLocations.Clear();
                return;
            }
            if (lightbringerCasters != null)
            {
                lightRitualProgress += 1;
                if (lightRitualProgress >= map.param.awareness_turnsForLightRitual)
                {
                    //We're done
                    int nHeld = this.computeLightbringerHeldLocations();
                    if (nHeld >= lightbringerLocations.Count / 2d)
                    {
                        lightRitualProgress = 0;
                        lightbringerCasters = null;
                        lightbringerLocations.Clear();
                        defeat();
                    }
                    else
                    {
                        map.world.prefabStore.popMsg("You have averted the effects of this Lightbringer Ritual, as the society known as " + lightbringerCasters.getName() + " does not hold sufficient "
                            + "ritual locations when casting ended.");
                        lightRitualProgress = 0;
                        lightbringerCasters = null;
                        lightbringerLocations.Clear();
                    }

                }
            }
        }

        public void startedComplete()
        {
            nStartingHumanSettlements = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc is Society && loc.settlement != null && loc.settlement.isHuman)//Note we don't count ruins, as they will quickly be lost
                {
                    nStartingHumanSettlements += 1;
                }
            }
            map.data_globalTempInitial = 0;
            foreach (Hex[] hexRow in map.grid)
            {
                foreach (Hex hex in hexRow)
                {
                    map.data_globalTempInitial += hex.getTemperature();
                }
            }
            availableEnthrallments = Math.Min(2, map.param.overmind_maxEnthralled);
            World.log("Human settlements computed. n: " + nStartingHumanSettlements);
        }

        public void progressToNextAge()
        {
            map.automatic = false;
            map.world.displayMessages = false;
            map.burnInComplete = false;
            panicFromCluesDiscovered = 0;
            panicFromPowerUse = 0;
            List<Location> monsters = new List<Location>();
            foreach (Location loc in map.locations)
            {
                if (loc.soc != null && loc.soc.isDark() && (loc.soc is Society == false))
                {
                    monsters.Add(loc);
                }
            }
            lightbringerCasters = null;
            lightRitualProgress = 0;
            lightbringerLocations.Clear();
            while (monsters.Count > 3)
            {
                int q = Eleven.random.Next(monsters.Count);
                Location loc = monsters[q];
                loc.soc = null;
                if (loc.settlement != null)
                {
                    loc.settlement = null;
                }
                monsters.RemoveAt(q);
            }
            int lastIndexPerson = 0;
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is Society)
                {
                    Society soc = (Society)sg;
                    foreach (Person p in soc.people)
                    {
                        lastIndexPerson = Math.Max(p.index, lastIndexPerson);
                    }
                    soc.isDarkEmpire = false;
                }
            }
            if (enthralled != null)
            {
                enthralled.die("Died as the age changed", false);
            }

            int burnTurns = 300;
            for (int i = 0; i < burnTurns; i++)
            {
                map.turnTick();

                if (i % 25 == 0)
                {
                    bool hasHumanity = false;
                    foreach (Location loc in map.locations)
                    {
                        if (loc.isOcean) { continue; }
                        if (loc.soc is Society && loc.settlement is SettlementHuman)
                        {
                            hasHumanity = true;
                        }
                    }
                    if (!hasHumanity)
                    {
                        foreach (Location loc in map.locations)
                        {
                            if (loc.isOcean) { continue; }
                            if (loc.hex.getHabilitability() > map.param.mapGen_minHabitabilityForHumans && loc.isMajor)
                            {
                                loc.settlement = new Set_City(loc);

                                Society soc = new Society(map, loc);
                                soc.setName(loc.shortName);
                                loc.soc = soc;
                                map.socialGroups.Add(soc);
                            }
                        }
                    }

                    if (map.data_avrgEnshadowment > 0.1)
                    {
                        foreach (SocialGroup sg in map.socialGroups)
                        {
                            if (sg is Society)
                            {
                                Society soc = (Society)sg;
                                List<Person> dead = new List<Person>();
                                foreach (Person p in soc.people)
                                {
                                    if (p.state == Person.personState.broken && Eleven.random.Next(2) == 0)
                                    {
                                        dead.Add(p);
                                    }else if (p.shadow > 0 && Eleven.random.Next(2) == 0)
                                    {
                                        p.shadow = 0;
                                    }
                                }
                                foreach (Person p in dead)
                                {
                                    p.die("Died of old age", false);
                                }
                            }
                        }
                    }
                }
                foreach (SocialGroup sg in map.socialGroups)
                {
                    if (sg is Society)
                    {
                        Society soc = (Society)sg;
                        List<Person> dead = new List<Person>();
                        foreach (Person p in soc.people)
                        {
                            if (p.index <= lastIndexPerson)
                            {
                                if (p.index % 100 == i)
                                {
                                    dead.Add(p);
                                }
                            }
                        }
                        foreach (Person p in dead)
                        {
                            p.die("Died of old age", false);
                        }
                    }
                }

                if (i == burnTurns / 2)
                {
                    List<Unit> units = new List<Unit>();
                    units.AddRange(map.units);
                    foreach (Unit u in units)
                    {
                        u.die(map,"Old age");
                    }
                }
            }
            map.burnInComplete = true;
            map.world.displayMessages = true;
            startedComplete();
            endOfGameAchieved = false;
            map.firstPlayerTurn = map.turn;
        }

        public List<MsgEvent> getThreats()
        {
            List<MsgEvent> reply = new List<MsgEvent>();

            List<Unit> agents = new List<Unit>();
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled() && (!u.automated)) { agents.Add(u); }
                else
                {
                    if (u.task is Task_Investigate)
                    {
                        reply.Add(new MsgEvent(u.getName() + " is investigating evidence at " + u.location.getName(), MsgEvent.LEVEL_ORANGE, false,u.location.hex));
                    }
                    else if (u.task is Task_ShareSuspicions)
                    {
                        reply.Add(new MsgEvent(u.getName() + " is warning the noble at " + u.location.getName(), MsgEvent.LEVEL_ORANGE, false,u.location.hex));

                    }
                    else if (u.task is Task_InvestigateNoble)
                    {
                        if (u.location.person() != null)
                        {
                            int level = MsgEvent.LEVEL_ORANGE;
                            if (u.location.person().state == Person.personState.enthralled)
                            {
                                level = MsgEvent.LEVEL_RED;
                            }
                            reply.Add(new MsgEvent(u.getName() + " is investigating the noble at " + u.location.getName(),level , false, u.location.hex));

                        }
                    }
                    else if (u.task is Task_InvestigateNobleBasic)
                    {
                        if (u.location.person() != null)
                        {
                            int level = MsgEvent.LEVEL_YELLOW;
                            if (u.location.person().state == Person.personState.enthralled)
                            {
                                level = MsgEvent.LEVEL_ORANGE;
                            }
                            reply.Add(new MsgEvent(u.getName() + " is investigating the noble at " + u.location.getName(), level, false, u.location.hex));

                        }
                    }
                }
            }
            int[] suspicions = new int[agents.Count];

            foreach (SocialGroup sg in map.socialGroups)
            {
                bool relevant = false;
                if (sg.hasEnthralled())
                {
                    relevant = true;
                }
                if (sg is Society)
                {
                    Society soc = (Society)sg;
                    if (soc.isDarkEmpire)
                    {
                        relevant = true;
                    }

                    foreach (Person p in soc.people)
                    {
                        if (p.state == Person.personState.enthralled || p.state == Person.personState.broken) { continue; }

                        for (int i = 0; i < agents.Count; i++)
                        {
                            Unit u = agents[i];
                            if (u.person != null)
                            {
                                if (p.getRelation(u.person).suspicion > 0)
                                {
                                    suspicions[i] += 1;
                                }
                            }
                        }
                    }

                    if (soc.voteSession != null)
                    {
                        if (soc.voteSession.issue is VoteIssue_CondemnAgent)
                        {
                            VoteIssue_CondemnAgent issue = (VoteIssue_CondemnAgent)soc.voteSession.issue;
                            if (issue.target.isEnthralled())
                            {
                                reply.Add(new MsgEvent(soc.getName() + " is voting to condemn " + issue.target.getName(), MsgEvent.LEVEL_RED, false,issue.target.location.hex));
                            }
                        }
                    }
                }
                else
                {
                    if (sg.isDark()) { relevant = true; }
                }

                if (relevant)
                {
                    foreach (SocialGroup s2 in map.socialGroups)
                    {
                        if (s2 == sg) { continue; }
                        if (s2 is Society)
                        {
                            Society soc = (Society)s2;
                            Hex hex = null;
                            if (soc.capital != null) { hex = soc.capital.hex; }
                            if (soc.offensiveTarget == sg)
                            {
                                if (soc.posture == Society.militaryPosture.offensive)
                                {
                                    int level = MsgEvent.LEVEL_RED;
                                    if (soc.currentMilitary < sg.currentMilitary * 0.75) { level = MsgEvent.LEVEL_ORANGE; }
                                    if (soc.currentMilitary < sg.currentMilitary * 0.25) { level = MsgEvent.LEVEL_YELLOW; }
                                    reply.Add(new MsgEvent(soc.getName() + " is offensive and targetting " + sg.getName(), MsgEvent.LEVEL_RED, false, hex));
                                    if (soc.voteSession != null && soc.voteSession.issue is VoteIssue_DeclareWar)
                                    {
                                        reply.Add(new MsgEvent(soc.getName() + " is voting to declare war on " + sg.getName(), MsgEvent.LEVEL_RED, false, hex));
                                    }
                                }
                                else
                                {
                                    reply.Add(new MsgEvent(soc.getName() + " is targetting " + sg.getName(), MsgEvent.LEVEL_ORANGE, false, hex));
                                }
                            }
                            else
                            {
                                int nThreat = 0;
                                foreach (Person p in soc.people)
                                {
                                    ThreatItem t = p.getGreatestThreat();
                                    if (t != null && t.group == sg)
                                    {
                                        nThreat += 1;
                                    }
                                }
                                if (nThreat > 0)
                                {
                                    reply.Add(new MsgEvent(nThreat + " nobles from " + soc.getName() + " consider " + sg.getName() + " their greatest threat", MsgEvent.LEVEL_YELLOW, false,hex));
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < agents.Count; i++)
            {
                if (suspicions[i] > 0)
                {
                    reply.Add(new MsgEvent(suspicions[i] + " nobles are suspicious of " + agents[i].getName(), MsgEvent.LEVEL_YELLOW, false));

                }
            }

            return reply;
        }

        public void victory()
        {
            endOfGameAchieved = true;
            AchievementManager.unlockAchievement(SteamManager.achievement_key.VICTORY);
            World.log("VICTORY DETECTED");
            map.world.prefabStore.popVictoryBox();

            if (!map.hasEnthralledAnAgent)
            {
                AchievementManager.unlockAchievement(SteamManager.achievement_key.POLITICS_ONLY);
            }
        }
        public void defeat()
        {
            endOfGameAchieved = true;
            World.log("DEFEAT DETECTED");
            map.world.prefabStore.popDefeatBox();
        }

        public int computeLightbringerHeldLocations()
        {
            int n = 0;
            foreach (Location loc in lightbringerLocations)
            {
                if (loc.person() != null && loc.person().state == Person.personState.normal && loc.person().shadow < 0.5)
                {
                    n += 1;
                }
            }
            return n;
        }

        public void processEnthralled()
        {
            if (enthralled == null) { return; }

            if (enthralled.isDead) { enthralled = null; }
        }
        public int countAvailableAbilities(Hex hex)
        {
            if (hex == null) { return 0; }
            if (hex.location == null) { return 0; }
            int n = 0;
            foreach (Ability a in abilities)
            {
                if (a.castable(map, hex))
                {
                    n += 1;
                }
            }
            return n;
        }
        public int countAvailablePowers(Hex hex)
        {
            if (hex == null) { return 0; }
            if (hex.location == null) { return 0; }
            int n = 0;
            foreach (Ability a in powers)
            {
                if (a.castable(map, hex))
                {
                    n += 1;
                }
            }
            return n;
        }
        public List<Ability> getAvailableAbilities(Hex hex)
        {
            if (hex == null) { return new List<Ability>(); }
            if (hex.location == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in abilities)
            {
                if (a.castable(map, hex))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public List<Ability> getAvailablePowers(Hex hex)
        {
            if (hex == null) { return new List<Ability>(); }
            if (hex.location == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in powers)
            {
                if (a.castable(map, hex))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public int countAvailableAbilities(Person p)
        {
            if (p == null) { return 0; }
            int n = 0;
            foreach (Ability a in abilities)
            {
                if (a.castable(map, p))
                {
                    n += 1;
                }
            }
            return n;
        }
        public int countAvailablePowers(Person p)
        {
            if (p == null) { return 0; }
            int n = 0;
            foreach (Ability a in powers)
            {
                if (a.castable(map, p))
                {
                    n += 1;
                }
            }
            return n;
        }
        public List<Ability> getAvailableAbilities(Person p)
        {
            if (p == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in abilities)
            {
                if (a.castable(map, p))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public List<Ability> getAvailablePowers(Person p)
        {
            if (p == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in powers)
            {
                if (a.castable(map, p))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public int countAvailableAbilities(Unit p)
        {
            if (p == null) { return 0; }
            int n = 0;

            if (p.isEnthralled())
            {
                foreach (Ability a in p.abilities)
                {
                    if (a.castable(map, p))
                    {
                        n += 1;
                    }
                }
                return n;
            }

            foreach (Ability a in abilities)
            {
                if (a.castable(map, p))
                {
                    n += 1;
                }
            }
            return n;
        }
        public int countAvailablePowers(Unit p)
        {
            if (p == null) { return 0; }
            int n = 0;

            if (p.isEnthralled())
            {
                foreach (Ability a in p.powers)
                {
                    if (a.castable(map, p))
                    {
                        n += 1;
                    }
                }
                return n;
            }

            foreach (Ability a in powers)
            {
                if (a.castable(map, p))
                {
                    n += 1;
                }
            }
            return n;
        }
        public List<Ability> getAvailableAbilities(Unit p)
        {
            if (p == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            if (p.isEnthralled())
            {
                foreach (Ability a in p.abilities)
                {
                    if (a.castable(map, p))
                    {
                        reply.Add(a);
                    }
                }
            }
            else
            {
                foreach (Ability a in abilities)
                {
                    if (a.castable(map, p))
                    {
                        reply.Add(a);
                    }
                }
            }
            return reply;
        }
        public List<Ability> getAvailablePowers(Unit p)
        {
            if (p == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            if (p.isEnthralled())
            {
                foreach (Ability a in p.powers)
                {
                    if (a.castable(map, p))
                    {
                        reply.Add(a);
                    }
                }
            }
            else
            {
                foreach (Ability a in powers)
                {
                    if (a.castable(map, p))
                    {
                        reply.Add(a);
                    }
                }
            }
            return reply;
        }
    }
}
