﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_City : SettlementHuman
    {
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
            if (population >= World.staticMap.param.city_level_metropole)
            {
                return LEVEL_METROPOLE;
            }
            else if (population >= World.staticMap.param.city_level_city)
            {
                return LEVEL_CITY;
            }
            else if (population >= World.staticMap.param.city_level_town)
            {
                return LEVEL_TOWN;
            }
            else if (population >= World.staticMap.param.city_level_village)
            {
                return LEVEL_VILLAGE;
            }
            else
            {
                return LEVEL_HAMLET;
            }
        }

        public override void humanTurnTick()
        {
            assignValuesFromPopulation();
        }

        private void assignValuesFromPopulation()
        {
            int level = getLevel();
            if (level == LEVEL_METROPOLE)
            {
                name = "Metropole of " + location.shortName;
                title.titleF = "Countess";
                title.titleM = "Count";
                basePrestige = 25;
                defensiveStrengthMax = 20;
                militaryRegenAdd = 4;
                militaryCapAdd = 40*location.map.param.unit_armyHPMult;
            }
            else if (level == LEVEL_CITY)
            {
                name = "City of " + location.shortName;
                title.titleF = "Countess";
                title.titleM = "Count";
                basePrestige = 20;
                defensiveStrengthMax = 16;
                militaryRegenAdd = 3;
                militaryCapAdd = 30 * location.map.param.unit_armyHPMult;
            }
            else if (level == LEVEL_TOWN)
            {
                name = "Town of " + location.shortName;
                title.titleF = "Mayor";
                title.titleM = "Mayor";
                basePrestige = 15;
                defensiveStrengthMax = 8;
                militaryRegenAdd = 2;
                militaryCapAdd = 20 * location.map.param.unit_armyHPMult;
            }
            else if (level == LEVEL_VILLAGE)
            {
                name = location.shortName + " Village";
                title.titleF = "Mayor";
                title.titleM = "Mayor";
                basePrestige = 10;
                militaryRegenAdd = 2;
                militaryCapAdd = 15 * location.map.param.unit_armyHPMult;
            }
            else
            {
                name = location.shortName + " Hamlet";
                title.titleF = "Mayor";
                title.titleM = "Mayor";
                basePrestige = 5;
                militaryRegenAdd = 2;
                militaryCapAdd = 10 * location.map.param.unit_armyHPMult;
            }
        }

        //public override int getMaxPopulation()
        //{
        //    double multiplier = location.map.param.city_popMaxPerHabilitability;
        //    if (location.isCoastal)
        //    {
        //        multiplier = location.map.param.city_popMaxPerHabilitabilityCoastal;
        //    }
        //    int maxPop = (int)Math.Ceiling(0.1 + ((location.hex.getHabilitability()-location.map.param.mapGen_minHabitabilityForHumans) * multiplier));
        //    if (maxPop < location.map.param.city_popMin) { maxPop = location.map.param.city_popMin; }
        //    return maxPop;
        //}

        public override string getFlavour()
        {
            if (this.getLevel() >= LEVEL_CITY)
            {
                if (this.location.hex.purity > 0.5)
                {
                    return location.map.world.wordStore.lookup("SET_CITY_PURE");
                }
                else
                {
                    return location.map.world.wordStore.lookup("SET_CITY_DARK");
                }
            }
            return location.map.world.wordStore.lookup("SET_FARM");
        }
        //public override void fallIntoRuin()
        //{
        //    //We're abandonning this location due to inhospitability
        //    if (title.heldBy != null && title.heldBy.title_land == title)
        //    {
        //        location.map.addMessage(title.heldBy.getFullName() + " is losing their title, as " + this.name + " is being abandoned.",
        //            title.heldBy.state == Person.personState.enthralled ? MsgEvent.LEVEL_RED : MsgEvent.LEVEL_ORANGE,
        //            title.heldBy.state == Person.personState.enthralled ? false : true);
        //        title.heldBy.title_land = null;
        //    }
        //    location.map.addMessage(this.name + " is no longer able to sustain human life, and is falling into ruin.");
        //    Set_CityRuins ruins = new Set_CityRuins(location);
        //    location.settlement = ruins;
        //    location.settlement.name = "Ruins of " + location.shortName;
        //    ruins.infrastructure = this.population;
        //    ruins.initialInfrastructure = this.population;
        //}

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

            else if (getLevel() >= LEVEL_TOWN)
            {
                return location.map.world.textureStore.loc_town;
            }
            return location.map.world.textureStore.loc_minor_farm;
        }
    }
}
