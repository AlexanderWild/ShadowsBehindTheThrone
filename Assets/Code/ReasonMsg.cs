using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class ReasonMsg
    {
        public double value;
        public string msg;

        public ReasonMsg(string v, double u)
        {
            this.msg = v;
            this.value = u;
        }
    }
}
