using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Insanity_Paranoid : Insanity
    {

        public Insanity_Paranoid()
        {
            name = "Paranoid";
            desc = "This character is convinced their fellow nobles are in league with dark powers, and will gain suspicion of others without need for evidence or reason.";
        }

        public override void process(Person p)
        {
            base.process(p);

            int c = 0;
            Person victim = null;
            int nSuspected = 0;
            foreach (Person p2 in p.society.people){
                if (p2 == p) { continue; }
                RelObj rel = p.getRelation(p2);
                if (rel.suspicion > 0.5)
                {
                    nSuspected += 1;
                }
                else
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        victim = p2;
                    }
                }
            }
            if (nSuspected < p.map.param.insanity_nParanoiaTargets && victim != null)
            {
                p.getRelation(victim).suspicion = 1;
            }
            lashOut(p);
        }
    }
}
