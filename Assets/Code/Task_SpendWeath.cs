using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_SpendWealth : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Spending Wealth";
        }
        public override string getLong()
        {
            return "This agent is spending their wealth, increasing the prestige of the local noble, as well as gaining liking and replenishing lost military forces.";
        }
        

        public override void turnTick(Unit unit)
        {
            if (unit is Unit_Merchant == false) { unit.task = null;return; }
            if (unit.society is Society == false) { unit.task = null; return; }
            if (unit.location.settlement == null) { unit.task = null; return; }
            
            Unit_Merchant merchant = (Unit_Merchant)unit;
            if (merchant.cash <= 0) { unit.task = null; return; }

            merchant.cash -= 10;

            Person noble = merchant.location.person();
            if (noble != null)
            {
                noble.getRelation(merchant.person).addLiking(10, "Spent Wealth", unit.location.map.turn, RelObj.STACK_ADD);
                noble.prestige += 1.5;
            }
            if (unit.location.soc.currentMilitary < unit.location.soc.maxMilitary)
            {
                unit.location.soc.currentMilitary += 1;
                if (unit.location.soc.currentMilitary > unit.location.soc.maxMilitary)
                {
                    unit.location.soc.currentMilitary = unit.location.soc.maxMilitary;
                }
            }
            
            if (merchant.cash <= 0)
            {
                merchant.cash = 0;
                merchant.task = null;
                if (merchant.isEnthralled())
                {
                    merchant.location.map.world.prefabStore.popMsg(unit.getName() + " has spent their last wealth, and must now return home to collect more cargo to sell.");
                }
            }
        }
    }
}