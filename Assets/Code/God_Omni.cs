using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class God_Omni : God
    {
        public override string getDescFlavour()
        {
            return "The Darkness is everywhere, and in every thing. The light is just a temporary bout of fevered hope, to be rapidly extinguished like a candle in a storm."
                + "\nThe darkness has all names, and all faces. Everything which was feared is true.";
        }

        public override string getDescMechanics()
        {
            return "Not necessarily recommended for normal gameplay, this Dark Name allows all powers for all other Names.";
        }

        public override string getName()
        {
            return "Omnipresent Darkness";
        }

        public override List<Ability> getUniquePowers()
        {
            return new List<Ability>();
        }

        public override Sprite getGodBackground(World world)
        {
            return world.textureStore.painting_deathAndGravedigger;
        }
        public override string getCredits()
        {
            return "Death and the Gravedigger, Carlos Schwabe, 1890s";
        }

        public override void onStart(Map map)
        {
            foreach (God g in map.world.potentialGods)
            {
                map.overmind.powers.AddRange(g.getUniquePowers());
            }
        }
    }
}
