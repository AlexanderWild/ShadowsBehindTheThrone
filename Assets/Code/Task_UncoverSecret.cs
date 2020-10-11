using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_UncoverSecret : Task
    {
        public int dur;
        public bool leaveEvidence = true;
        public override string getShort()
        {
            return "Uncovering Secret " + dur  + "/" + World.staticMap.param.unit_seeker_uncoverTime;
        }
        public override string getLong()
        {
            return "This agent is uncovering a forgotten secret in this location.";
        }

        public override void turnTick(Unit unit)
        {
            dur += 1;
            if (dur >= unit.location.map.param.unit_seeker_uncoverTime)
            {

                Unit_Seeker seeker = (Unit_Seeker)unit;

                seeker.secrets += 1;

                List<Property> rems = new List<Property>();
                foreach (Property pr in unit.location.properties)
                {
                    if (pr.proto is Pr_ForgottenSecret)
                    {
                        rems.Add(pr);
                    }
                }
                foreach (Property pr in rems)
                {
                    seeker.location.properties.Remove(pr);
                }

                string msg = unit.getName() + " discovers a forgotten secret, they now have " + seeker.secrets + " and need " + seeker.reqSecrets + " to discover the truth.";
                if (seeker.secrets == seeker.reqSecrets)
                {
                    msg = unit.getName() + " learns a forgotten secret, and now knows enough to piece together the truth they have been seeking. Use their ability to do so.";
                }
                if (leaveEvidence)
                {
                    msg += "\nThey left evidence behind.";
                }
                unit.location.map.world.prefabStore.popImgMsg(msg,unit.location.map.world.wordStore.lookup("ABILITY_SEEKER_UNCOVER_SECRET"), img: 2);
                unit.task = null;

                if (leaveEvidence)
                {
                    Evidence e = new Evidence(unit.location.map.turn);
                    e.pointsTo = unit;
                    e.weight = unit.location.map.param.unit_seeker_uncoverEvidence;
                    unit.location.evidence.Add(e);
                }
            }
        }
    }
}