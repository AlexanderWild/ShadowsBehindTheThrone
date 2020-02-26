using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class God_Creatures : God
    {
        public List<Ability> powers = new List<Ability>();

        public God_Creatures()
        {
            powers.Add(new Ab_Fishman_Lair());
            powers.Add(new Ab_Fishman_CultOfTheDeep());
            powers.Add(new Ab_Fishman_Attack());
            powers.Add(new Ab_Fishman_HauntingSong());
            powers.Add(new Ab_UnholyFlesh_Seed());
            powers.Add(new Ab_UnholyFlesh_Screetching());
            powers.Add(new Ab_UnholyFlesh_Attack());
            powers.Add(new Ab_UnholyFlesh_Defend());
            powers.Add(new Ab_UnholyFlesh_Grow());
        }


        public override string getDescFlavour()
        {
            return "Flavour here";
        }

        public override string getDescMechanics()
        {
            return "God what has the creatures.";
        }

        public override string getName()
        {
            return "TMP Creatures";
        }

        public override List<Ability> getUniquePowers()
        {
            return powers;
        }
    }
}
