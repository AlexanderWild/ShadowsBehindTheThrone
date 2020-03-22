using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Insanity_Envious : Insanity
    {

        public Insanity_Envious()
        {
            name = "Envious";
            desc = "This character will grow to hate all those who have greater prestige than they themselves possess.";
        }

        public override void process(Person p)
        {
            base.process(p);
            
            foreach (Person p2 in p.society.people){
                if (p2 == p) { continue; }
                RelObj rel = p.getRelation(p2);
                if (p2.prestige > p.prestige)
                {
                    if (rel.getLiking() > -100)
                    {
                        rel.addLiking(-100, "Grew envious due to higher prestige", p.map.turn);
                    }
                }
            }
            lashOut(p);
        }
    }
}
