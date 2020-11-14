using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class RelObj
    {
        public const int STACK_NONE = 0;
        public const int STACK_REPLACE = 1;
        public const int STACK_ADD = 2;

        //Leave this crashing thing so it doesn't get reimplemented
        //public double liking { get { return liking; } set { liking = value;liking = Math.Min(100, liking);liking = Math.Max(-100, liking); } } THIS IMPLEMENTATION CRASHES THE UNITY EDITOR
        //private double liking;
        public double suspicion;
        public LinkedList<RelEvent> events = new LinkedList<RelEvent>();
        public bool isSelf = false;
        public Person me;
        public int them;

        public RelObj(Person me,int them)
        {
            isSelf = me.index == them;
            this.me = me;
            this.them = them;
        }

        public double getLiking()
        {
            double liking = me.getRelBaseline(them);
            liking += getLikingModifiers(me,them,null);
            foreach (RelEvent r in events)
            {
                liking += r.amount;
            }

            liking += getDislikingFromSuspicion();

            if (liking > 100) { liking = 100; }
            if (liking < -100) { liking = -100; }
            return liking;
        }

        public static double getLikingModifiers(Person me,int themIndex,List<ReasonMsg> reasons)
        {
            Person them = World.staticMap.persons[themIndex];
            double u = 0;

            if (me.society.socType.usesHouses() && (me.house == them.house))
            {
                u += me.map.param.society_houseLiking;
                if (reasons != null)
                {
                    reasons.Add(new ReasonMsg("Same House", me.map.param.society_houseLiking));
                }
            }
            if (them.state == Person.personState.enthralled && me.state == Person.personState.broken)
            {
                u += me.map.param.society_houseLiking;
                if (reasons != null)
                {
                    reasons.Add(new ReasonMsg("Cultist Towards Enthralled", me.map.param.society_houseLiking));
                }
            }

            if (them.madness is Insanity_Sane == false)
            {
                u += me.map.param.insanity_relHit;
                if (reasons != null)
                {
                    reasons.Add(new ReasonMsg("Obviously insane", me.map.param.insanity_relHit));
                }
            }
            foreach (Trait t in them.traits)
            {
                if (t.receivedLikingDelta() != 0)
                {
                    u += t.receivedLikingDelta();
                    if (reasons != null)
                    {
                        reasons.Add(new ReasonMsg(them.getFullName() + " is " + t.name, t.receivedLikingDelta()));
                    }
                }
            }
            foreach (Trait t in me.traits)
            {
                if (t.likingChange() != 0)
                {
                    u += t.likingChange();
                    if (reasons != null)
                    {
                        reasons.Add(new ReasonMsg(me.getFullName() + " is " + t.name, t.likingChange()));
                    }
                }
            }

            return u;
        }

        public double getDislikingFromSuspicion()
        {
            double evMult = (1 - me.shadow);//You care less about shadow the more enshadowed you are
            evMult = Math.Min(evMult, 1);
            evMult = Math.Max(0, evMult);
            return suspicion * evMult * me.map.param.person_dislikingFromSuspicion;
        }
        /*
        public void setLiking(double v)
        {
            liking += v;
            if (liking > 100) { liking = 100; }
            if (liking < -100) { liking = -100; }
        }
        */

        public void turnTick(Person me)
        {
            //if (them == me) { liking = 100; }//Be at least loyal to yourself (till traits override this)

            double baseline = me.getRelBaseline(them);


            List<RelEvent> rems = new List<RelEvent>();
            foreach (RelEvent ev in events)
            {
                ev.amount *= me.map.param.relObj_decayRate;
                if (Math.Abs(ev.amount) < 2) { rems.Add(ev); }
            }
            foreach (RelEvent ev in rems)
            {
                events.Remove(ev);
            }
        }

        public void addLiking(double v,string reason,int turn,int stackStyle=STACK_NONE)
        {
            if (isSelf) { return; }//No events for self

            
            if (stackStyle == STACK_NONE)
            {
                RelEvent ev = new RelEvent();
                ev.amount = v;
                ev.reason = reason;
                ev.turn = turn;
                events.AddLast(ev);
            }
            else
            {
                RelEvent prev = null;
                foreach (RelEvent r2 in events)
                {
                    if (r2.reason == reason)
                    {
                        prev = r2;
                        break;
                    }
                }
                
                if (prev == null)
                {
                    //Didn't matter, wasn't there anyway. Make new
                    RelEvent ev = new RelEvent();
                    ev.amount = v;
                    ev.reason = reason;
                    ev.turn = turn;
                    events.AddLast(ev);
                }
                else
                {
                    if (stackStyle == STACK_REPLACE)
                    {
                        prev.turn = turn;
                        prev.amount = v;
                    }else if (stackStyle == STACK_ADD)
                    {
                        prev.turn = turn;
                        prev.amount += v;
                    }
                }
            }

            //liking += v;
        }
    }
}
