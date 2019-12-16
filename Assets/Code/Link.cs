using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Link
    {
        public Location a;
        public Location b;

        public Link(Location la, Location lb)
        {
            a = la;
            b = lb;
        }

        public Location other(Location c)
        {
            if (c == a) { return b; }
            if (c == b) { return a; }
            throw new Exception("Not either of this link's locations");
        }
    }
}
