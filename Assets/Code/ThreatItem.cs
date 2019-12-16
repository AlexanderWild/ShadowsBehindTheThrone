using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class ThreatItem : IComparable<ThreatItem>
    {
        public SocialGroup group;
        public Map map;
        public Person p;
        public double zeitgeist = 1;
        public double temporaryDread;
        public double threat;
        public List<ReasonMsg> reasons = new List<ReasonMsg>();

        public enum formTypes { NONE, HOSTILE_NATION,ENSHADOWED_NOBLES}
        public formTypes form = formTypes.HOSTILE_NATION;

        public int responseCode = RESPONSE_MILITARY;
        public static int RESPONSE_DARKNESSWITHIN = 0;
        public static int RESPONSE_MILITARY = 1;
        public static string[] responseNames = new string[] { "Introspection","Military"};

        public static float param_at_war = 5;
        public static float param_strength = 2;
        public static float param_territory = 2;
        public static float param_zeitgeistDecay = 0.95f;
        public static float param_accumulatorDecay = 0.97f;
        public static float param_stance_offensive = 1.75f;
        public static float param_stance_defensive = 1.25f;

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
        }

        public List<string> getReasons()
        {
            List<string> reply = new List<string>();
            foreach (ReasonMsg reason in reasons)
            {
                reply.Add(reason.msg + " " + (int)(reason.value));
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
