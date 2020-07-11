using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class SocialGroup
    {
        public Map map;
        public string name;
        public Color color;
        public Color color2;

        public SavableMap_SG_DipRel relations = new SavableMap_SG_DipRel();
        public DipRel selfRel;
        public List<Location> lastTurnLocs = new List<Location>();

        public double threat_mult = 0;
        public double maxMilitary;
        public double currentMilitary;
        public double militaryRegen;
        public int lastBattle;
        public bool cachedGone;
        public double temporaryThreat;
        public double permanentThreat;

        public SocialGroup(Map map)
        {
            this.map = map;
            color = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble());
            color2 = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble());
            name = "SocialGroup";

            //Self-diplomacy
            DipRel rel = new DipRel(map, this, this);
            selfRel = rel;
            relations.Add(this, rel);
        }

        public virtual string getTypeName() { return "Generic Group"; }
        public virtual string getTypeDesc() { return "This group has no associated information."; }
        public virtual bool hostileTo(Unit u) { return false; }
        public bool isAtWar()
        {
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (this.getRel(sg).state == DipRel.dipState.war) { return true; }
            }
            return false;
        }

        /*
         * By default, a social group is gone if it holds no territory. Can be overriden by specials
         */
        public virtual bool checkIsGone() {
            if (cachedGone) { return true; }
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this) {return false; }
            }
            cachedGone = true;
            return true;
        }
        /*
         * By default, a social group is gone if it holds no territory. Can be overriden by specials
         */
        public virtual bool isGone()
        {
            return cachedGone;
        }

        public virtual bool hasEnthralled()
        {
            return false;
        }

        public int getSize()
        {
            int reply = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this) { reply += 1; }
            }
            return reply;
        }

        public DipRel getRel(SocialGroup soc)
        {
            if (relations.ContainsKey(soc)) { return relations.lookup(soc); }
            if (soc.relations.ContainsKey(this))
            {
                relations.Add(soc,soc.relations.lookup(this));
                return relations.lookup(soc);
            }
            DipRel rel = new DipRel(map,this,soc);
            relations.Add(soc, rel);
            soc.relations.Add(this, rel);

            return rel;
        }

        public void setName(string newName)
        {
            name = newName;
        }

        public List<SocialGroup> getNeighbours()
        {
            return map.getNeighbours(this);
        }

        public List<DipRel> getAllRelations()
        {
            List<DipRel> reply = new List<DipRel>();
            foreach (SocialGroup other in map.socialGroups)
            {
                reply.Add(getRel(other));
            }
            return reply;
        }

        public virtual string getName()
        {
            return name;
        }
        public virtual void turnTick()
        {
            temporaryThreat *= map.param.temporaryThreatDecay;
            permanentThreat += temporaryThreat * map.param.temporaryThreatConversion;
            computeMilitaryCap();

            lastTurnLocs.Clear();
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    lastTurnLocs.Add(loc);
                }
            }
        }

        public virtual double getThreat(List<ReasonMsg> reasons)
        {
            ReasonMsg msg;
            double threat = 5;
            if (reasons != null)
            {
                msg = new ReasonMsg("Base", 5);
                reasons.Add(msg);
            }

            threat += (currentMilitary + (maxMilitary/2))*0.2;
            if (reasons != null)
            {
                msg = new ReasonMsg("Current Military", currentMilitary * 0.2);
                reasons.Add(msg);
                msg = new ReasonMsg("Max Military", (maxMilitary/ 2) * 0.2);
                reasons.Add(msg);
            }

            if (this.threat_mult != 0)
            {
                int percent = (int)(100 * this.threat_mult);
                double addT = threat * this.threat_mult;
                threat += addT;
                if (reasons != null)
                {
                    string sign = addT > 0 ? "+" : "";//An advanced programing thingy to make Wyatt think I know how to program
                    msg = new ReasonMsg(sign + percent + "% from type", addT);
                    reasons.Add(msg);
                }

            }

            threat += temporaryThreat;
            if (reasons != null)
            {
                msg = new ReasonMsg("Temporary Threat", temporaryThreat);
                reasons.Add(msg);
            }
            threat += permanentThreat;
            if (reasons != null)
            {
                msg = new ReasonMsg("Permanent Threat", permanentThreat);
                reasons.Add(msg);
            }
            return threat;
        }
        
        public void computeMilitaryCap() {
            maxMilitary = 0;
            militaryRegen = 0;
            currentMilitary = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this && loc.settlement != null)
                {
                    militaryRegen += loc.settlement.militaryRegenAdd;
                    maxMilitary += loc.settlement.getMilitaryCap() ;

                    if (loc.settlement.embeddedUnit != null && loc.settlement.embeddedUnit.isMilitary)
                    {
                        currentMilitary += loc.settlement.embeddedUnit.hp;
                    }
                }
            }
            foreach (Unit u in map.units)
            {
                if (u.isMilitary && u.society == this)
                {
                    currentMilitary += u.hp;
                }
            }
        }

        public void processMilitaryRegen() { 
            currentMilitary += militaryRegen;
            if (currentMilitary > maxMilitary) { currentMilitary = maxMilitary; }
        }

        public virtual void takeLocationFromOther(SocialGroup def, Location taken)
        {
            this.temporaryThreat += map.param.threat_takeLocation;
        }

        public virtual bool isDark()
        {
            return false;
        }
    }
}
