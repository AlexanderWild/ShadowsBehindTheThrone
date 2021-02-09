using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class God_Fog : God
    {
        public List<Ability> powers = new List<Ability>();

        public God_Fog()
        {
            powers.Add(new Ab_Fog_WellOfFog());
            powers.Add(new Ab_Fog_TrapFog());
        }


        public override string getDescFlavour()
        {
            return "Every prey creatures knows its fate from the moment it is born. Its life exists as food for another, and ten thousand eyes watch its every movement, a thousand jaws salivating expectantly." +
                " Its life will be a constant battle to postpone what it knows will eventually come. One day, it will be too tired, too slow or too weak. That day may come at any moment, even in its prime."
                + "\n\nRegardless of the prey's actions, there is the inevitable fate,the guarantee, the promise. The promise of teeth."
                + "\n\nAgainst a dark power such as yourself, the race of humans know themselves to be prey.";
        }

        public override string getDescMechanics()
        {
            return "This Dark Name is designed to provide a more straightforward gameplay in the political sphere. The powers are focused on direct manipulation of social interactions, allowing to you spend power to ascend in society and bend it to your will."
                + "\n";
        }

        public override string getName()
        {
            return "The Drowing of the Sun";
        }

        public override Sprite getGodBackground(World world)
        {
            return world.textureStore.painting_processionInTheFog;
        }
        public override string getCredits()
        {
            return "Procession in the Fog, Ernst Ferdinand Oehme, 1828";
        }

        public override List<Ability> getUniquePowers()
        {
            return powers;
        }
    }
}
