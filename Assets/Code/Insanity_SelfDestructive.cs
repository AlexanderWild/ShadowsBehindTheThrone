using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Insanity_SelfDestructive : Insanity
    {

        public Insanity_SelfDestructive()
        {
            name = "Self-Destructive";
            desc = "This character acts to harm themselves, voting against their own interests.";
            selfLove = -100;
        }

        public override void process(Person p)
        {
            base.process(p);

            lashOut(p);
        }
    }
}
