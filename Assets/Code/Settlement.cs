using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Settlement
    {
        public Location location;
        public string name = "defaultSettlementName";
        public TitleLanded title;
        public double basePrestige = 10;
        public double militaryCapAdd = 0;
        public double militaryRegenAdd = 0;
        //public double defensiveStrength = 0;
        public double defensiveStrengthRegen = 0.33;
        public double defensiveStrengthCurrent = 0;
        public double defensiveStrengthMax;
        public double infiltration = 0;
        public int security;
        public bool isHuman = false;//Mainly used to determine whether it can be maintained by a human society
        public int lastAssigned = -1;
        public Unit embeddedUnit = null;
        public Unit attachedUnit = null;
        public int spawnCounter = 0;

        public Settlement(Location loc)
        {
            this.location = loc;
        }

        public double getMilitaryCap()
        {
            double cap = militaryCapAdd;
            foreach (Property p in location.properties)
            {
                cap += p.proto.milCapAdd;
            }
            bool wasNonZero = cap > 0;
            if (title != null && title.heldBy != null)
            {
                foreach (Trait trait in title.heldBy.traits)
                {
                    cap += trait.milCapChange();
                }
            }
            if (wasNonZero)
            {
                if (cap < 1) { cap = 1; }
            }
            else
            {
                if (cap < 0) { cap = 0; }
            }
            return cap;
        }
        public double getEconEffectMult()
        {
            double mult = 1;
            if (location.soc != null && location.soc is Society)
            {
                foreach (EconEffect effect in ((Society)location.soc).econEffects)
                {
                    if (econTraits().Contains(effect.to))
                    {
                        mult *= location.map.param.econ_multFromBuff;
                    }
                    if (econTraits().Contains(effect.from))
                    {
                        mult /= location.map.param.econ_multFromBuff;
                    }
                }
            }
            return mult;
        }
        public virtual double getPrestige()
        {
            double mult = getEconEffectMult();
            return basePrestige*mult;
        }

        public List<EconTrait> econTraits()
        {
            return location.hex.province.econTraits;
        }

        public virtual string getFlavour() { return ""; }

        public virtual double getDefensiveMax()
        {
            double d = defensiveStrengthMax;

            if (title != null && title.heldBy != null)
            {
                foreach (Trait trait in title.heldBy.traits)
                {
                    d += trait.defChange();
                }
            }
            if (d < 0) { d = 0; }
            return d;
        }
        public virtual void turnTick()
        {
            checkTitles();

            defensiveStrengthCurrent += defensiveStrengthRegen;
            if (defensiveStrengthCurrent > getDefensiveMax()) { defensiveStrengthCurrent = getDefensiveMax(); }

            if (title != null && title.heldBy != null)
            {
                location.hex.purity = (float)(1 - title.heldBy.shadow);
            }

            //Deploy forces if you're at war
            if (embeddedUnit != null && location.soc != null)
            {
                if (location.soc.isAtWar())
                {
                    location.units.Add(embeddedUnit);
                    location.map.units.Add(embeddedUnit);
                    embeddedUnit = null;
                }
            }

            //Check if our unit is dead
            if (attachedUnit != null && embeddedUnit != attachedUnit && location.map.units.Contains(attachedUnit) == false)
            {
                attachedUnit = null;
            }
            //Check if we want to spawn a unit
            if (attachedUnit == null)
            {
                if (embeddedUnit != null) { attachedUnit = embeddedUnit; }//Unsure why this would occur, but best to catch
                else
                {
                    checkUnitSpawning();
                }
            }
            if (attachedUnit != null)
            {
                attachedUnit.maxHp = (int)(this.getMilitaryCap());
            }
            //Reset the values downwards if they're too high
            if (embeddedUnit != null)
            {
                embeddedUnit.hp = embeddedUnit.maxHp;
            }
        }

        public virtual void checkUnitSpawning()
        {
            if (isHuman && location.soc != null && location.soc is Society)
            {
                spawnCounter += 1;
                if (spawnCounter > 5)
                {
                    spawnCounter = 0;

                    if (this.attachedUnit != null) { throw new Exception(); }

                    Unit_Army army = new Unit_Army(location, location.soc);
                    location.map.units.Add(army);
                    army.maxHp = (int)this.getMilitaryCap();
                    this.attachedUnit = army;
                }
            }
        }

        public void checkTitles() { 
            if (title != null && title.heldBy != null){
                if (title.heldBy.society != this.location.soc)
                {
                    World.log("Settlement " + this.name + " recognises loss of title of " + title.heldBy.getFullName());
                    title.heldBy = null;
                }
                else if (title.heldBy.society.people.Contains(title.heldBy) == false)
                {
                    World.log("Settlement " + this.name + " recognises loss of title of " + title.heldBy.getFullName());
                    title.heldBy = null;
                }
            }
        }


        public virtual int getSecurity(List<ReasonMsg> reasons)
        {
            int security = 0;
            if (this.isHuman && location.soc != null && location.soc is Society)
            {
                Society soc = (Society)location.soc;
                if (this.title != null && title.heldBy != null)
                {
                    if (title.heldBy.society.getSovreign() == title.heldBy)
                    {
                        if (soc.people.Count >= location.map.param.society_nPeopleForEmpire)
                        {
                            security += 7;
                            reasons.Add(new ReasonMsg("Major Sovreign", 7));
                        }
                        else if (soc.people.Count >= location.map.param.society_nPeopleForKingdom)
                        {
                            security += 4;
                            reasons.Add(new ReasonMsg("Sovreign", 4));
                        }
                        else
                        {
                            security += 2;
                            reasons.Add(new ReasonMsg("Minor Sovreign", 2));
                        }
                    }
                    else
                    {
                        bool isDuke = false;
                        foreach (Title t in title.heldBy.titles)
                        {
                            if (t is Title_ProvinceRuler)
                            {
                                if (soc.lastTurnLocs.Count >= location.map.param.society_nPeopleForEmpire)
                                {
                                    security += 4;
                                    reasons.Add(new ReasonMsg("High Ranking Noble", 4));
                                }
                                else if (soc.lastTurnLocs.Count >= location.map.param.society_nPeopleForKingdom)
                                {
                                    security += 3;
                                    reasons.Add(new ReasonMsg("High Ranking Noble", 3));
                                }
                                else
                                {
                                    security += 2;
                                    reasons.Add(new ReasonMsg("High Ranking Noble", 2));
                                }
                                isDuke = true;
                                break;
                            }
                        }
                        if (!isDuke)
                        {
                            security += 1;
                            reasons.Add(new ReasonMsg("Low Rank Noble", 1));
                        }
                    }
                }
                if (soc.posture == Society.militaryPosture.defensive)
                {
                    security += 1;
                    reasons.Add(new ReasonMsg("Defensive Society", 1));
                }
                if (soc.posture == Society.militaryPosture.introverted)
                {
                    security += 2;
                    reasons.Add(new ReasonMsg("Introverted Society", 2));
                }
                if (soc.posture == Society.militaryPosture.offensive)
                {
                    reasons.Add(new ReasonMsg("Offensive Society", 0));
                }
                if (embeddedUnit != null)
                {
                    security += 1;
                    reasons.Add(new ReasonMsg("Army in garrison", 1));
                }
            }
            
            return security;
        }

        public virtual Sprite getSprite()
        {
            return location.map.world.textureStore.loc_green;
        }

        public virtual void fallIntoRuin()
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
            Set_Ruins ruins = new Set_Ruins(location);
            location.settlement = ruins;
            location.settlement.name = "Ruins of " + location.shortName;
        }

        internal virtual void takeAssault(SocialGroup sg, SocialGroup defender, double theirLosses)
        {
        }
    }
}
