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
            powers.Add(new Ab_Fog_Convocation());
        }


        public override string getDescFlavour()
        {
            return "\"The fog spills out across the nations, flowing from the cities, somehow our sins made manifest. It blots out the sun. The light which remains is pale, and sickly. Whatever warmth sunlight once had, it no longer holds now."
                + "\nWorse, shape moves in the fog. They hide in its white cloak, and act with co-ordination and purpose. The fog makes it nearly impossible to find them, and prevent whatever vile conspiracy they are engaged in.\"";
        }

        public override string getDescMechanics()
        {
            return "This Name allows you to generate a sea of fog, emerging from the nobles under your shadow. Any enthralled, broken or enshadowed noble can be used to add to the sea. Within the fog, your agents and nobles operate better."
                + "\nSpecifically: security is reduced, and your agents do not take damage from being exiled in locations inside the fog. Nobles will take longer to become suspicious of other nobles with evidence, if these are inside the fog.";
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
