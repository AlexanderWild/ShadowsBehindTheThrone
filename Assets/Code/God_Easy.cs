using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class God_Easy : God
    {
        public List<Ability> powers = new List<Ability>();

        public God_Easy()
        {
            powers.Add(new Ab_Easy_AddLiking());
            powers.Add(new Ab_Easy_ShortMemories());
        }


        public override string getDescFlavour()
        {
            return "Flavour here";
        }

        public override string getDescMechanics()
        {
            return "God designed to be a simpler playstyle.";
        }

        public override string getName()
        {
            return "TMP Easy";
        }

        public override List<Ability> getUniquePowers()
        {
            return powers;
        }
    }
}
