using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Province
    {
        public Hex coreHex;
        public string name;
        public List<int> locations = new List<int>();
        public int capital = -1;
        public List<EconTrait> econTraits = new List<EconTrait>();
        public bool isSea = false;

        public float cr = (float)Eleven.random.NextDouble();
        public float cg = (float)Eleven.random.NextDouble();
        public float cb = (float)Eleven.random.NextDouble();
        public int index;

        public Province(Hex hex)
        {
            coreHex = hex;
        }
    }
}
