using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Globalist
    {
        public List<Trait> allTraits = new List<Trait>();
        public List<EconTrait> allEconTraits = new List<EconTrait>();
        public List<Property_Prototype> allProperties = new List<Property_Prototype>();
        public SavableMap_String_PropertyP propertyMap = new SavableMap_String_PropertyP();

        public EconTrait econTrait(string name)
        {
            foreach (EconTrait t in allEconTraits)
            {
                if (t.name == name)
                {
                    return t;
                }
            }
            throw new Exception("Unable to find econ trait: " + name);
        }
    }
}
