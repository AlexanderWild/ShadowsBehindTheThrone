using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public abstract class SocType
    {
        public abstract string getName();
        public abstract string getDesc();

        public abstract bool periodicElection();
    }
}
