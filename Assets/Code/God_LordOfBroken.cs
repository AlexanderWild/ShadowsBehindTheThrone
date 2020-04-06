using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class God_LordOfBroken : God
    {
        public override string getDescFlavour()
        {
            return "Lorum ipsem dolorum";
        }

        public override string getDescMechanics()
        {
            return ".";
        }

        public override string getName()
        {
            return "Lord of the Broken";
        }

        public override List<Ability> getUniquePowers()
        {
            List<Ability> powers = new List<Ability>();
            powers.Add(new Ab_Fash_DenounceLeader());
            powers.Add(new Ab_Fash_InstillDread());
            powers.Add(new Ab_Fash_SpreadFear());
            powers.Add(new Ab_Fash_BreakMind());
            powers.Add(new Ab_Fash_PolariseByFear());
            powers.Add(new Ab_Fash_CallToViolence());
            return powers;
        }

        public override Sprite getGodBackground(World world)
        {
            return world.textureStore.painting_monkAndOldWoman;
        }
        public override string getCredits()
        {
            return "Monk Talking to an Old Woman, Francisco Goya, 1825";
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
