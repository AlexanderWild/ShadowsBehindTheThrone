using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class God_Flesh : God
    {
        public List<Ability> powers = new List<Ability>();

        public God_Flesh()
        {
            powers.Add(new Ab_UnholyFlesh_Seed());
            powers.Add(new Ab_UnholyFlesh_Screetching());
            powers.Add(new Ab_UnholyFlesh_Attack());
            powers.Add(new Ab_UnholyFlesh_Defend());
            powers.Add(new Ab_UnholyFlesh_Grow());
        }


        public override string getDescFlavour()
        {
            return "The Darkness does not come to end all life, but merely to grow a new way to live. Life springs up in great masses, emerging from the ground and reaching into the world to take its first breaths." +
                "\n\nThis new life is terrible, blasphemous and utterly hostile to all human life. The world of men will be replaced by an abomination of flesh and tooth, serrated claw and twitching eyes.";
        }

        public override string getDescMechanics()
        {
            return "This Name works by creating a hostile mass of flesh stretching across continents." +
                " While the vast creature cannot prevail against strong foes, it can chip away at weakened empires, if you can provoke civil wars or international conflicts.";
        }

        public override string getName()
        {
            return "New Life";
        }

        public override List<Ability> getUniquePowers()
        {
            return powers;
        }
    }
}
