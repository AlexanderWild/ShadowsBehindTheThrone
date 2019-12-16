using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class TitleLanded
    {
        public Settlement settlement;
        public Person heldBy;
        public string titleM;
        public string titleF;

        public TitleLanded(string titleM, string titleF,Settlement set)
        {
            this.titleM = titleM;
            this.titleF = titleF;
            this.settlement = set;
        }

        public string getName()
        {
            return "Hold of " + settlement.name;
        }
    }
}
