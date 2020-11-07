﻿using UnityEngine;
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
        public Image profileAdvEyes;
        public Image profileAdvHair;
        public Image profileAdvMouth;
        public Image profileAdvJewel;

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
            profileAdvEyes.enabled = true;
            profileAdvMouth.enabled = true;
            profileAdvHair.enabled = true;
            profileAdvJewel.enabled = true;
            profileBorder.sprite = null;
            if (p.society.getSovreign() == p) { profileBorder.sprite = p.map.world.textureStore.slotKing; }
            else if (p.titles.Count > 0) { profileBorder.sprite = p.map.world.textureStore.slotDuke; }
            else { profileBorder.sprite = p.map.world.textureStore.slotCount; }

            if (World.staticMap.param.option_useAdvancedGraphics == 1)
            {
                //Null setting done to unfuck the distortion of images which periodically occurs
                profileBack.sprite = null;
                profileMid.sprite = null;
                profileAdvEyes.sprite = null;
                profileAdvMouth.sprite = null;
                profileAdvHair.sprite = null;
                profileAdvJewel.sprite = null;
                profileFore.sprite = null;
                profileBack.sprite = p.getImageBack();
                if (p.isMale)
                {
                    profileMid.sprite = p.map.world.textureStore.cultures[0].m_faces[p.imgAdvFace];
                    profileAdvEyes.sprite = p.map.world.textureStore.cultures[0].m_eyes[p.imgAdvEyes];
                    profileAdvMouth.sprite = p.map.world.textureStore.cultures[0].m_mouths[p.imgAdvMouth];
                    profileAdvHair.sprite = p.map.world.textureStore.cultures[0].m_hair[p.imgAdvHair];
                    profileAdvJewel.sprite = p.map.world.textureStore.cultures[0].m_jewels[p.imgAdvJewel];
                }
                else
                {
                    profileMid.sprite = p.map.world.textureStore.cultures[0].f_faces[p.imgAdvFace];
                    profileAdvEyes.sprite = p.map.world.textureStore.cultures[0].f_eyes[p.imgAdvEyes];
                    profileAdvMouth.sprite = p.map.world.textureStore.cultures[0].f_mouths[p.imgAdvMouth];
                    profileAdvHair.sprite = p.map.world.textureStore.cultures[0].f_hair[p.imgAdvHair];
                    profileAdvJewel.sprite = p.map.world.textureStore.cultures[0].f_jewels[p.imgAdvJewel];
                }
                profileFore.sprite = p.getImageFore();
            }
            else
            {
                profileAdvEyes.enabled = false;
                profileAdvMouth.enabled = false;
                profileAdvJewel.enabled = false;
                profileAdvHair.enabled = false;
                //Null setting done to unfuck the distortion of images which periodically occurs
                profileBack.sprite = null;
                profileMid.sprite = null;
                profileFore.sprite = null;
                profileBack.sprite = p.getImageBack();
                profileMid.sprite = p.getImageMid();
                profileFore.sprite = p.getImageFore();
            }


            personTitle.text = p.getFullName();
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
            profileAdvEyes.enabled = false;
            profileAdvMouth.enabled = false;
            profileAdvHair.enabled = false;
            profileAdvJewel.enabled = false;
            personTitle.text = "No Person Selected";
            personVoting.text = "";
        }
    }
}
