﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Society : SocialGroup
    {
        public List<Person> people = new List<Person>();
        public Title sovereign;
        public List<Title> titles = new List<Title>();

        public List<TitleLanded> unclaimedTitles = new List<TitleLanded>();

        public enum militaryPosture { introverted, defensive, offensive };
        public militaryPosture posture = militaryPosture.introverted;
        public SocialGroup defensiveTarget;
        public SocialGroup offensiveTarget;
        public VoteSession voteSession;
        public int voteCooldown = 0;
        public List<EconEffect> econEffects;
        public bool isRebellion = false;
        public List<KillOrder> killOrders = new List<KillOrder>();
        public List<Zeit> zeits = new List<Zeit>();
        public Location capital;
        public List<House> houses = new List<House>();

        public int billsSinceLastSettlementAssignment;
        public int instabilityTurns;
        public double data_loyalLordsCap;
        public double data_rebelLordsCap;
        public int turnsNotInOffensiveStance;
        public int turnSovereignAssigned = -1;

        public bool needsToDecreasePopulation = false;
        public bool isDarkEmpire = false;

        public string allianceName = null;

        public LogBox logbox;
        public double data_societalStability;
        public int data_nProvinceRulers;
        public List<Unit> enemies = new List<Unit>();

        //public double dread_agents_evidenceFound = 0;
        public int electionID = 0;//Used by characters to check if they're still using valid data

        public SocType socType = new SocType_ElectiveMonarchy();

        public HashSet<Evidence> evidenceSubmitted = new HashSet<Evidence>();
        public HashSet<Evidence> handledEvidence = new HashSet<Evidence>();
        public int lastEvidenceSubmission = 0;
        public int lastEvidenceResponse = 0;
        public int lastPlagueCrisis;

        //These are flags for the AI, which will automatically trigger a response if it finds these set
        public string crisisWarShort = null;
        public string crisisWarLong = null;
        public string crisisPlague = null;
        public string crisisPlagueLong = null;
        public bool crisisNobles = false;
        public bool crisisWitchHunt = false;
        public int lastNobleCrisis;

        public int cooldownLastMilitarySwitch = 0;
        public int cooldownLastDefTargetSwitch = 0;
        public int cooldownLastOffTargetSwitch = 0;

        public Society(Map map, Location location) : base(map)
        {
            setName("DEFAULT_SOC_NAME");
            sovereign = new Title_Sovereign(this);
            titles.Add(sovereign);
            econEffects = new List<EconEffect>();

            if (map.simplified)
            {
                socType = new SocType_Monarchy();
            }

            for (int i = 0; i < 3; i++)
            {
                House house = new House();
                house.name = TextStore.getName(false);
                house.background = Eleven.random.Next(World.self.textureStore.layerBack.Count);
                house.culture = map.sampleCulture(location);
                houses.Add(house);
            }
        }

        public override void turnTick()
        {
            base.turnTick();
            debug();
            processExpirables();
            processKillOrders();
            processStability();
            checkTitles();
            processTerritoryExchanges();
            processVoting();
            checkPopulation();//Add people last, so new people don't suddenly arrive and act before the player can see them
            checkAssertions();
            misc();
            log();
        }

        public int getLevel()
        {
            if (people.Count > World.staticMap.param.society_nPeopleForEmpire)
            {
                return 2;
            }
            else if (people.Count > World.staticMap.param.society_nPeopleForKingdom)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public override string getTypeName()
        {
            return socType.getName();
        }
        public override string getTypeDesc()
        {
            string reply = socType.getDesc();



            reply += "\n\nThis society's unlanded titles are:\n";
            foreach (Title t in titles)
            {
                reply += t.getName() + "\n";
                if (t.heldBy != null)
                {
                    reply += "  Held by: " + t.heldBy.getFullName() + "\n";
                    int turnsRemaining = map.param.society_minTimeBetweenTitleReassignments - (map.turn - t.turnLastAssigned);
                    if (turnsRemaining < 0) { turnsRemaining = 0; }
                    reply += "     Turns till election: " + turnsRemaining + "\n";
                }
                else
                {
                    reply += "  Currently Vacant\n";
                    int turnsRemaining = map.param.society_minTimeBetweenTitleReassignments - (map.turn - t.turnLastAssigned);
                    if (turnsRemaining < 0) { turnsRemaining = 0; }
                    reply += "     Turns till election: " + turnsRemaining + "\n";
                }
            }

            return reply;
        }

        public void misc()
        {
            if (posture == militaryPosture.offensive)
            {
                turnsNotInOffensiveStance = 0;
            }
            else
            {
                turnsNotInOffensiveStance += 1;
            }

            //dread_agents_evidenceFound -= map.param.threat_agentFearDecayPerTurn;
            //if (dread_agents_evidenceFound < 0) { dread_agents_evidenceFound = 0; }
        }

        public void checkAssertions()
        {
            if (titles.Count == 0) { throw new Exception("Sovereign title not present"); }
        }

        public override bool hostileTo(Unit u)
        {
            if (this.getRel(u.society).state == DipRel.dipState.war) { return true; }
            return enemies.Contains(u);
        }

        public void log()
        {
            if (World.logging)
            {
                if (logbox == null)
                {
                    logbox = new LogBox(this);
                }
                logbox.takeLine("--------Turn " + map.turn + "------");
            }
        }

        public void processKillOrders()
        {
            foreach (KillOrder order in killOrders)
            {
                if (people.Contains(order.person))
                {
                    bool makePumpkin = false;
                    if (order.votedByNobles)
                    {
                        foreach (Unit u in map.units)
                        {
                            if (u is Unit_HeadlessHorseman)
                            {
                                makePumpkin = true;
                            }
                        }
                    }
                    if (order.person.shadow > 0.3) { makePumpkin = false; }
                    if (order.person.state != Person.personState.normal && order.person.state != Person.personState.lightbringer) { makePumpkin = false; }
                    if (makePumpkin)
                    {
                        Property.addProperty(map, order.person.getLocation(), "Pumpkin");
                        map.world.prefabStore.popImgMsg("The society of " + getName() + " has executed " + order.person.getFullName() + "! Their head-pumpkin is now available in " +
                            order.person.getLocation().getName() + " for the horseman to steal!",
                            "\"If you take my head, I'll take yours! All of yours!\"", 3);
                    }

                    order.person.die("Executed by " + this.getName() + ". Reason: " + order.reason, true);

                }
            }
        }
        public void processStability()
        {
            data_loyalLordsCap = 0;
            data_rebelLordsCap = 0;
            if (getSovereign() == null)
            {
                instabilityTurns = 0;
                return;
            }

            List<Person> rebels = new List<Person>();
            foreach (Person p in people)
            {
                if (p.title_land == null) { continue; }
                bool isRebel = p.getRelation(getSovereign()).getLiking() <= map.param.society_rebelLikingThreshold;
                if (p.state == Person.personState.enthralled)
                {
                    isRebel = p.rebellingFrom == p.society;
                }
                if (p.society.getSovereign() == p) { isRebel = false; }
                if (isRebel)
                {
                    data_rebelLordsCap += p.title_land.settlement.getMilitaryCap();
                    rebels.Add(p);
                }
                else
                {
                    data_loyalLordsCap += p.title_land.settlement.getMilitaryCap();
                }
            }

            if (posture == militaryPosture.introverted)
            {
                data_loyalLordsCap *= map.param.society_introversionStabilityGain;
            }


            if (data_loyalLordsCap + data_rebelLordsCap <= 0)
            {
                data_societalStability = 1;
            }
            else
            {
                data_societalStability = (data_loyalLordsCap - data_rebelLordsCap) / (data_loyalLordsCap + data_rebelLordsCap);
            }

            if (rebels.Count > 0 && data_rebelLordsCap >= data_loyalLordsCap)//Gotta ensure there exist rebels, or you can rebel against yourself by dropping your cap to zero
            {
                instabilityTurns += 1;
                int turnsTillCivilWar = map.param.society_instablityTillRebellion - instabilityTurns;

                if (turnsTillCivilWar <= 0)
                {
                    triggerCivilWar(rebels);
                }
                else
                {
                    int level = MsgEvent.LEVEL_ORANGE;
                    bool goodThing = true;
                    if (this.hasEnthralled())
                    {
                        goodThing = false;
                        level = MsgEvent.LEVEL_RED;
                    }
                    Hex hex = null;
                    if (getCapital() != null) { hex = getCapital().hex; }
                    map.addMessage(this.getName() + " is unstable, as too many powerful nobles oppose the sovereign. " + turnsTillCivilWar + " turns till civil war.",
                        level, goodThing,hex);
                }
            }
            else
            {
                instabilityTurns = 0;
            }
        }

        public void triggerCivilWar(List<Person> rebelsTotal)
        {
            World.log(this.getName() + " falls into civil war as " + rebelsTotal.Count + " out of " + people.Count + " nobles declare rebellion against " + getSovereign().getFullName());

            int level = MsgEvent.LEVEL_ORANGE;
            bool goodThing = true;
            if (this.hasEnthralled())
            {
                goodThing = false;
                level = MsgEvent.LEVEL_RED;
            }
            Hex hex = null;
            if (getCapital() != null) { hex = getCapital().hex; }
            map.addMessage(this.getName() + " falls into civil war! Provinces declare war on the sovereign's loyal forces.",
                level, goodThing,hex);
            this.posture = militaryPosture.offensive;//Flip to offensive to recapture lost territory

            List<Province> seenProvinces = new List<Province>();
            List<List<Person>> rebelsByProvince = new List<List<Person>>();
            List<Person> unmappedRebels = new List<Person>();
            foreach (Person p in rebelsTotal)
            {
                if (p.title_land == null) { unmappedRebels.Add(p); continue; }
                if (seenProvinces.Contains(p.title_land.settlement.location.province))
                {
                    int ind = seenProvinces.IndexOf(p.title_land.settlement.location.province);
                    rebelsByProvince[ind].Add(p);
                }
                else
                {
                    seenProvinces.Add(p.title_land.settlement.location.province);
                    rebelsByProvince.Add(new List<Person>());
                    rebelsByProvince[rebelsByProvince.Count - 1].Add(p);
                }
            }

            if (rebelsByProvince.Count == 0)
            {
                World.log("No rebels had any territory. Rebellion called off");
                return;
            }

            rebelsByProvince[0].AddRange(unmappedRebels);

            World.log("Rebellion has " + seenProvinces.Count + " provinces");

            for (int k = 0; k < seenProvinces.Count; k++)
            {
                List<Person> rebels = rebelsByProvince[k];
                Location startLocation = null;
                if (rebels[0].title_land != null) { startLocation = rebels[0].title_land.settlement.location; }
                Society rebellion = new Society(map, startLocation);
                map.socialGroups.Add(rebellion);
                rebellion.setName(seenProvinces[k].name + " rebellion");
                rebellion.isRebellion = true;
                if (Eleven.random.Next(2) == 0)
                {
                    rebellion.posture = militaryPosture.defensive;
                }
                foreach (Person p in rebels)
                {
                    if (p.title_land != null)
                    {
                        p.title_land.settlement.location.soc = rebellion;

                        if (p.title_land.settlement.attachedUnit != null)
                        {
                            p.title_land.settlement.attachedUnit.society = rebellion;
                        }
                        if (p.title_land.settlement.embeddedUnit != null)
                        {
                            p.title_land.settlement.embeddedUnit.society = rebellion;
                        }
                    }
                    this.people.Remove(p);
                    rebellion.people.Add(p);
                    p.society = rebellion;
                }

                double proportionalStrength = 0;
                rebellion.computeMilitaryCap();
                this.computeMilitaryCap();

                if (this.maxMilitary > 0 || rebellion.maxMilitary > 0)
                {
                    proportionalStrength = this.maxMilitary / (this.maxMilitary + rebellion.maxMilitary);
                    rebellion.currentMilitary = this.currentMilitary * (1 - proportionalStrength);
                    this.currentMilitary = this.currentMilitary * proportionalStrength;
                }

                if (getSovereign() != null)
                {
                    KillOrder killSovereign = new KillOrder(getSovereign(), "Rebellion against tyranny");
                    rebellion.killOrders.Add(killSovereign);
                }

                foreach (Person p in rebels)
                {
                    KillOrder killRebel = new KillOrder(p, "Rebelled against sovereign");
                    this.killOrders.Add(killRebel);

                }

                rebellion.posture = militaryPosture.offensive;
                this.map.declareWar(rebellion, this);
            }
        }

        public void processExpirables()
        {
            if (offensiveTarget != null && offensiveTarget.checkIsGone()) { offensiveTarget = null; }

            List<EconEffect> rems = new List<EconEffect>();
            foreach (EconEffect effect in econEffects)
            {
                bool hasFrom = false;
                bool hasTo = false;
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this && loc.settlement != null && loc.settlement.econTraits().Contains(effect.from)) { hasFrom = true; break; }
                }
                if (hasFrom == false) { effect.durationLeft = 0; }//No longer valid
                else
                {
                    foreach (Location loc in map.locations)
                    {
                        if (loc.soc == this && loc.settlement != null && loc.settlement.econTraits().Contains(effect.to)) { hasTo = true; break; }
                    }
                    if (hasTo == false) { effect.durationLeft = 0; }//No longer valid
                }
                if (effect.durationLeft > 0)
                {
                    effect.durationLeft -= 1;
                }
                if (effect.durationLeft == 0) { rems.Add(effect); }
            }
            foreach (EconEffect effect in rems)
            {
                econEffects.Remove(effect);
            }

            if (isRebellion)
            {
                if (!isAtWar())
                {
                    if (this.getCapital() != null)
                    {
                        isRebellion = false;
                        World.log(this.getName() + " has successfully defended itself and broken away properly. Renaming now");
                        this.setName(this.getCapital().shortName);
                    }
                }
            }
        }
        public override bool hasEnthralled()
        {
            foreach (Person p in people)
            {
                if (p.state == Person.personState.enthralled) { return true; }
            }
            return false;
        }
        public override string getName()
        {
            string basic = base.getName();
            if (this.isDarkEmpire)
            {
                return "Dark Empire of " + basic;
            }
            if (allianceName != null)
            {
                return allianceName;
            }
            if (this.people.Count > map.param.society_nPeopleForEmpire)
            {
                return "Empire of " + basic;
            }
            else if (this.people.Count > map.param.society_nPeopleForKingdom)
            {
                if (this.getSovereign() == null || this.getSovereign().isMale)
                {
                    return "Kingdom of " + basic;
                }
                else
                {
                    return "Queendom of " + basic;
                }
            }
            else if (this.titles.Count > 1)
            {
                return "Grand Duchey of " + basic;
            }
            else
            {
                return "Duchy of " + basic;
            }
        }

        public Location getCapital()
        {
            if (capital == null || capital.soc != this)
            {
                computeCapital();
                return capital;
            }
            if (capital.settlement == null) { computeCapital(); return capital; }
            if (capital.soc != this) { computeCapital(); }
            return capital;
        }

        public void computeCapital()
        {
            if (this.getSovereign() != null)
            {
                if (this.getSovereign().title_land != null)
                {
                    capital = this.getSovereign().title_land.settlement.location;
                    return;
                }
            }
            double bestPrestige = -100000;
            foreach (Location loc in map.locations)
            {
                if (loc.soc != this) { continue; }
                if (loc.settlement == null) { continue; }
                if (loc.settlement.basePrestige > bestPrestige)
                {
                    bestPrestige = loc.settlement.basePrestige;
                    capital = loc;
                }
            }
        }

        public override double getThreat(List<ReasonMsg> msgs)
        {
            double threat = base.getThreat(msgs);
            if (this.posture == militaryPosture.offensive)
            {
                int percent = (int)(100 * map.param.society_threatMultFromOffensivePosture);
                double add = map.param.society_threatMultFromOffensivePosture * threat;
                if (msgs != null)
                {
                    msgs.Add(new ReasonMsg("Offensive Posture (+" + percent + "%)", (int)add));
                }
                threat += add;
            }
            return threat;
        }


        public void processTerritoryExchanges()
        {
            bool hasWar = false;
            foreach (DipRel rel in getAllRelations())
            {
                if (rel.state == DipRel.dipState.war)
                {
                    hasWar = true;
                    break;
                }
            }
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.isForSocieties == false || loc.hex.getHabilitability() < map.param.mapGen_minHabitabilityForHumans)
                    {
                        if (loc.settlement != null)
                        {
                            if (loc.settlement.isHuman)
                            {
                                loc.settlement.fallIntoRuin();
                            }
                        }
                        //Note it remains under our control
                    }
                }
            }
            //Expansion can only happen at peace time
            if (!hasWar)
            {
                //Drop your naval/desert presence
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        if (loc.isForSocieties == false || loc.hex.getHabilitability() < map.param.mapGen_minHabitabilityForHumans)
                        {
                            if (loc.settlement != null)
                            {
                                if (loc.settlement.isHuman)
                                {
                                    loc.settlement.fallIntoRuin();
                                }
                            }
                            loc.soc = null;
                        }
                        else if (loc.settlement == null || loc.settlement is Set_Ruins)
                        {
                            loc.soc = null;
                        }
                    }
                }
            }
        }

        public void debug()
        {
            /*
            if (this.getSize() < 5)
            {
                int nWars = 0;
                foreach (SocialGroup other in getNeighbours())
                {
                    if (getRel(other).state == DipRel.dipState.war)
                    {
                        nWars += 1;
                    }

                }
                if (nWars == 0)
                {
                    int c = 0;
                    SocialGroup choice = null;
                    foreach (SocialGroup other in getNeighbours())
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            choice = other;
                        }
                    }
                    if (choice != null)
                    {
                        map.declareWar(this, choice);
                    }
                }
            }
            */
        }

        public Hex getCapitalHex()
        {
            Location loc = getCapital();
            if (loc != null) { return loc.hex; }
            return null;
        }

        public void processVoting()
        {
            if (voteSession == null)
            {
                if (voteCooldown > 0) { voteCooldown -= 1; return; }

                Person proposer = null;
                foreach (Person p in people)
                {
                    if (p.state == Person.personState.enthralled) { continue; }
                    if (proposer == null)
                    {
                        proposer = p;
                    }
                    else
                    {
                        int myDelta = map.turn - p.lastVoteProposalTurn;
                        int theirDelta = map.turn - proposer.lastVoteProposalTurn;
                        if (myDelta > theirDelta)
                        {
                            proposer = p;
                        }
                    }
                }
                if (proposer != null)
                {
                    proposer.lastVoteProposalTurn = map.turn;
                    VoteIssue issue = proposer.proposeVotingIssue();
                    if (issue == null) { return; }

                    bool positive = true;
                    int priority = (this.hasEnthralled()) ? 1 : 3;
                    string msg = this.getName() + " now voting on " + issue.ToString();

                    World.staticMap.addMessage(msg, priority, positive, getCapitalHex());

                    //Otherwise, on with voting for this new thing
                    voteSession = new VoteSession();
                    voteSession.issue = issue;
                    voteSession.timeRemaining = map.param.society_votingDuration;
                    if (World.logging && logbox != null) { logbox.takeLine("Starting voting on " + voteSession.issue.ToString()); }

                    if (this.hasEnthralled())
                    {
                        voteSession.assignVoters();
                        VoteOption optionChoice = null;
                        foreach (VoteOption opt in voteSession.issue.options)
                        {
                            if (opt.votesFor.Contains(proposer))
                            {
                                optionChoice = opt;
                                break;
                            }
                        }
                        string str = proposer.getFullName() + " has proposed a measure to be voted on by the nobles of "
                            + this.getName() + ".\nThey are voting " +
                            optionChoice.info(issue);

                        map.world.prefabStore.popImgMsg(str, "Voting issue: " + issue.getLargeDesc());
                    }
                }
            }
            else
            {
                if (voteSession.issue.stillValid(map) == false)
                {
                    voteSession = null;
                    World.log("Vote session no longer valid");
                    return;
                }
                if (voteSession.timeRemaining > 0) { voteSession.timeRemaining -= 1; return; }

                voteSession.assignVoters();

                double topVote = 0;
                VoteOption winner = null;
                foreach (VoteOption option in voteSession.issue.options)
                {
                    if (option.votingWeight > topVote || winner == null)
                    {
                        winner = option;
                        topVote = option.votingWeight;
                    }

                    voteSession.issue.changeLikingForVotes(option, voteSession.issue);
                }

                if (World.logging && logbox != null) { logbox.takeLine("End voting on " + voteSession.issue.ToString()); }
                if (World.logging && logbox != null) { logbox.takeLine("    Winning option: " + winner.info()); }

                if (this.hasEnthralled())
                {
                    string str = "Chosen outcome: " + winner.info(voteSession.issue);
                    str += "\nVotes for: ";
                    foreach (Person p in winner.votesFor)
                    {
                        str += p.getFullName() + ", ";
                    }
                    str = str.Substring(0, str.Length - 2);

                    string strTitle = "Vote Concluded on " + voteSession.issue.ToString();
                    string strSubtitle = "Outcome: " + winner.info(voteSession.issue);
                    map.world.prefabStore.popVoteMsg(strTitle, strSubtitle, voteSession.issue.getLargeDesc());
                    //map.world.prefabStore.popImgMsg(str, "Voting concluded. Issue: " + voteSession.issue.getLargeDesc());
                }

                voteSession.issue.implement(winner);
                if (voteSession.issue is VoteIssue_AssignLandedTitle == false)
                {
                    billsSinceLastSettlementAssignment += 1;
                }
                else
                {
                    billsSinceLastSettlementAssignment = 0;
                }
                voteSession = null;
            }
        }

        public void checkTitles()
        {
            unclaimedTitles.Clear();
            foreach (Person p in people)
            {
                if (p.title_land != null)
                {
                    //Settlement is razed
                    if (p.title_land.settlement.location.settlement != p.title_land.settlement)
                    {
                        log(p.getFullName() + " has lost title " + p.title_land.getName() + " has it no longer exists.");
                        p.title_land.heldBy = null;
                        p.title_land = null;
                        continue;
                    }
                    if (p.title_land.settlement.location.soc != this)
                    {
                        log(p.getFullName() + " has lost title " + p.title_land.getName() + " has it is no longer owned by their society, " + this.getName());
                        p.title_land.heldBy = null;
                        p.title_land = null;
                        continue;
                    }
                }
            }

            int[] provCounts = new int[map.provinces.Count];
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.settlement != null && loc.settlement.title != null && loc.settlement.title.heldBy == null)
                    {
                        unclaimedTitles.Add(loc.settlement.title);
                    }
                    provCounts[loc.province.index] += 1;
                }
            }
            //Compute provincial titles. First we need to see how many we actually retain since last time
            bool[] present = new bool[map.provinces.Count];
            List<Title_ProvinceRuler> rems = new List<Title_ProvinceRuler>();
            foreach (Title t in titles)
            {
                if (t is Title_ProvinceRuler)
                {
                    Title_ProvinceRuler t2 = (Title_ProvinceRuler)t;
                    if (provCounts[t2.province.index] < 2)
                    {
                        rems.Add(t2);
                    }
                    else
                    {
                        present[t2.province.index] = true;
                    }
                }
            }
            foreach (Title_ProvinceRuler t in rems)
            {
                if (t.heldBy != null)
                {
                    if (map.overmind.enthralled != null && t.heldBy.society == map.overmind.enthralled.society)
                    {
                        map.addMessage(t.heldBy.getFullName() + " loses provincial rule over " + t.province.name + " due to loss of territory.",MsgEvent.LEVEL_RED,false, t.province.coreHex);
                    }
                    
                    t.heldBy.titles.Remove(t);
                    t.heldBy = null;
                    titles.Remove(t);
                }
            }
            int nHeldProvinces = 0;

            for (int i = 0; i < provCounts.Length; i++)
            {
                if (provCounts[i] > 1)
                {
                    nHeldProvinces += 1;
                }
            }

            if (titles.Count <= socType.getNDukesMax())
            {
                for (int i = 0; i < provCounts.Length; i++)
                {
                    if (provCounts[i] > 1 && nHeldProvinces > 1 && (!present[i]))
                    {
                        Title_ProvinceRuler title = new Title_ProvinceRuler(this, map.provinces[i]);
                        titles.Add(title);

                        if (map.overmind.enthralled != null && this == map.overmind.enthralled.society)
                        {
                            map.addMessage(this.getName() + " adds a new provincial ruler title, for " + title.province.name, MsgEvent.LEVEL_GREEN, false,title.province.coreHex);
                        }
                    }
                }
            }

            data_nProvinceRulers = 0;
            foreach (Title t in titles)
            {
                t.turnTick();
                if (t is Title_ProvinceRuler)
                {
                    data_nProvinceRulers += 1;
                }
            }
        }

        public void checkPopulation()
        {
            foreach (Person p in people)
            {
                p.turnTick();
            }

            int nUntitled = 0;
            foreach (Person p in people)
            {
                if (p.title_land == null)
                {
                    nUntitled += 1;
                }
            }
            int nNeeded = map.param.soc_untitledPeople - nUntitled;
            //Insta-add enough to make up the numbers
            for (int i=0;i<nNeeded;i++)
            {
                if (people.Count > 0)
                {
                    Person sponsor = people[Eleven.random.Next(people.Count)];
                    Person p = new Person(this);
                    //log(p.getFullName() + " has risen to note in the society of " + this.getName() + ", invited by " + sponsor.getFullName());
                    people.Add(p);
                    p.shadow = sponsor.shadow;
                }
                else
                {
                    Person p = new Person(this);
                    //log(p.getFullName() + " has risen to note in the society of " + this.getName());
                    people.Add(p);
                }
            }

            needsToDecreasePopulation = nUntitled > map.param.soc_untitledPeople;

            /*
            if (nUntitled > map.param.soc_untitledPeople)
            {
                Person lastUntitled = null;
                foreach (Person p in people)
                {
                    if (p.state == Person.personState.enthralled)
                    {
                        continue;
                    }
                    if (p.title_land == null)
                    {
                        lastUntitled = p;
                    }
                }
                if (lastUntitled != null)
                {
                    string str = lastUntitled.getFullName() + " has no title, and has lost lordship in " + this.getName();
                    log(str);
                    World.log(str);
                    people.Remove(lastUntitled);
                    if (this.hasEnthralled()) {
                        map.addMessage(str);
                    }
                }
            }
            */
        }

        public Person getSovereign()
        {
            return sovereign.heldBy;
        }
        public void log(String msg)
        {
            World.log(msg);
        }
    }
}
