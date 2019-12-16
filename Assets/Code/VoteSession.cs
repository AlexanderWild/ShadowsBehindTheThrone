using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteSession
    {
        public VoteIssue issue;
        public int timeRemaining;

        internal void assignVoters()
        {

            foreach (VoteOption opt in issue.options)
            {
                opt.votesFor.Clear();
            }

            foreach (Person p in this.issue.society.people)
            {
                //if (p.state == Person.personState.enthralled) { continue; }

                VoteOption bestChoice = p.getVote(this);
                bestChoice.votesFor.Add(p);
            }

            issue.options.Sort(delegate(VoteOption a, VoteOption b) { return b.votingWeight.CompareTo(a.votingWeight); });
        }
    }
}
