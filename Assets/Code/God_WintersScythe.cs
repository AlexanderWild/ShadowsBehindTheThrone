using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class God_WintersScythe : God
    {
        public List<Ability> powers = new List<Ability>();

        public God_WintersScythe()
        {
            powers.Add(new Ab_Ice_DeathOfTheSun());
            powers.Add(new Ab_Ice_ColdAsDeath());
        }


        public override string getDescFlavour()
        {
            return "Flavour here";
        }

        public override string getDescMechanics()
        {
            return "This Dark Name revolves around the provocation of constant war to destroy the world's climate. Each battle between human forces can be exploited to slightly reduce the world's temperature," +
                " either in a region or worldwide. Slowly the farmlands in the north will become uninhabitable, and the people forced south, into the ever-dwindling habitable regions, until those too are overrun with the snows."
                + "\nYour strategy should be to cause wars regardless of purpose. Inducing civil wars in kingdoms across the map gives death to use, declaring war of conquest on your neighbours can lead to the entire world suffering, " +
                "and even uprisings against you will fuel your victory.";
        }

        public override string getName()
        {
            return "Winter's Scythe";
        }

        public override List<Ability> getUniquePowers()
        {
            return powers;
        }
        public override Sprite getGodBackground(World world)
        {
            return world.textureStore.painting_monasteryRuins;
        }
        public override string getCredits()
        {
            return "Ruins of the Oybin Monastery in Winter, by Karl Heinrich Beichling, 1830";
        }
    }
}
