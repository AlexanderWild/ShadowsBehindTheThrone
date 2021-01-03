using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_TreatDisease : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Treating Disease";
        }
        public override string getLong()
        {
            return "This agent is treating the infection here, reducing its duration.";
        }

        public override void turnTick(Unit unit)
        {
            Property disease = null;
            foreach (Property pr in unit.location.properties)
            {
                if (pr.proto.isDisease)
                {
                    disease = pr;
                    World.log("On disease");
                    break;
                }
            }
            if (disease == null)
            {
                World.log("No disease present");
                unit.task = null; return;
            }

            disease.charge -= 2;
            if (disease.charge < 0)
            {
                disease.charge = 0;
            }
        }
    }
}