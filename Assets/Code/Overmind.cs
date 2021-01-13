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
        public bool victoryAchieved = false;
        public bool hasEnthrallAbilities = false;
        public double panicFromPowerUse;
        public double panicFromCluesDiscovered;
        public int nStartingHumanSettlements;
        public bool isFirstEnthralledAgent = true;
        public int availableEnthrallments = 0;
        public int nEnthralled = 0;//Set by the UI. Probably not a great thing to have done, but oh well
        public int maxEnthralled = 3;

        public Overmind(Map map)
        {
            this.map = map;
            maxEnthralled = map.param.overmind_maxEnthralled;

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
                if (u.isEnthralled())
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
                powers.Add(new Ab_Over_CancelVote());
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

        public double computeWorldPanic(List<ReasonMsg> reasons)
        {
            double panic = 0;
            panic += panicFromPowerUse;
            reasons.Add(new ReasonMsg("Power use", panic*100));

            double shadow = map.data_avrgEnshadowment*map.param.panic_panicAtFullShadow;
            panic += shadow;
            reasons.Add(new ReasonMsg("World Shadow", shadow*100));

            panic += panicFromCluesDiscovered;
            reasons.Add(new ReasonMsg("Evidence Discovered", panicFromCluesDiscovered * 100));

            double nHumans = map.data_nSocietyLocations;
            double extinction = (nStartingHumanSettlements - nHumans)/nStartingHumanSettlements;
            extinction *= map.param.panic_panicAtFullExtinction;
            if (extinction < 0) { extinction = 0; }//In the off chance they reclaim something
            panic += extinction;
            reasons.Add(new ReasonMsg("Lost Settlements", extinction*100));

            if (panic > 1) { panic = 1; }
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

            if (detector != null) {
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
            power += map.param.overmind_powerRegen;
            power = Math.Min(power, map.param.overmind_maxPower);


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
                if (loc.person() != null) { sum += loc.person().shadow;count += 1; }
                if (loc.soc != null && loc.settlement != null && (loc.settlement is Set_Ruins == false) && (loc.settlement is Set_CityRuins == false) && loc.soc is Society)
                {
                    nHumanSettlements += 1;
                }
            }
            if (count == 0) { map.data_avrgEnshadowment = 0; }
            else { map.data_avrgEnshadowment = sum / count; }
            if ((!victoryAchieved) && map.data_avrgEnshadowment > map.param.victory_targetEnshadowmentAvrg)
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
                automatic();
            }

            if (map.burnInComplete && (map.turn - map.param.mapGen_burnInSteps) % map.param.overmind_enthrallmentUseRegainPeriod == 0)
            {
                if (availableEnthrallments < map.param.overmind_maxEnthralled)
                {
                    availableEnthrallments += 1;
                }
            }
        }

        public void automatic()
        {
            if (this.power > 0)
            {
                foreach (Unit u in map.units)
                {
                    if (u is Unit_Investigator && u.task is Task_Investigate)
                    {
                        u.task = new Task_Disrupted();
                        power -= map.param.ability_disruptAgentCost;
                        break;
                    }
                }
            }
        }

        public void startedComplete()
        {
            foreach (Location loc in map.locations)
            {
                if (loc.soc is Society && loc.settlement != null && loc.settlement.isHuman)//Note we don't count ruins, as they will quickly be lost
                {
                    nStartingHumanSettlements += 1;
                }
            }
            availableEnthrallments = Math.Min(2, map.param.overmind_maxEnthralled);
            World.log("Human settlements computed. n: " + nStartingHumanSettlements);
        }

        public List<MsgEvent> getThreats()
        {
            List<MsgEvent> reply = new List<MsgEvent>();

            List<Unit> agents = new List<Unit>();
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled()) { agents.Add(u); }
                else
                {
                    if (u.task is Task_Investigate)
                    {
                        reply.Add(new MsgEvent(u.getName() + " is investigating evidence at " + u.location.getName(), MsgEvent.LEVEL_ORANGE, false));
                    }
                    else if (u.task is Task_ShareSuspicions)
                    {
                        reply.Add(new MsgEvent(u.getName() + " is warning the noble at " + u.location.getName(), MsgEvent.LEVEL_ORANGE, false));

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

                        for (int i=0;i<agents.Count;i++)
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
                                reply.Add(new MsgEvent(soc.getName() + " is voting to condemn " + issue.target.getName(), MsgEvent.LEVEL_RED, false));
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
                            if (soc.offensiveTarget == sg)
                            {
                                if (soc.posture == Society.militaryPosture.offensive)
                                {
                                    reply.Add(new MsgEvent(soc.getName() + " is offensive and targetting " + sg.getName(), MsgEvent.LEVEL_RED, false));
                                    if (soc.voteSession != null && soc.voteSession.issue is VoteIssue_DeclareWar)
                                    {
                                        reply.Add(new MsgEvent(soc.getName() + " is voting to declare war on " + sg.getName(), MsgEvent.LEVEL_RED, false));
                                    }
                                }
                                else
                                {
                                    reply.Add(new MsgEvent(soc.getName() + " is targetting " + sg.getName(), MsgEvent.LEVEL_ORANGE, false));
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
                                    reply.Add(new MsgEvent(nThreat + " nobles from " + soc.getName() + " consider " + sg.getName() + " their greatest threat", MsgEvent.LEVEL_YELLOW, false));
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
            victoryAchieved = true;
            AchievementManager.unlockAchievement(SteamManager.achievement_key.VICTORY);
            World.log("VICTORY DETECTED");
            map.world.prefabStore.popVictoryBox();

            if (!map.hasEnthralledAnAgent)
            {
                AchievementManager.unlockAchievement(SteamManager.achievement_key.POLITICS_ONLY);
            }
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
