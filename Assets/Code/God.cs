using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public abstract class God
    {
        public abstract string getName();
        public abstract string getDescMechanics();
        public abstract string getDescFlavour();
        public abstract List<Ability> getUniquePowers();

        public virtual void onStart(Map map)
        {
            List<Ability> powers = new List<Ability>();
            List<Ability> powersPostfix = new List<Ability>();
            foreach (Ability a in map.overmind.powers)
            {
                if (a is Ab_Enth_Enthrall)
                {
                    powers.Add(a);
                }
                else
                {
                    powersPostfix.Add(a);
                }
            }
            powers.AddRange(this.getUniquePowers());
            powers.AddRange(powersPostfix);
            map.overmind.powers = powers;
        }

        public virtual Sprite getGodBackground(World world)
        {
            return world.textureStore.painting_nightMoon;
        }
        public virtual string getCredits()
        {
            return "Moonlit Landscape with a View of the New Amstel River and Castle Kostverloren, Aert van der Neer, 1647";
        }
    }
}