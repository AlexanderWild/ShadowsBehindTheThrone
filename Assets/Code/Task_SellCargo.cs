using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_SellCargo : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Selling Cargo";
        }
        public override string getLong()
        {
            return "This agent is selling cargo to gain wealth.";
        }

        public float getSaleValue(SocialGroup seller,Location loc)
        {
            if (loc.soc is Society == false) { return 0; }
            if (loc.settlement == null) { return 0; }
            float sellMult = 1;
            if (seller == loc.soc) { sellMult = 0.5f; }
            if (loc.settlement is Set_City)
            {
                Set_City city = (Set_City)loc.settlement;
                if (city.getLevel() >= Set_City.LEVEL_METROPOLE)
                {
                    return 1.25f* sellMult;
                }
                if (city.getLevel() >= Set_City.LEVEL_CITY)
                {
                    return 1 * sellMult;
                }
                if (city.getLevel() >= Set_City.LEVEL_TOWN)
                {
                    return 0.85f * sellMult;
                }
            }
            return 0.5f * sellMult;
        }

        public override void turnTick(Unit unit)
        {
            if (unit is Unit_Merchant == false) { unit.task = null;return; }
            if (unit.society is Society == false) { unit.task = null; return; }
            if (unit.location.settlement == null) { unit.task = null; return; }


            float profit = getSaleValue(unit.society, unit.location);
            Unit_Merchant merchant = (Unit_Merchant)unit;

            int sold = Math.Min(merchant.cargo, 10);
            merchant.cargo -= sold;
            if (merchant.cargo <= 0) {
                merchant.cargo = 0;
            }
            int gain = (int)(0.5f + (profit * sold));

            merchant.cash += gain;
            

            if (merchant.cash >= 100)
            {
                merchant.task = null;
                if (merchant.isEnthralled())
                {
                    merchant.location.map.world.prefabStore.popMsg(unit.getName() + " has generated as much wealth as they can transport, their strong-boxes brim with gold. They must spend before they can sell again.");
                }
            }
            if (merchant.cargo <= 0)
            {
                merchant.task = null;
                if (merchant.isEnthralled())
                {
                    merchant.location.map.world.prefabStore.popMsg(unit.getName() + " has sold all their cargo. They may now spend their profits or gather further cargo from home.");
                }
            }
        }
    }
}