using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class RelObj
    {
        //Leave this crashing thing so it doesn't get reimplemented
        //public double liking { get { return liking; } set { liking = value;liking = Math.Min(100, liking);liking = Math.Max(-100, liking); } } THIS IMPLEMENTATION CRASHES THE UNITY EDITOR
        //private double liking;
        public double suspicion;
        public LinkedList<RelEvent> events = new LinkedList<RelEvent>();
        public bool isSelf = false;

        public RelObj(Person me,Person them)
        {
            isSelf = me == them;
        }
        public double getLiking()
        {
            return 0;
        }

        public double getLiking(Person me,Person them)
        {
            double liking = me.getRelBaseline(them);
            foreach (RelEvent r in events)
            {
                liking += r.amount;
            }

            liking += getDislikingFromSuspicion(me);

            if (liking > 100) { liking = 100; }
            if (liking < -100) { liking = -100; }
            return liking;
        }

        public double getDislikingFromSuspicion(Person me)
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

        private List<RelEvent> rems = new List<RelEvent>();
        public void turnTick(Person me,Person them)
        {
            //if (them == me) { liking = 100; }//Be at least loyal to yourself (till traits override this)

            double baseline = me.getRelBaseline(them);

            rems.Clear();
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

        public void addLiking(double v,string reason,int turn)
        {
            if (isSelf) { return; }//No events for self

            RelEvent ev = new RelEvent();
            ev.amount = v;
            ev.reason = reason;
            ev.turn = turn;
            events.AddLast(ev);
            //liking += v;
        }
    }
}
