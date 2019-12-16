using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    /**
     * Used to hold all manner of items. Exact semantics of this object is dependent on the exact voting issue itself
     */
    public class VoteOption
    {
        public Person person;
        public Location location;
        public SocialGroup group;
        public EconTrait econ_from;
        public EconTrait econ_to;
        public int index = -1;//For stuff which doesn't have a discrete target, such as "Declare war yes/no"

        public SavableMap_Person_Double randomUtility = new SavableMap_Person_Double();
        //public SavableMap_Person_ListMsg msgs = new SavableMap_Person_ListMsg();
        public List<Person> votesFor = new List<Person>();


        public double votingWeight
        {
            get
            {
                double sum = 0;
                foreach (Person p in votesFor)
                {
                    sum += p.prestige;
                }
                return sum;
            }
            private set
            {

            }
        }


        public double getBaseUtility(Person voter)
        {
            if (randomUtility.ContainsKey(voter))
            {
                return randomUtility.lookup(voter);
            }
            double u = Eleven.random.NextDouble()*0.0001;
            randomUtility.Add(voter, u);
            return u;
        }
        public override string ToString()
        {
            return info();
        }
        public string info(VoteIssue issue, bool shrt = false)
        {
            string reply = "";
            if (econ_from != null)
            {
                if (shrt)
                    return econ_from.name + " -> " + econ_to.name;
                else
                    return "Move economic priority away from " + econ_from.name + " to benefit " + econ_to.name;
            }
            if (issue is VoteIssue_MilitaryStance)
            {
                if (shrt)
                    return new string[] { "Defensive", "Offensive", "Introspective" }[index];
                else
                    return new string[] { "take Defensive Stance", "take Offensive Stance", "take Introspective Stance" }[index];
            }
            if (issue is VoteIssue_DeclareWar)
            {
                if (shrt)
                    return new string[] { "Peace", "War" }[index];
                else
                    return new string[] { "Maintain Peace","Declare War" }[index];
            }
            if (issue is VoteIssue_JudgeSuspect)
            {
                if (shrt)
                    return new string[] { "Innocent", "Guilty" }[index];
                else
                    return new string[] { "Pronounce Innocent", "Pronounce Guilty" }[index];
            }
            if (issue is VoteIssue_Vassalise)
            {
                if (shrt)
                    return new string[] { "Stay", "Merge" }[index];
                else
                    return new string[] { "Remain Independent", "Become a Vassal" }[index];
            }

            if (person != null) { reply += person.getFullName() + " "; }
            if (location != null) { reply += location.getName() + " "; }
            if (group != null) { reply += group.getName() + " "; }
            if (econ_from != null) { reply += econ_from.name + " "; }
            if (econ_to != null) { reply += econ_to.name + " "; }
            if (index != -1) { reply += issue.getIndexInfo(index) + " "; }

            return reply;
        }
        public string info()
        {
            string reply = "";
            if (person != null) { reply += person.getFullName() + " "; }
            if (location != null) { reply += location.getName() + " "; }
            if (group != null) { reply += group.getName() + " "; }
            if (econ_from != null) { reply += econ_from.name + " "; }
            if (econ_to != null) { reply += econ_to.name + " "; }
            if (index != -1) { reply += index + " "; }

            return reply;
        }

        public string fixedLenInfo()
        {
            string line = "";
            VoteOption opt = this;
            if (opt.econ_from != null) { line += " eFrom  " + Eleven.toFixedLen("" + opt.econ_from.name, 10); }
            if (opt.econ_to != null) { line += " eTo    " + Eleven.toFixedLen("" + opt.econ_to.name, 10); }
            if (opt.group != null) { line += " group  " + Eleven.toFixedLen("" + opt.group.getName(), 10); }
            if (opt.location != null) { line += " loc    " + Eleven.toFixedLen("" + opt.location.getName(), 10); }
            if (opt.person != null) { line += " person " + Eleven.toFixedLen("" + opt.person.getFullName(), 10); }
            if (opt.index != -1) { line += " person " + Eleven.toFixedLen("" + opt.index, 10); }
            return line;
        }
    }
}
