using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class God_MerchantOfNightmares : God
    {
        public override string getDescFlavour()
        {
            return "Fear is a disease, spreading from mind to mind by word of mouth, by printed propaganda and by whispered rumour. It saps at reason, undermines logic and caution, and drives"
                + " the afflicted to seek any refuge, any escape, any safety, regardless of the consequences. Those striken by terror will fall into whichever traps are laid before them," +
                "too concerned with the foes, real or imagined, they flee from.";
        }

        public override string getDescMechanics()
        {
            return "This Name revolves around creating then spreading fear (as seen by noble's threat items) across the population. The fear can be used to divide the population and topple established power structures, leaving "
                + " a vacuum for you to rise to the top of. Once there, you can begin a campaign of miliary conquest, constantly expelling any who oppose your rampant militarism."
                + "\n\nNote that this Name's powers are better suited for spreading existing fear than creating it from nothing. Other Names, finding prime targets, or creating suspicion of your" +
                " true nature may be useful in starting the process.";
        }

        public override string getName()
        {
            return "Merchant of Nightmares";
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
            map.overmind.powers.AddRange(this.getUniquePowers());
        }
    }
}
