using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Insanity
    {
        public string name;
        public string desc;
        public double selfLove = 100;

        public virtual void process(Person p)
        {

        }

        public void lashOut(Person p)
        {
            if (Eleven.random.NextDouble() > p.map.param.insanity_lashOutProbability) { return; }

            int c = 0;
            Person victim = null;
            foreach (Person p2 in p.society.people)
            {
                if (p2 == p) { continue; }
                c += 1;
                if (Eleven.random.Next(c) == 0)
                {
                    victim = p2;
                }
            }
            if (victim != null)
            {
                victim.getRelation(p).addLiking(-10, "Lashed out in madess", p.map.turn);
                if (p.society.hasEnthralled())
                {
                    p.map.addMessage(p.getFullName() + " lashes out against " + victim.getFullName() + " in madness", MsgEvent.LEVEL_DARK_GREEN, p.state != Person.personState.enthralled);
                }
            }
        }
    }
}
