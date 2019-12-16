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

        public int instabilityTurns;
        public double data_loyalLordsCap;
        public double data_rebelLordsCap;
        public int turnsNotInOffensiveStance;
        public int turnSovreignAssigned = -1;

        public bool isDarkEmpire = false;

        public LogBox logbox;
        public double data_societalStability;

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
                if (isAtWar())
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
            if (capital.settlement.location.soc != this) { computeCapital(); }
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
            if (!hasWar)
            {
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        if (loc.isForSocieties == false || loc.hex.getHabilitability() < map.param.mapGen_minHabitabilityForHumans)
                        {
                            if (loc.settlement != null)
                            {
                                loc.settlement = null;
                            }
                            loc.soc = null;
                        }

                        if (loc.isForSocieties && loc.settlement == null && loc.isForSocieties && loc.hex.getHabilitability() >= map.param.mapGen_minHabitabilityForHumans)
                        {
                            if (loc.isMajor)
                            {
                                loc.settlement = new Set_City(loc);
                            }
                            else
                            {
                                loc.settlement = new Set_Fort(loc);
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

                    voteSession.issue.changeLikingForVotes(option);
                }

                if (World.logging && logbox != null) { logbox.takeLine("End voting on " + voteSession.issue.ToString()); }
                if (World.logging && logbox != null) { logbox.takeLine("    Winning option: " + winner.info()); }
                voteSession.issue.implement(winner);
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
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.settlement != null && loc.settlement.title != null && loc.settlement.title.heldBy == null)
                    {
                        unclaimedTitles.Add(loc.settlement.title);
                    }
                }
            }


            foreach (Title t in titles)
            {
                t.turnTick();
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
                Person p = new Person(this);
                log(p.getFullName() + " has risen to note in the society of " + this.getName());
                people.Add(p);
            }
            
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
