using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_LoadCargo : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Loading Cargo";
        }
        public override string getLong()
        {
            return "This agent is loading cargo from .";
        }

        public override void turnTick(Unit unit)
        {
            if (unit is Unit_Merchant == false) { unit.task = null;return; }
            if (unit.society != unit.location.soc) { unit.task = null; return; }
            if (unit.location.settlement == null) { unit.task = null; return; }


            Unit_Merchant merchant = (Unit_Merchant)unit;
            if (unit.location.settlement is Set_City)
            {
                merchant.cargo += 30;
            }
            else
            {
                merchant.cargo += 15;
            }
            if (merchant.cargo >= 100)
            {
                merchant.cargo = 100;
                merchant.task = null;
                if (merchant.isEnthralled())
                {
                    merchant.location.map.world.prefabStore.popMsg(unit.getName() + " has finished loading cargo");
                }
            }
        }
    }
}