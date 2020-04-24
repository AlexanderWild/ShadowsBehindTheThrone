using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Assets.Code
{
    public class PopOptBar : MonoBehaviour
    {
        public GameObject mover;
        public Text textName;
        public Text textResults;
        public VoteOption opt;

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
            textResults.text = "Voters for: " + opt.votesFor.Count + "\nTotal Prestige: " + (int)opt.votingWeight;
        }
        public void setTo(VoteOption option,VoteSession sess)
        {
            this.opt = option;
            textName.text = option.info(sess.issue);
        }
    }
}
