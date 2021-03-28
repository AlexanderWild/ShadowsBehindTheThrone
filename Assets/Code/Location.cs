using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Location
    {
        public static int indexCounter;
        public int index;
        public Hex hex;
        public Location parent;
        public SocialGroup soc;
        public Settlement settlement;
        public HashSet<Hex> territory = new HashSet<Hex>();
        public List<int[]> territoryStore = new List<int[]>();
        public Color territoryColor;
        public List<Link> links = new List<Link>();
        public List<int> savedLinks = new List<int>();
        public bool isCoastal;
        public bool isOcean;
        public bool isForSocieties = true;
        public Map map;
        public string name;
        public string shortName;
        public bool isMajor;//Remember if it's a city, so we have roughly the same amount at all times, regardless of razing
        public List<Property> properties = new List<Property>();
        public List<Unit> units = new List<Unit>();
        public int turnLastTaken = -1000;
        public double inherentInformationAvailability = 0.85;
        public int lastTaken;
        public Dictionary<SocialGroup,int> distanceToTarget = new Dictionary<SocialGroup, int>();
        public int debugVal;
        //public int turnLastAssigned;
        public Province province;
        public List<Evidence> evidence = new List<Evidence>();
        public Culture culture;

        public EventContext.State eventState = new EventContext.State();

        public Location(Map map, Hex hex,bool isMajor)
        {
            index = indexCounter;
            indexCounter += 1;

            this.isMajor = isMajor;
            this.map = map;
            this.hex = hex;
            name = "Empty Location";
            shortName = "Empty Location";

            territoryColor = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                0.5f);
        }
        

        public string getName()
        {
            string position = " (" + (map.sizeX-this.hex.x) + "," + this.hex.y + ")";
            if (settlement != null) { return settlement.name + position; }
            return name + position;
        }

        public Person person()
        {
            if (settlement == null) { return null; }
            if (settlement.title == null) { return null; }
            return settlement.title.heldBy;
        }

        /**
         * The amount of damage which could theoretically be absorbed by defences
         */
        public double getMaxMilitaryDefence()
        {
            if (settlement != null) { return settlement.getDefensiveMax(); }
            return 0;
        }
        /**
         * The amount of damage which is absorbed by a defensive structure
         */
        public double getMilitaryDefence()
        {
            if (settlement != null) { return settlement.defensiveStrengthCurrent; }
            return 0;
        }
        /**
         * Removes defence from settments first, then allows any overflow damage through
         */
        public double takeMilitaryDamage(double amount)
        {
            if (settlement != null) {
                double soaked = Math.Min(settlement.defensiveStrengthCurrent, amount);
                settlement.defensiveStrengthCurrent -= soaked;
                amount -= soaked;
            }
            return amount;
        }

        public Person getSuperiorInSociety(Society society)
        {
            if (society.getCapital() != null && society.getCapital().province == this.province) { return society.getSovereign(); }
            foreach (Title t in society.titles)
            {
                if (t is Title_ProvinceRuler)
                {
                    Title_ProvinceRuler t2 = (Title_ProvinceRuler)t;
                    if (t2.province == this.province) { return t2.heldBy; }
                }
            }
            return null;
        }

        public List<Location> getNeighbours()
        {
            List<Location> reply = new List<Location>();
            foreach (Link link in links)
            {
                if (link.disabled) { continue; }
                reply.Add(link.other(this));
            }
            return reply;
        }

        public void recomputeLinkDisabling()
        {
            bool underBlizzard = false;
            foreach (Property pr in properties)
            {
                if (pr.proto is Pr_Blizzard) { underBlizzard = true;break; }
            }
            int nDisabled = 0;
            foreach (Link link in links)
            {
                link.disabled = false;

                if (underBlizzard) { 
                    foreach (Property pr in link.other(this).properties)
                    {
                        if (pr.proto is Pr_Blizzard) {
                            //Make sure you're not completely isolating this node
                            int nOtherDisabled = 0;
                            foreach (Link l2 in link.other(this).links)
                            {
                                if (l2.disabled) { nOtherDisabled += 1; }
                            }
                            if (nOtherDisabled < link.other(this).links.Count-1)
                            {
                                link.disabled = true;
                            }
                        }
                    }
                }
                if (link.disabled) { nDisabled += 1; }
            }
            if (nDisabled == links.Count)
            {
                links[0].disabled = false;
            }
        }

        public bool isNeighbour(Location l2)
        {
            foreach (Link link in links)
            {
                if (link.other(this) == l2) { return true; }
            }
            return false;
        }

        public void turnTick()
        {
            if (map.locations[index] != this) { throw new Exception("Assertion failed: locations not consistent"); }
            if (settlement != null)
            {
                settlement.turnTick();
            }
            
            foreach (Property p in properties)
            {
                p.proto.turnTick(p,this);
            }
            checkPropertiesEndOfTurn();

            if (soc != null && soc is Society) {
                foreach (Evidence e in evidence)
                {
                    if (e.assignedInvestigator != null)
                    {
                        if (map.units.Contains(e.assignedInvestigator) == false) { e.assignedInvestigator = null; }//Dead
                        else if (e.assignedInvestigator.location != this && e.assignedInvestigator.task is Task_GoToClue == false)
                        {
                            e.assignedInvestigator = null;//Disrupted
                        }
                    }
                    e.rumourCounter += 1;
                    if (e.rumourCounter > 4)
                    {
                        e.rumourCounter = 0;
                        if (!e.reportedToSociety)//Evidence self-submits to the local society, to allow folks without investigators to have a chance
                        {
                            Society socSoc = (Society)soc;
                            socSoc.evidenceSubmitted.Add(e);
                            socSoc.lastEvidenceSubmission = map.turn;
                            e.turnSubmitted = map.turn;
                            e.locationFound = this;

                            double deltaFear = World.staticMap.param.threat_evidencePresented;
                            if (socSoc.isDarkEmpire == false)
                            {
                                socSoc.dread_agents_evidenceFound += deltaFear;
                            }
                        }
                        e.reportedToSociety = true;
                        if (e.assignedInvestigator == null && e.pointsTo != null && e.pointsTo.person != null)
                        {
                            double minDist = 0;
                            Unit bestU = null;
                            foreach (Unit u in map.units)
                            {
                                if (
                                    u is Unit_Investigator &&
                                    u.person != null &&
                                    u.person.state != Person.personState.enthralledAgent &&
                                    u.person.getRelation(e.pointsTo.person).suspicion > 0 &&
                                    u.task is Task_Wander &&
                                    u.location.evidence.Count == 0)
                                {
                                    double dist = map.getDist(u.location, this);
                                    dist *= Eleven.random.NextDouble();
                                    if (dist < minDist || bestU == null)
                                    {
                                        bestU = u;
                                        minDist = dist;
                                    }
                                }
                            }
                            if (bestU != null)
                            {
                                e.assignedInvestigator = bestU;
                                bestU.task = new Task_GoToClue(this);
                                map.world.prefabStore.popMsgAgent(bestU,e.pointsTo,bestU.getName() + " has learnt of evidence in " + this.getName() + " and is travelling to investigate." +
                                    " They recognised the methods of " + e.pointsTo.getName() + ", whom they were already suspicious of.");
                            }
                        }
                    }
            }
            }

            bool needDeletion = false;
            foreach (Unit u in units)
            {
                if (u.location != this)
                {
                    throw new Exception("Unit at wrong location: " + u.getName() + " at " + this.getName());
                }
                if (map.units.Contains(u) == false)
                {
                    needDeletion = true;
                    //throw new Exception("Badly handled unit");
                }
            }

            //A horrific hack, because units were, for whatever reason, getting left behind in locations
            //They called "disband", removed themselves from the main unit list, and even checked to ensure they were gone. For whatever reason, either they were not or they were re-added later
            //Bad times, can't fix, sad times
            //
            //Might have been fixed a while ago, I realise. Feel free to try removing this and seeing what it does. Or changing it to an assert
            if (needDeletion)
            {
                List<Unit> rems = new List<Unit>();
                foreach (Unit u in units)
                {
                    if (map.units.Contains(u) == false)
                    {
                        rems.Add(u);
                    }
                }
                foreach (Unit u in rems)
                {
                    units.Remove(u);
                }
            }

            recomputeLinkDisabling();
        }

        public void checkPropertiesEndOfTurn()
        {
            List<Property> rems = new List<Property>();
            foreach (Property p in properties)
            {
                if (p.proto.decaysOverTime && p.charge > 0)
                {
                    p.charge -= 1;
                }
                if (p.charge <= 0)
                {
                    rems.Add(p);
                }
            }
            foreach (Property p in rems)
            {
                properties.Remove(p);
                p.proto.endProperty(this,p);
            }
        }
        public void addPropertyInternally(Property p)
        {
            this.properties.Add(p);
        }

        public Sprite getSprite()
        {
            if (settlement != null) { return settlement.getSprite(); }
            //if (isMajor) { return map.world.textureStore.loc_green; }
            
            if (hex.terrain == Hex.terrainType.SNOW || hex.terrain == Hex.terrainType.TUNDRA)
            {
                return map.world.textureStore.loc_minor_emptySnow;
            }
            else if (hex.terrain == Hex.terrainType.DESERT || hex.terrain == Hex.terrainType.DRY)
            {
                return map.world.textureStore.loc_minor_emptyDesert;
            }
            else if (hex.terrain == Hex.terrainType.SEA)
            {
                return map.world.textureStore.loc_minor_emptyOcean;
            }
            else if (hex.terrain == Hex.terrainType.GRASS || hex.terrain == Hex.terrainType.PATH || hex.terrain == Hex.terrainType.MUD)
            {
                return map.world.textureStore.loc_minor_emptyGrass;
            }
            else if (hex.terrain == Hex.terrainType.SWAMP || hex.terrain == Hex.terrainType.WETLAND)
            {
                return map.world.textureStore.loc_minor_emptyGrass;
            }
            return map.world.textureStore.loc_minor_green;
           // return map.world.textureStore.loc_minor_emptyGrass;
        }
    }
}
