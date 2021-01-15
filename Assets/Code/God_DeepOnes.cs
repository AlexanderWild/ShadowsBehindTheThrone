using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class God_DeepOnes : God
    {
        public List<Ability> powers = new List<Ability>();

        public God_DeepOnes()
        {
            powers.Add(new Ab_Fishman_Lair());
            powers.Add(new Ab_Fishman_CultOfTheDeep());
            powers.Add(new Ab_Fishman_Attack());
            powers.Add(new Ab_Fishman_HauntingSong());
            powers.Add(new Ab_Fishman_AbyssalSirens());
            powers.Add(new Ab_Fishman_Withdraw());
        }


        public override string getDescFlavour()
        {
            return "Beneath the waves the light of the sun never shines. Faint shimmers make it down a few fathoms, but no further. Far far further down, unnatural creatures make their own lights," +
                " flickering and blinking like drowned stars. Amongst these, creatures raise up giant cities of granite and obsidian, and temples to beings far beyond the reckoning of man." +
                " On the surface, these prayers are felt as unearthly calls to join those who dwell beneath the waves. The Deep Ones, worshipping their blasphemous God.";
        }

        public override string getDescMechanics()
        {
            return "This Name revolves around the Deep Ones, a race of beings made by corrupting humans and drawing them beneath the waves. They build up colonies near human settlements, use" +
                " their siren songs to convert humans to their cult, then assault the land when the surface dwellers are weakened by internal conflict. They are powerful against costal nations," +
                " by starting cults which can infiltrate their settlements, driving coastal nobles insane or launching full military attacks on their lands." +
                "\n\nDeep One colonies start hidden, reducing the threat perceived by nobles, but you must still keep an eye on the dread you accumulate and temporary threat, lest you be attacked before you are ready.";
        }

        public override string getName()
        {
            return "The Priest in the Deep";
        }

        public override string getCredits()
        {
            return "Joseph Mallord William Turner, Fishermen At Sea, 1796";
        }

        public override Sprite getGodBackground(World world)
        {
            return world.textureStore.painting_fishermenAtSea;
        }

        public override List<Ability> getUniquePowers()
        {
            return powers;
        }
    }
}
