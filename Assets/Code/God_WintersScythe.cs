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
            powers.Add(new Ab_Ice_IceBlood());
        }


        public override string getDescFlavour()
        {
            return "It was always humanity's violence which would doom it. Human history is the history of warfare, waged pointlessly between leaders who sent tens of thousands to die for a cause they would never profit from," +
                "and had no connection to. The peasants die for the games of the nobles."
                + "\n\nEach senseless death kills not just the soldiers, but the world itself, slowly sapping the life out of the world, as the battles rage. The world grows colder with every sword blow struck, as Winter's Scythe reaps the misery, sorrow and fear.";
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
