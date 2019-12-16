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
        public bool isHuman = false;//Mainly used to determine whether it can be maintained by a human society


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

        public virtual void turnTick()
        {
            checkTitles();

            defensiveStrengthCurrent += defensiveStrengthRegen;
            if (defensiveStrengthCurrent > defensiveStrengthMax) { defensiveStrengthCurrent = defensiveStrengthMax; }

            if (title != null && title.heldBy != null)
            {
                location.hex.purity = (float)(1 - title.heldBy.shadow);
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

        public virtual Sprite getSprite()
        {
            return location.map.world.textureStore.loc_green;
        }
    }
}
