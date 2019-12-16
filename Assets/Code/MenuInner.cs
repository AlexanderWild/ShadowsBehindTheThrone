using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public abstract class MenuInner
    {
        public abstract string getMenuTitle();
        public abstract string getMenuBody();
        public abstract void optSelected(int opt);
        public abstract string getBoxTitle(int opt);
        public abstract string getBoxCost(int opt);
        public abstract int getNOpts();
    }
}
