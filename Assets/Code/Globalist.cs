using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Globalist
    {
        public List<Trait> allTraits = new List<Trait>();
        public List<Insanity> allInsanities = new List<Insanity>();
        public List<EconTrait> allEconTraits = new List<EconTrait>();
        public List<Property_Prototype> allProperties = new List<Property_Prototype>();
        public SavableMap_String_PropertyP propertyMap = new SavableMap_String_PropertyP();
        public Insanity madness_sane = new Insanity_Sane();

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

        internal void buildBasicElements()
        {
            allInsanities.Add(new Insanity_Paranoid());
            allInsanities.Add(new Insanity_Envious());
            allInsanities.Add(new Insanity_SelfDestructive());

            allTraits.Add(new Trait_Charismatic());
            allTraits.Add(new Trait_Unlikable());
            allTraits.Add(new Trait_Incautious());
            allTraits.Add(new Trait_Aware());
            allTraits.Add(new Trait_Hateful());
            allTraits.Add(new Trait_Warmaster());
            allTraits.Add(new Trait_BadCommander());
            allTraits.Add(new Trait_Defender());
            allTraits.Add(new Trait_IncompetentFollower());
            allTraits.Add(new Trait_CompetentFollower());
        }

        public Trait getTrait(Person person)
        {
            int c = 0;
            Trait response = null;
            foreach (Trait t in allTraits)
            {
                bool allowed = true;
                foreach (Trait t2 in person.traits)
                {
                    if (t2.groupCode == t.groupCode) { allowed = false;break; }
                }
                if (allowed)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        response = t;
                    }
                }
            }
            return response;
        }
    }
}
