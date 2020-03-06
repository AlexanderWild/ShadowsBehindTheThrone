using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_City : Settlement
    {
        public int infrastructure;
        public int population;

        public static int LEVEL_METROPOLE = 5;
        public static int LEVEL_CITY = 4;
        public static int LEVEL_TOWN = 3;
        public static int LEVEL_VILLAGE = 2;
        public static int LEVEL_HAMLET = 1;

        public Set_City(Location loc) : base(loc)
        {
            title = new TitleLanded("Count", "Countess", this);
            name = "City of " + loc.shortName;
            basePrestige = 25;
            militaryCapAdd += 10;
            defensiveStrengthMax = 5;
            militaryRegenAdd = 0.15;
            isHuman = true;
        }

        public int getLevel()
        {
            if (infrastructure >= World.staticMap.param.city_level_metropole)
            {
                return LEVEL_METROPOLE;
            }
            else if (infrastructure >= World.staticMap.param.city_level_city)
            {
                return LEVEL_CITY;
            }
            else if (infrastructure >= World.staticMap.param.city_level_town)
            {
                return LEVEL_TOWN;
            }
            else if (infrastructure >= World.staticMap.param.city_level_village)
            {
                return LEVEL_VILLAGE;
            }
            else
            {
                return LEVEL_HAMLET;
            }
        }

        public override void turnTick()
        {
            base.turnTick();
            statsTurnTick();
            assignOutcomeValues();
        }

        private void assignOutcomeValues()
        {
            int level = getLevel();
            if (level == LEVEL_METROPOLE)
            {
                name = "Metropole of " + location.shortName;
                title.titleF = "Countess";
                title.titleM = "Count";
                basePrestige = 30;
                defensiveStrengthMax = 20;
                militaryRegenAdd = 0.2;
                militaryCapAdd = 20;
            }
            else if (level == LEVEL_CITY)
            {
                name = "City of " + location.shortName;
                title.titleF = "Countess";
                title.titleM = "Count";
                basePrestige = 25;
                defensiveStrengthMax = 10;
                militaryRegenAdd = 0.15;
                militaryCapAdd = 15;
            }
            else if (level == LEVEL_TOWN)
            {
                name = "Town of " + location.shortName;
                title.titleF = "Mayor";
                title.titleM = "Mayor";
                basePrestige = 15;
                defensiveStrengthMax = 5;
                militaryRegenAdd = 0.1;
                militaryCapAdd = 10;
            }
            else if (level == LEVEL_VILLAGE)
            {
                name = location.shortName + " Village";
                title.titleF = "Mayor";
                title.titleM = "Mayor";
                basePrestige = 10;
                militaryRegenAdd = 0.1;
                militaryCapAdd = 5;
            }
            else
            {
                name = location.shortName + " Hamlet";
                title.titleF = "Mayor";
                title.titleM = "Mayor";
                basePrestige = 5;
                militaryRegenAdd = 0.1;
                militaryCapAdd = 3;
            }
        }

        public int getMaxPopulation()
        {
            int maxPop = (int)Math.Ceiling(0.1 + ((location.hex.getHabilitability()-location.map.param.mapGen_minHabitabilityForHumans) * location.map.param.city_popMaxPerHabilitability));
            if (location.isCoastal)
            {
                maxPop += 10;
            }
            return maxPop;
        }
        public void statsTurnTick()
        {
            if (population < getMaxPopulation())
            {
                population += 1;
            }

            if (infrastructure < population)
            {
                infrastructure += 1;
            }
        }

        public double production()
        {
            return Math.Min(population, infrastructure);
        }

        public override void fallIntoRuin()
        {
            //We're abandonning this location due to inhospitability
            if (title.heldBy != null && title.heldBy.title_land == title)
            {
                location.map.addMessage(title.heldBy.getFullName() + " is losing their title, as " + this.name + " is being abandoned.",
                    title.heldBy.state == Person.personState.enthralled ? MsgEvent.LEVEL_RED : MsgEvent.LEVEL_ORANGE,
                    title.heldBy.state == Person.personState.enthralled ? false : true);
                title.heldBy.title_land = null;
            }
            location.map.addMessage(this.name + " is no longer able to sustain human life, and is falling into ruin.");
            Set_CityRuins ruins = new Set_CityRuins(location);
            location.settlement = ruins;
            location.settlement.name = "Ruins of " + location.shortName;
            ruins.infrastructure = this.infrastructure;
            ruins.initialInfrastructure = this.infrastructure;
        }

        public override Sprite getSprite()
        {
            if (getLevel() >= LEVEL_METROPOLE)
            {
                return location.map.world.textureStore.loc_city_metropole;
            }
            else if (getLevel() >= LEVEL_CITY)
            {
                return location.map.world.textureStore.loc_city_roman;
            }
            return location.map.world.textureStore.loc_minor_farm;
        }

        public void takeMilitaryHit()
        {
            population -= (int)(Eleven.random.NextDouble() * location.map.param.city_popDmg);
            if (population == 0) { population = 0; }
            infrastructure -= (int)(Eleven.random.NextDouble() * location.map.param.city_infraDmg);
            if (infrastructure == 0) { infrastructure = 0; }
        }

        internal override void takeAssault(SocialGroup sg, SocialGroup defender, double theirLosses)
        {
            int deltaP = Eleven.random.Next(location.map.param.combat_popDamageMax + 1);
            int deltaI = Eleven.random.Next(location.map.param.combat_infraDamageMax + 1);
            World.log("Damage received " + deltaP + " " + deltaI);
            population = Math.Max(1,population-deltaP);
            infrastructure = Math.Max(1, infrastructure - deltaI);

        }

        internal string getStatsDesc()
        {
            string reply = "";
            reply += "Population Level:";
            reply += "\nPopulation Maximum:";
            reply += "\nInfrastructure:";
            //reply += "\nDevelopment Level: " + Eleven.toFixedLen(getLevel(), 9);

            return reply;
        }

        public string getStatsValues()
        {
            string reply = "";
            reply += population;
            reply += "\n" + getMaxPopulation();
            reply += "\n" + infrastructure;
            //reply += "\nDevelopment Level: " + Eleven.toFixedLen(getLevel(), 9);

            return reply;

        }
    }
}
