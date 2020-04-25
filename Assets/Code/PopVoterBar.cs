using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Assets.Code
{
    public class PopVoterBar : MonoBehaviour
    {
        public GameObject mover;
        public Image profileBack;
        public Image profileMid;
        public Image profileFore;
        public Image profileBorder;
        public Text textName;
        public Text textPrestige;
        public Text textVotingOption;
        public Person voter;

        public float targetPosition;

        public void Update()
        {
            
            float pos = mover.transform.localPosition[1];
            float delta = targetPosition - pos;
            delta *= 0.1f;
            if (delta < 0) { pos = targetPosition; }
            else
            {
                if (Math.Abs(delta) < 0.1) { delta = targetPosition - pos; }
            }
            mover.transform.localPosition = new Vector3(100, pos + delta, 0);
        }
        public void checkData()
        {
            textVotingOption.text = "Voting: " + voter.getVote(voter.society.voteSession).info(voter.society.voteSession.issue);

        }
        public void setTo(Person p)
        {
            profileBack.sprite = p.getImageBack();
            profileMid.sprite = p.getImageMid();
            profileFore.sprite = p.getImageFore();
            profileBorder.sprite = p.getImageBorder();
            textName.text = p.getFullName();
            textPrestige.text = "Prestige: " + (int)p.prestige;
            textVotingOption.text = "Voting: " + p.getVote(p.society.voteSession).info(p.society.voteSession.issue);
            voter = p;
        }
    }
}
