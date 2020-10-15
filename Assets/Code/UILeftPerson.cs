using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class UILeftPerson : MonoBehaviour
    {
        public Image profileBack;
        public Image profileMid;
        public Image profileFore;
        public Image profileBorder;

        public Text personTitle;
        public Text locText;
        public Text personBody;
        public Text personAwarenss;
        public Text personAwarenssDesc;
        public Text personShadowAndEvidenceVals;
        public Text personVoting;

        public void setTo(Person p)
        {
            if (p == null) { setToNull(); return; }

            profileBack.enabled = true;
            profileMid.enabled = true;
            profileFore.enabled = true;
            profileBorder.enabled = true;
            //Done to unfuck the distortion of images which periodically occurs
            profileBack.sprite = null;
            profileMid.sprite = null;
            profileFore.sprite = null;
            profileBorder.sprite = null;
            profileBack.sprite = p.getImageBack();
            profileMid.sprite = p.getImageMid();
            profileFore.sprite = p.getImageFore();
            if (p.society.getSovreign() == p) { profileBorder.sprite = p.map.world.textureStore.slotKing; }
            else if (p.titles.Count > 0) { profileBorder.sprite = p.map.world.textureStore.slotDuke; }
            else { profileBorder.sprite = p.map.world.textureStore.slotCount; }

            TitleLanded title = p.title_land;
            if (title == null)
            {
                locText.text = "No Landed Title";
            }
            else
            {
                locText.text = "of " + title.settlement.name;
            }

            Society soc = p.society;
            VoteSession vote = (soc != null) ? soc.voteSession : null;

            if (vote != null)
            {
                personVoting.text = "Voting: " +  p.getVote(vote).info(vote.issue);
            }
            else
            {
                personVoting.text = "Not voting";
            }
        }

        public void setToNull()
        {
            profileBack.enabled = false;
            profileMid.enabled = false;
            profileFore.enabled = false;
            profileBorder.enabled = false;
            personTitle.text = "No Person Selected";
            personVoting.text = "";
        }
    }
}
