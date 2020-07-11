using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Society : SocialGroup
    {
        public List<Person> people = new List<Person>();
        public Title sovreign;
        public List<Title> titles = new List<Title>();

        public List<TitleLanded> unclaimedTitles = new List<TitleLanded>();

        public enum militaryPosture { introverted,defensive,offensive};
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

        public int billsSinceLastSettlementAssignment;
        public int instabilityTurns;
        public double data_loyalLordsCap;
        public double data_rebelLordsCap;
        public int turnsNotInOffensiveStance;
        public int turnSovreignAssigned = -1;

        public bool needsToDecreasePopulation = false;
        public bool isDarkEmpire = false;

        public string allianceName = null;

        public LogBox logbox;
        public double data_societalStability;
        public int data_nProvinceRulers;
        public List<Unit> enemies = new List<Unit>();

        public int lastOffensiveTargetSetting;

        public Society(Map map) : base(map)
        {
            setName("DEFAULT_SOC_NAME");
            sovreign = new Title_Sovreign(this);
            titles.Add(sovreign);
            econEffects = new List<EconEffect>();
        }

        public override void turnTick()
        {
            base.turnTick();
            debug();
            processExpirables();
            processKillOrders();
            processStability();
            checkTitles();
            processWarExpansion();
            processTerritoryExchanges();
            processVoting();
            checkPopulation();//Add people last, so new people don't suddenly arrive and act before the player can see them
            checkAssertions();
            misc();
            log();
        }

        public override string getTypeName()
        {
            return "Elective Monarchy";
        }
        public override string getTypeDesc()
        {
            string reply = "A human society, consisting of large numbers of serfs, and the nobles who rule over them."
                + "\nThis society is an elective monarchy, that is to say that the nobles vote on their rulers, selecting a single noble to act as sovreign."
                + "\nIf the nation owns enough territory outside the sovreign's province, they will elect dukes to rule over provinces. If they do, the sovreign can only be elected from the dukes (or be re-elected)"
                + " Only nobles residing in a province are eligible to become that province's duke."
                + "\n\nAll the actions (wars, territory allocation, criminal trials...) the society takes are voted on by the nobles, with weight a noble's vote carries equal to their prestige.";

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
        }

        public void checkAssertions()
        {
            if (titles.Count == 0) { throw new Exception("Sovreign title not present"); }
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
                    order.person.die("Executed by " + this.getName() + ". Reason: " + order.reason);
                }
            }
        }
        public void processStability()
        {
            data_loyalLordsCap = 0;
            data_rebelLordsCap = 0;
            if (getSovreign() == null)
            {
                instabilityTurns = 0;
                return;
            }

            List<Person> rebels = new List<Person>();
            foreach (Person p in people)
            {
                if (p.title_land == null) { continue; }
                bool isRebel = p.getRelation(getSovreign()).getLiking() <= map.param.society_rebelLikingThreshold;
                if (p.state == Person.personState.enthralled)
                {
                    isRebel = p.rebellingFrom == p.society;
                }
                if (p.society.getSovreign() == p) {isRebel = false; }
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
                    map.addMessage(this.getName() + " is unstable, as too many powerful nobles oppose the sovreign. " + turnsTillCivilWar + " turns till civil war.",
                        level,goodThing);
                }
            }
            else
            {
                instabilityTurns = 0;
            }
        }

        public void triggerCivilWar(List<Person> rebelsTotal)
        {
            World.log(this.getName() + " falls into civil war as " + rebelsTotal.Count + " out of " + people.Count + " nobles declare rebellion against " + getSovreign().getFullName());

            int level = MsgEvent.LEVEL_ORANGE;
            bool goodThing = true;
            if (this.hasEnthralled())
            {
                goodThing = false;
                level = MsgEvent.LEVEL_RED;
            }
            map.addMessage(this.getName() + " falls into civil war! Provinces declare war on the sovreign's loyal forces.",
                level, goodThing);
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
                Society rebellion = new Society(map);
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

                if (getSovreign() != null)
                {
                    KillOrder killSovreign = new KillOrder(getSovreign(), "Rebellion against tyranny");
                    rebellion.killOrders.Add(killSovreign);
                }

                foreach (Person p in rebels)
                {
                    KillOrder killRebel = new KillOrder(p, "Rebelled against sovreign");
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
                    if (loc.soc == this && loc.settlement != null && loc.settlement.econTraits().Contains(effect.from)){ hasFrom = true; break; }
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
                if (this.getSovreign() == null || this.getSovreign().isMale)
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
            if (capital.settlement == null) { computeCapital();return capital; }
            if (capital.soc != this) { computeCapital(); }
            return capital;
        }

        public void computeCapital()
        {
            if (this.getSovreign() != null)
            {
                if (this.getSovreign().title_land != null)
                {
                    capital =  this.getSovreign().title_land.settlement.location;
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
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        if (loc.isForSocieties && loc.settlement == null && loc.isForSocieties && loc.hex.getHabilitability() >= map.param.mapGen_minHabitabilityForHumans)
                        {
                            if (loc.isMajor)
                            {
                                loc.settlement = new Set_City(loc);
                            }
                            else
                            {
                                int q = 0;
                                double[] weights = new double[] { 2, 1, 2 };
                                double roll = 0;
                                for (int i = 0; i < weights.Length; i++) { roll += weights[i]; }
                                roll *= Eleven.random.NextDouble();
                                for (int i = 0; i < weights.Length; i++) { roll -= weights[i]; if (roll <= 0) { q = i; break; } }
                                
                                if (q == 0)
                                {
                                    loc.settlement = new Set_Abbey(loc);
                                }
                                else if (q == 1)
                                {
                                    loc.settlement = new Set_University(loc);
                                }
                                else
                                {
                                    loc.settlement = new Set_Fort(loc);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Expand into lost territory
                        foreach (Location l2 in loc.getNeighbours())
                        {
                            if (l2.soc == null && l2.isForSocieties && l2.hex.getHabilitability() > map.param.mapGen_minHabitabilityForHumans)
                            {
                                if (Eleven.random.NextDouble() < map.param.society_pExpandIntoEmpty)
                                {
                                    l2.soc = this;
                                }
                            }
                        }
                    }
                }

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
                    }
                }
            }
        }
        //Check to see if you need to build outposts to reach an enemy you're at war with
        public void processWarExpansion()
        {
            List<SocialGroup> neighbours = this.getNeighbours();
            foreach (DipRel rel in getAllRelations())
            {
                if (rel.state == DipRel.dipState.war && rel.war.att == this)
                {
                    if (neighbours.Contains(rel.other(this)) == false)
                    {
                        //We need to build an outpost towards them
                        Location[] pathTo = map.getEmptyPathTo(this, rel.other(this));
                        if (pathTo != null && pathTo.Length > 1 && pathTo[1].soc == null)
                        {
                            pathTo[1].soc = this;
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
                    if (issue == null) {return; }

                    bool positive = true;
                    int priority = (this.hasEnthralled()) ? 1 : 3;
                    string msg = this.getName() + " now voting on " + issue.ToString();

                    World.staticMap.addMessage(msg, priority, positive);

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

                    voteSession.issue.changeLikingForVotes(option,voteSession.issue);
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
                    map.world.prefabStore.popVoteMsg(strTitle,strSubtitle,voteSession.issue.getLargeDesc());
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
                    }
                    if (p.title_land.settlement.location.soc != this)
                    {
                        log(p.getFullName() + " has lost title " + p.title_land.getName() + " has it is no longer owned by their society, " + this.getName());
                        p.title_land.heldBy = null;
                        p.title_land = null;
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
                        map.addMessage(t.heldBy.getFullName() + " loses provincial rule over " + t.province.name + " due to loss of territory.",MsgEvent.LEVEL_RED,false);
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

            if (titles.Count <= World.staticMap.param.society_maxDukes)
            {
                for (int i = 0; i < provCounts.Length; i++)
                {
                    if (provCounts[i] > 1 && nHeldProvinces > 1 && (!present[i]))
                    {
                        Title_ProvinceRuler title = new Title_ProvinceRuler(this, map.provinces[i]);
                        titles.Add(title);

                        if (map.overmind.enthralled != null && this == map.overmind.enthralled.society)
                        {
                            map.addMessage(this.getName() + " adds a new provincial ruler title, for " + title.province.name, MsgEvent.LEVEL_GREEN, false);
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
                    log(p.getFullName() + " has risen to note in the society of " + this.getName() + ", invited by " + sponsor.getFullName());
                    people.Add(p);
                    p.shadow = sponsor.shadow;
                }
                else
                {
                    Person p = new Person(this);
                    log(p.getFullName() + " has risen to note in the society of " + this.getName());
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

        public Person getSovreign()
        {
            return sovreign.heldBy;
        }
        public void log(String msg)
        {
            World.log(msg);
        }
    }
}
