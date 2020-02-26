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
            map.overmind.powers.AddRange(this.getUniquePowers());
        }
    }
}