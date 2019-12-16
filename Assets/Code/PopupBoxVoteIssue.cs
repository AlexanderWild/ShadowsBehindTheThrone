using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxVoteIssue : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public GameObject mover;
        public float targetY;
        public Society soc;
        public VoteIssue issue;
        public Ab_Soc_ProposeVote ability;

        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }
        

        public void setTo(Ab_Soc_ProposeVote ability,Society soc,VoteIssue issue)
        {
            this.soc = soc;
            this.issue = issue;
            this.ability = ability;
            title.text = issue.ToString();
            //body.text = issue.getLargeDesc();
        }

        public float ySize()
        {
            return 100;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            this.ability.turnLastCast = map.turn;

            soc.voteCooldown = 0;
            soc.voteSession = new VoteSession();
            soc.voteSession.issue = issue;
            soc.voteSession.assignVoters();
            soc.voteSession.timeRemaining = map.param.society_votingDuration;

            map.world.prefabStore.popImgMsg("Voting session called. You may now cast your vote on the issue of: " + issue.ToString(),
                map.world.wordStore.lookup("SOC_VOTE_SESSION_CALLED"));
            map.overmind.hasTakenAction = true;
        }

        public string getTitle()
        {
            return issue.ToString();
        }

        public string getBody()
        {
            return issue.getLargeDesc();
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return true;
        }
    }
}
