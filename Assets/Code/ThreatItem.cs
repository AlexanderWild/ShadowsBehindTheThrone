﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class ThreatItem : IComparable<ThreatItem>
    {
        public SocialGroup group;
        public double groupRandID;
        public Map map;
        public Person p;
        public double temporaryDread;
        public double threat;
        public double generatedThreat;
        public List<ReasonMsg> reasons = new List<ReasonMsg>();

        public enum formTypes { NONE, HOSTILE_NATION,ENSHADOWED_NOBLES, AGENTS,PLAGUE }
        public formTypes form = formTypes.HOSTILE_NATION;
        
        public double threatBeforeTemporaryDread;

        public ThreatItem(Map map,Person parent)
        {
            this.map = map;
            this.p = parent;
        }

        public bool isSame(ThreatItem other)
        {
            if (group != null)
            {
                return group == other.group;
            }
            return form == other.form;
        }
        public void setTo(ThreatItem other)
        {
            group = other.group;
            form = other.form;
        }

        public void turnTick()
        {
            generatedThreat -= map.param.threat_agentFearDecayPerTurn;
            if (generatedThreat < 0) { generatedThreat = 0; }
        }

        public List<string> getReasons()
        {
            List<string> reply = new List<string>();
            foreach (ReasonMsg reason in reasons)
            {
                //reply.Add(reason.msg + " " + (int)(reason.value));
                reply.Add(reason.msg);
            }
            return reply;
        }

        public string getGreatestReason()
        {
            string g = null;
            double v = 0.0;

            foreach (ReasonMsg r in reasons)
            {
                if (r.value > v)
                {
                    g = r.msg;
                    v = r.value;
                }
            }

            return g + " (" + (int)v + ")";
        }

        public string getTitle()
        {
            if (group == null)
            {
                if (form == formTypes.HOSTILE_NATION)
                {
                    return "Hostile Nation";
                }
                if (form == formTypes.ENSHADOWED_NOBLES)
                {
                    return "Enshadowed Nobles";
                }
                if (form == formTypes.AGENTS)
                {
                    return "Dark Agents";
                }
                if (form == formTypes.PLAGUE)
                {
                    return "Plague";
                }
                return "UNKNOWN";
            }else
            {
                return group.getName();
            }
        }

        public int CompareTo(ThreatItem obj)
        {
            if (obj.threat > threat)
            {
                return 1;
            }
            else if (obj.threat < threat)
            {
                return -1;
            }
            return 0;
        }
    }
}
