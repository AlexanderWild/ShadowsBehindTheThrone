using UnityEngine;

namespace Assets.Code
{
    public class Evidence
    {
        public Unit pointsTo;
        public double weight;
        public int turnDropped = 0;

        public Evidence(int turn)
        {
            turnDropped = turn;
        }
    }
}