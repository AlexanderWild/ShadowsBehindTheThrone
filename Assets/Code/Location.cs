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
        public Color territoryColor;
        public List<Link> links = new List<Link>();
        public bool isCoastal;
        public bool isOcean;
        public bool isForSocieties = true;
        public Map map;
        public string name;
        public string shortName;
        public bool isMajor;//Remember if it's a city, so we have roughly the same amount at all times, regardless of razing
        public List<Property> properties = new List<Property>();
        public int turnLastTaken = -1000;
        public double inherentInformationAvailability = 0.85;
        public int lastTaken;
        public SavableMap_SG_Double information = new SavableMap_SG_Double();
        public int debugVal;
        //public int turnLastAssigned;
        public Province province;

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

        public double getInformationAvailability()
        {
            double mult = 1;
            foreach (Property p in properties)
            {
                mult *= p.proto.informationAvailabilityMult;
            }
            return inherentInformationAvailability;
        }
        

        public string getName()
        {
            if (settlement != null) { return settlement.name; }
            return name;
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
            if (society.getCapital() != null && society.getCapital().province == this.province) { return society.getSovreign(); }
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
                reply.Add(link.other(this));
            }
            return reply;
        }

        public void turnTick()
        {
            if (settlement != null)
            {
                settlement.turnTick();
            }
            
            foreach (Property p in properties)
            {
                p.proto.turnTick(p,this);
            }
            checkPropertiesEndOfTurn();
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
                p.endProperty(this);
            }
        }
        public void addPropertyInternally(Property p)
        {
            this.properties.Add(p);
        }

        /*
        public void combat(Location other, DipRel rel)
        {
            //Forbid double changing of hands
            if (turnLastTaken == map.turn) { return; }
            if (other.turnLastTaken == map.turn) { return; }

            Property.addProperty(map, this, "ravages_of_war");
            Property.addProperty(map, other, "ravages_of_war");

            float strUs = soc.getMilitaryStrength();
            float strThem = other.soc.getMilitaryStrength();
            bool weAreSafe = this.settlement != null && this.settlement.def > 0;
            bool theyAreSafe = other.settlement != null && other.settlement.def > 0;

            bool weWin = strUs * Eleven.random.NextDouble() > strThem * Eleven.random.NextDouble();
            if (Eleven.random.NextDouble() < map.para.combat_pCoinflipResolution)
            {
                weWin = Eleven.random.NextDouble() > 0.5;
            }

            float ourDmgDone = (float)(strUs * Eleven.random.NextDouble() * map.para.combat_lethality);
            float theirDmgDone = (float)(strThem * Eleven.random.NextDouble() * map.para.combat_lethality);

            if (weWin) { theirDmgDone /= 2; }
            else { ourDmgDone /= 2; }

            if (weAreSafe) { theirDmgDone /= 2; }
            if (theyAreSafe) { ourDmgDone /= 2; }

            World.log("Combat between " + this.hex.getName() + " and " + other.hex.getName());
            inflictMilitaryDamage(other.soc, ourDmgDone, strThem);
            inflictMilitaryDamage(soc, theirDmgDone, strUs);
            if (ourDmgDone == theirDmgDone)
            {

            }
            else if (ourDmgDone > theirDmgDone)
            {
                //We win, damage defender or take
                if (other.settlement == null)
                {
                    other.turnLastTaken = map.turn;
                    if (rel.war != null) { rel.war.turnsSinceProgress = 0; }
                    this.soc.takeLocation(other);
                }
                else
                {
                    other.settlement.def -= 1;
                    if (other.settlement.def <= 1)
                    {
                        if (rel.war != null) { rel.war.turnsSinceProgress = 0; }
                        other.turnLastTaken = map.turn;
                        this.soc.takeLocation(other);
                    }
                }
            }
            else
            {
                //They win, damage defender or take
                if (settlement == null)
                {
                    turnLastTaken = map.turn;
                    other.soc.takeLocation(this);
                    if (rel.war != null) { rel.war.turnsSinceProgress = 0; }
                }
                else
                {
                    this.settlement.def -= 1;
                    if (this.settlement.def <= 1)
                    {
                        if (rel.war != null) { rel.war.turnsSinceProgress = 0; }
                        turnLastTaken = map.turn;
                        other.soc.takeLocation(this);
                    }
                }

            }
        }
        */

        /*
        public void checkOwnershipStatus()
        {
            //Owned by no-longer existant society
            if (soc != null)
            {
                if (map.socialGroups.Contains(soc) == false)
                {
                    if (settlement != null)
                    {
                        World.log("Check isGone status: " + soc.isGone());
                        throw new Exception("Society deleted despite still owning land: " + soc.getName() + " " + settlement.getComplexName());
                    }
                    soc = null;
                }
            }

            //if (soc != null && soc is Society)
            //{
            //    if (parent == null)
            //    {
            //        if (settlement == null) { soc = null; }//Nothing to maintain the parent location
            //    }
            //    else if (parent.settlement == null || (parent.settlement is Set_City == false))//Parent has no city, so can't maintain control over you
            //    {
            //        soc = null;
            //    }
            //    else if (parent.soc != null && (parent.soc is Society == false))
            //    {
            //        soc = null;
            //    }
            //}
            //else if (soc == null && parent != null)//See if you need to be attributed
            //{
            //    if (parent.soc != null && parent.soc is Society && parent.settlement != null && parent.settlement is Set_City)
            //    {
            //        soc = parent.soc;
            //    }
            //}

            //Claim neighbouring locations
            if (soc is Society && settlement != null && settlement is Set_City)
            {
                foreach (Location l2 in getNeighbours())
                {
                    if (l2.isOcean) { continue; }
                    if (l2.soc == null)
                    {
                        l2.soc = soc;
                    }
                }
            }

            if (settlement == null)
            {
                //Lose control of the sea if you're a human society
                if (soc is Society && isOcean)
                {
                    soc = null;
                }
                //Force them out of the uninhabitable areas
                if (soc is Society && map.cityPlacementMap[hex.x, hex.y] < map.param_cityPlacementMapLimit)
                {
                    if (map.turn - turnLastTaken > 10)
                    {
                        soc = null;
                    }
                }
            }

            //Lose control of anything which doesn't have a neighbouring settlement to support it
            if (settlement == null && soc != null)
            {
                bool maintained = false;
                if (map.turn - turnLastTaken < 10) { maintained = true; }
                foreach (Location l2 in getNeighbours())
                {
                    if (l2.settlement != null && l2.soc == this.soc)
                    {
                        maintained = true;
                    }
                }
                if (!maintained)
                {
                    soc = null;
                }
            }
        }
        */

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
            else if (hex.terrain == Hex.terrainType.GRASS || hex.terrain == Hex.terrainType.DRY)
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
