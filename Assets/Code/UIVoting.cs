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
        public Image agentBack;
        public Image agentMid;
        public Image agentEyes;
        public Image agentMouth;
        public Image agentHair;
        public Image agentJewel;
        public Image agentFore;
        public Image agentBorder;
        public VoteSession sess;
        public Text textVoterName;
        public Text textVoterUtility;
        public Text textVoterCurrUtility;
        public Text textSwitchCostLiking;
        public Text textSwitchCostPower;
        public Text textVPower;
        public Text textVLiking;
        public Text textWinningOpt;
        public Text textAgentName;
        public Text textAgentDesc;
        public Person agent;
        public Button switchPower;
        public Button switchEnthralled;
        public Button switchLiking;
        public Button viewSelf;

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
            switchEnthralled.gameObject.SetActive(voterBars[0].voter.state == Person.personState.enthralled);

            
        }

        public void populate(Society soc,Person agent)
        {
            this.agent = agent;
            societyName.text = soc.getName();
            textVotingIssue.text = soc.voteSession.issue.ToString();
            textVotingIssueDesc.text = soc.voteSession.issue.getLargeDesc();
            textProposer.text = "Proposed by: " + soc.voteSession.issue.proposer.getFullName();
            if (soc.voteSession.issue.proposer != null)
            {
                World.log("Proposer: " + soc.voteSession.issue.proposer.getFullName());
                World.log("Text " + textProposerVote);
                textProposerVote.text = "Voting For: " + soc.voteSession.issue.proposer.getVote(soc.voteSession).info(soc.voteSession.issue);
                proposerBack.sprite = soc.voteSession.issue.proposer.getImageBack();
                proposerMid.sprite = soc.voteSession.issue.proposer.getImageMid();
                proposerFore.sprite = soc.voteSession.issue.proposer.getImageFore();
                proposerBorder.sprite = soc.voteSession.issue.proposer.getImageBorder();
            }
            else
            {
                Person arbitrary = soc.getSovreign();
                if (arbitrary == null) { arbitrary = soc.people[0]; }
                if (arbitrary != null) {
                    textProposerVote.text = "";
                    proposerBack.sprite = arbitrary.getImageBack();
                    proposerMid.sprite = arbitrary.getImageMid();
                    proposerFore.sprite = arbitrary.getImageFore();
                    proposerBorder.sprite = arbitrary.getImageBorder();
                }

            }
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
            viewSelf.gameObject.SetActive(false);
            if (agent != null)
            {
                textAgentName.text = "Interacting with:\n" + agent.getFullName();
                agentBack.sprite = agent.getImageBack();
                agentBorder.sprite = agent.getImageBorder();
                agentFore.sprite = agent.getImageFore();
                agentMid.sprite = agent.getImageMid();
                textAgentDesc.text = "You may use the voter's liking for " + agent.getFullName() + " to sway their votes one way or another, spending this liking as political capital.";

            }
            else
            {
                agentBack.sprite = world.textureStore.icon_mask;
                agentMid.sprite = world.textureStore.icon_mask;
                agentBorder.sprite = world.textureStore.slotCount;
                agentFore.sprite = world.textureStore.icon_mask;
                textAgentName.text = "Interacting without character";
                textAgentDesc.text = "If you interact with a society with an enthralled noble, or use an agent, you can spend the liking the voters have towards them to sway their votes.";
            }


            if (World.staticMap.param.option_useAdvancedGraphics == 1 && agent != null)
            {
                Person p = agent;
                if (p.isMale)
                {
                    agentMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_faces[p.imgAdvFace];
                    agentEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_eyes[p.imgAdvEyes];
                    agentMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_mouths[p.imgAdvMouth];
                    agentHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_hair[p.imgAdvHair];
                    agentJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_jewels[p.imgAdvJewel];
                }
                else
                {
                    agentMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_faces[p.imgAdvFace];
                    agentEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_eyes[p.imgAdvEyes];
                    agentMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_mouths[p.imgAdvMouth];
                    agentHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_hair[p.imgAdvHair];
                    agentJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_jewels[p.imgAdvJewel];

                }
                agentFore.sprite = p.getImageFore();
            }
            else
            {
                agentEyes.sprite = World.self.textureStore.person_advClear;
                agentMouth.sprite = World.self.textureStore.person_advClear;
                agentHair.sprite = World.self.textureStore.person_advClear;
                agentJewel.sprite = World.self.textureStore.person_advClear;
            }


            sess = soc.voteSession;
            checkData();

            if (agent != null && agent == soc.map.overmind.enthralled)
            {
                bGoToSelf();
                viewSelf.gameObject.SetActive(true);
            }
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

            foreach (PopVoterBar bar in voterBars)
            {
                bar.checkData();
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

        public void bGoToSelf()
        {
            for (int i = 0; i < voterBars.Count; i++)
            {
                if (voterBars[i].voter.state == Person.personState.enthralled)
                {
                    while(voterBars[0].voter.state != Person.personState.enthralled)
                    {
                        PopVoterBar element = voterBars[voterBars.Count - 1];
                        voterBars.RemoveAt(voterBars.Count - 1);
                        voterBars.Insert(0, element);
                    }
                    world.audioStore.playClick();
                    checkData();
                    break;
                }
            }
        }
        public void bSwitchVote()
        {
            voterBars[0].voter.forcedVoteOption = voteOptBars[0].opt;
            voterBars[0].voter.forcedVoteSession = sess;
            checkData();
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
