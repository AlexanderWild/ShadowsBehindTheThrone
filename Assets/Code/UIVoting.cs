using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIVoting : MonoBehaviour
    {
        public World world;
        public List<PopVoterBar> voterBars = new List<PopVoterBar>();
        public List<PopOptBar> voteOptBars = new List<PopOptBar>();
        public Text societyName;
        public Text textVotingIssue;
        public Text textVotingIssueDesc;
        public Text textProposer;
        public Text textProposerVote;
        public Image proposerBack;
        public Image proposerMid;
        public Image proposerFore;
        public Image proposerBorder;
        public VoteSession sess;
        public Text textVoterName;
        public Text textVoterUtility;
        public Text textVoterCurrUtility;
        public Text textSwitchCostLiking;
        public Text textSwitchCostPower;
        public Text textVPower;
        public Text textVLiking;
        public Text textWinningOpt;
        public Person agent;
        public Button switchPower;
        public Button switchLiking;

        public float offsetPrimary = 0;
        public float offset = 0;
        public float offsetPerItem = -65;

        public void Update()
        {
            if (world.ui.state != UIMaster.uiState.VOTING)
            {
                return;
            }
            voterBars[0].targetPosition = offsetPrimary;
            for (int i = 1; i < voterBars.Count; i++)
            {
                voterBars[i].targetPosition = offset + (i*offsetPerItem);
            }
            voteOptBars[0].targetPosition = offsetPrimary;
            for (int i = 1; i < voteOptBars.Count; i++)
            {
                voteOptBars[i].targetPosition = offset + (i * offsetPerItem);
            }
        }

        public void populate(Society soc,Person agent)
        {
            this.agent = agent;
            societyName.text = soc.getName();
            textVotingIssue.text = soc.voteSession.issue.ToString();
            textVotingIssueDesc.text = soc.voteSession.issue.getLargeDesc();
            textProposer.text = "Proposed by: " + soc.voteSession.issue.proposer.getFullName();
            textProposerVote.text = "Voting For: " + soc.voteSession.issue.proposer.getVote(soc.voteSession).info(soc.voteSession.issue);
            proposerBack.sprite = soc.voteSession.issue.proposer.getImageBack();
            proposerMid.sprite = soc.voteSession.issue.proposer.getImageMid();
            proposerFore.sprite = soc.voteSession.issue.proposer.getImageFore();
            proposerBorder.sprite = soc.voteSession.issue.proposer.getImageBorder();
            foreach (Person p in soc.people){
                PopVoterBar bar = world.prefabStore.getVoterBar(p);
                //bar.gameObject.transform.parent = this.gameObject.transform;
                voterBars.Add(bar);
            }
            foreach (VoteOption opt in soc.voteSession.issue.options)
            {
                PopOptBar bar = world.prefabStore.getVoteOptBar(opt, soc.voteSession);
                //bar.gameObject.transform.parent = this.gameObject.transform;
                voteOptBars.Add(bar);
            }
            sess = soc.voteSession;
            checkData();
        }

        public void checkData()
        {
            sess.assignVoters();
            textVoterName.text = "Voter: " + voterBars[0].voter.getFullName();
            textVPower.text = "" + (int)(world.map.overmind.power);
            if (agent != null)
            {
                textVLiking.text = "" + (int)voterBars[0].voter.getRelation(agent).getLiking();
            }
            else
            {
                textVLiking.text = "N/A";
            }
            double uSel = sess.issue.computeUtility(voterBars[0].voter, voteOptBars[0].opt, new List<ReasonMsg>());
            double uCurr = sess.issue.computeUtility(voterBars[0].voter, voterBars[0].voter.getVote(sess), new List<ReasonMsg>());
            textVoterUtility.text = "Opinion of selected option: " + (int)uSel;
            textVoterCurrUtility.text = "Opinion of their current option: " + (int)uCurr;

            if (voterBars[0].voter.state != Person.personState.enthralled && voterBars[0].voter.getVote(sess) != voteOptBars[0].opt)
            {
                double utilityDelta = uCurr - uSel;
                if (utilityDelta < 0) { utilityDelta = 0; }
                textSwitchCostPower.text = "" + (int)((utilityDelta * world.map.param.voting_powerToSwitchPerU) + world.map.param.voting_powerToSwitchMin);
                textSwitchCostLiking.text = "" + (int)((utilityDelta * world.map.param.voting_likingToSwitchPerU) + world.map.param.voting_likingToSwitchMin);
                switchLiking.gameObject.SetActive(true);
                switchPower.gameObject.SetActive(true);
            }
            else
            {
                textSwitchCostPower.text = "N/A";
                textSwitchCostLiking.text = "N/A";
                switchLiking.gameObject.SetActive(false);
                switchPower.gameObject.SetActive(false);
            }

            double highest = voteOptBars[0].opt.votingWeight;
            VoteOption winner = voteOptBars[0].opt;
            for (int i = 1; i < voteOptBars.Count; i++)
            {
                if (voteOptBars[i].opt.votingWeight > highest)
                {
                    highest = voteOptBars[i].opt.votingWeight;
                    winner = voteOptBars[i].opt;
                }
            }
            textWinningOpt.text = winner.info(sess.issue);
        }

        public void bSwitchUsingPower()
        {
            if (world.map.overmind.power < 0)
            {
                world.audioStore.playClick();
                world.prefabStore.popMsg("You currently have no remaining power");
                return;
            }
            double uSel = sess.issue.computeUtility(voterBars[0].voter, voteOptBars[0].opt, new List<ReasonMsg>());
            double uCurr = sess.issue.computeUtility(voterBars[0].voter, voterBars[0].voter.getVote(sess), new List<ReasonMsg>());

            double utilityDelta = uCurr - uSel;
            if (utilityDelta < 0) { utilityDelta = 0; }
            world.map.overmind.power -= (int)(utilityDelta * world.map.param.voting_powerToSwitchPerU) + world.map.param.voting_powerToSwitchMin;

            world.audioStore.playActivate();
            voterBars[0].voter.forcedVoteOption = voteOptBars[0].opt;
            voterBars[0].voter.forcedVoteSession = sess;
            checkData();
        }
        public void bSwitchUsingLiking()
        {
            double uSel = sess.issue.computeUtility(voterBars[0].voter, voteOptBars[0].opt, new List<ReasonMsg>());
            double uCurr = sess.issue.computeUtility(voterBars[0].voter, voterBars[0].voter.getVote(sess), new List<ReasonMsg>());

            double utilityDelta = uCurr - uSel;
            if (utilityDelta < 0) { utilityDelta = 0; }
            double delta = (utilityDelta * world.map.param.voting_likingToSwitchPerU) + world.map.param.voting_likingToSwitchMin;

            RelObj rel = voterBars[0].voter.getRelation(agent);
            if (rel.getLiking() >= delta)
            {
                rel.addLiking(-delta, "Asked to switch vote", world.map.turn);
                world.audioStore.playActivate();
            }
            else
            {
                world.audioStore.playClick();
                world.prefabStore.popMsg(voterBars[0].voter.getFullName() + " does not like " + agent.getFullName() + " to agree to change to this voting option. Their liking is " + (int)(rel.getLiking())
                    + " but you need " + (int)(delta));
                return;
            }
            voterBars[0].voter.forcedVoteOption = voteOptBars[0].opt;
            voterBars[0].voter.forcedVoteSession = sess;
            checkData();
        }

        public void bViewReasons()
        {
            world.audioStore.playClick();
            
            world.ui.addBlocker(world.prefabStore.getScrollSetVotes(voterBars[0].voter, sess.issue).gameObject);
        }
        public void bVotingNext()
        {
            world.audioStore.playClick();
            PopVoterBar element = voterBars[0];
            voterBars.RemoveAt(0);
            voterBars.Add(element);
            checkData();
        }
        public void bVotingPrev()
        {
            world.audioStore.playClick();
            PopVoterBar element = voterBars[voterBars.Count-1];
            voterBars.RemoveAt(voterBars.Count-1);
            voterBars.Insert(0, element);
            checkData();
        }
        public void bOptNext()
        {
            world.audioStore.playClick();
            PopOptBar element = voteOptBars[0];
            voteOptBars.RemoveAt(0);
            voteOptBars.Add(element);
            checkData();
        }
        public void bOptPrev()
        {
            world.audioStore.playClick();
            PopOptBar element = voteOptBars[voteOptBars.Count - 1];
            voteOptBars.RemoveAt(voteOptBars.Count - 1);
            voteOptBars.Insert(0, element);
            checkData();
        }

        public void bDismiss()
        {

            world.audioStore.playClick();
            for (int i = 0; i < voterBars.Count; i++)
            {
                Destroy(voterBars[i].gameObject);
            }
            voterBars.Clear();
            for (int i = 0; i < voteOptBars.Count; i++)
            {
                Destroy(voteOptBars[i].gameObject);
            }
            voteOptBars.Clear();
            world.ui.setToWorld();
            agent = null;
            sess = null;
        }

    }
}
