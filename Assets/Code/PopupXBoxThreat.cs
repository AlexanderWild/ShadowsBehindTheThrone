using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupXBoxThreat : MonoBehaviour,PopupXScrollable
    {
        public Text mainText;
        public Text titleText;
        public GameObject mover;
        public float targetX;
        public string body = "";

        public void Update()
        {
            Vector3 loc = new Vector3(targetX,mover.transform.position.y,  mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        public void setTo(ThreatItem item)
        {
            titleText.text = item.getTitle();
            if (item.p != null)
            {
                string t = "";

                List<string> list = item.getReasons();
                foreach (string s in list)
                {
                    t += s + "\n\n";
                }

                t += "\nTotal Threat: " + (int)(item.threat);
                t += "\n\nResponse: " + ThreatItem.responseNames[item.responseCode];

                mainText.text = t;
            }else
            {
                mainText.text = "Average Society-wide Threat Estimate:\n" + (int)(item.threat);
            }
        }

        public void setTo(VoteIssue vi, VoteOption vo, Person p)
        {
            titleText.text = "For " + vo.info(vi);

            List<ReasonMsg> rs = new List<ReasonMsg>();
            double total = vi.computeUtility(p, vo, rs);

            string t = "";
            foreach (ReasonMsg r in rs)
            {
                t += "Influenced (" + r.value.ToString("N0") + ") by\n" + r.msg + "\n\n";
            }

            t += "\nTotal Desirability: " + total.ToString("N0");
            mainText.text = t;
        }

        public float xSize()
        {
            return 300;
        }

        public void setTargetX(float y)
        {
            targetX = y;
        }

        public string getTitle()
        {
            return titleText.text;
        }

        public string getBody()
        {
            return body;

            // return "Each character evaluates the threats to their society independently. Their society then aggregates these together into a singular concensus of what to be afraid of."
            //     + "\nA society gains responses to its highest-rated threat, if this threat scores over 100. This occurs by allowing new types of vote for new types of action and by triggering zeitgeist events.";
        }
    }
}
