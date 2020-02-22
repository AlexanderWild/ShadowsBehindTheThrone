using UnityEngine;


namespace Assets.Code
{
    public abstract class God
    {
        public abstract string getName();
        public abstract string getDescMechanics();
        public abstract string getDescFlavour();
        public abstract void onStart(Map map);
    }
}