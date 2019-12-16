using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class ButtonPortrait : MonoBehaviour
    {
        public Text nameTxt;
        public Text infoTxt;

        public Button button;
        public Image foreground;
        public Image background;

        // FIXME
        public Sprite personFrame;
        public Sprite personHead;
        public Sprite votesIcon;

        public void SetInfo(Person p)
        {
            foreground.sprite = personHead;
            background.sprite = personFrame;
            background.enabled = true;

            nameTxt.text  = p.getFullName();
            if (p.title_land != null)
                infoTxt.text = p.title_land.getName();
            else
                infoTxt.text = "Resident";

            infoTxt.text += "\n" + p.prestige + " Prestige";
        }

        public void SetInfo(Settlement s)
        {
            foreground.sprite = s.getSprite();
            background.enabled = false;

            nameTxt.text = s.name;
            if (s.title != null && s.title.heldBy != null)
                infoTxt.text = "Overseen by " + s.title.heldBy.getFullName();
            else
                infoTxt.text = "No Overseer";

            infoTxt.text += "\n" + s.getPrestige() + " Prestige";
        }

        public void SetInfo(VoteOption v)
        {
            foreground.sprite = votesIcon;
            background.enabled = false;

            nameTxt.text = "For " + v.info();
            infoTxt.text = v.votingWeight + " Influence";
        }
    }
}
