using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class Portrait : MonoBehaviour
    {
        public Text name;
        public Text info;
        public Text name2;
        public Text info2;

        public Image foreground;
        public Image midground;
        public Image background;

        // FIXME
        public Sprite personFrame;
        public Sprite personHead;
        public Sprite votesIcon;

        public void SetInfo(Person p)
        {
            foreground.sprite = p.getImageFore();
            midground.sprite  = p.getImageMid();
            background.sprite = p.getImageBack();
            foreground.enabled = true;
            midground.enabled = true;
            background.enabled = true;

            name.text  = p.getFullName();
            if (p.title_land != null)
                info.text = p.title_land.getName();
            else
                info.text = "Resident";

            info.text += "\n" + p.prestige.ToString("N0") + " prestige";

            name2.text = "";
            info2.text = "";
        }

        public void SetInfo(Settlement s)
        {
            foreground.sprite = s.getSprite();
            foreground.enabled = true;
            midground.enabled = false;
            background.enabled = false;

            name.text = s.name;
            if (s.title != null && s.title.heldBy != null)
                info.text = "Overseen by " + s.title.heldBy.getFullName();
            else
                info.text = "No Overseer";

            info.text += "\n" + s.getPrestige().ToString("N0") + " prestige";

            name2.text = "";
            info2.text = "";
        }

        public void SetInfo(VoteIssue i, VoteOption v, Person p)
        {
            foreground.sprite = votesIcon;
            foreground.enabled = false;
            midground.enabled = false;
            background.enabled = false;

            name.text = "";
            info.text = "";

            name2.text = "For " + v.info(i, true);
            info2.text = v.votingWeight.ToString("N0") + " influence across " + v.votesFor.Count + " votes";

            if (p != null)
            {
                // FIXME: use the list of reasons somewhere
                double utility = i.computeUtility(p, v, new List<ReasonMsg>());
                info2.text += "\n" + p.getFullName() + " values at " + utility.ToString("N0");
            }
        }
    }
}
