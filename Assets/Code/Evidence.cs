using UnityEngine;

namespace Assets.Code
{
    public class Evidence
    {
        public Unit pointsTo;
        public Person pointsToPerson;
        public double weight;
        public int turnDropped = 0;
        public Unit assignedInvestigator;
        public int rumourCounter;
        public int turnSubmitted;
        public Location locationFound;
        public Unit_Investigator discoveredBy;

        public Evidence(int turn)
        {
            turnDropped = turn;
        }
    }
}