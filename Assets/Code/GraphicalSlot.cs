﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class GraphicalSlot : MonoBehaviour
    {
        public World world;
        public Person inner;
        public Image textBackground;
        public Image border;
        public Image layerBack;
        public Image layerMid;
        public Image layerAdvHair;
        public Image layerAdvEyes;
        public Image layerAdvMouth;
        public Image layerAdvJewel;
        public Image layerFore;
        public Image selectable;
        public Text title;
        public Text subtitle;
        public Text upperRightText;
        public Text lowerRightText;
        public GameObject popupBox;
        public LineRenderer line;

        public Color goodColor;
        public Color neutralColor;
        public Color badColor;

        public GraphicalSlot connection = null;
        public Vector3 targetPosition = Vector3.zero;
        public bool targetEnabled = true;

        public Vector3 offset = Vector3.zero;
        public Color targetStartColor;
        public Color targetEndColor;

        public bool apoptose;
        public int param_apoptosisTime = 30;
        public int apoptosisCount = 0;

        public void setToPlaceholder(string t)
        {
            inner = null;
            int ind = Eleven.random.Next(world.textureStore.layerBack.Count);

            layerBack.sprite = world.textureStore.layerBack[ind];
            layerMid.enabled = false;
            layerFore.enabled = false;
            layerAdvEyes.enabled = false;
            layerAdvMouth.enabled = false;
            layerAdvHair.enabled = false;
            layerAdvJewel.enabled = false;
            border.color = Color.white;

            title.text = "";
            subtitle.text = t;

            lowerRightText.text = "";
            upperRightText.text = "";

            border.sprite = world.textureStore.slotBasic;
        }

        public void setTo(Person p)
        {
            inner = p;
            p.outer = this;

            layerBack.sprite = p.getImageBack();
            layerMid.sprite  = p.getImageMid();
            layerFore.sprite = p.getImageFore();


            if (World.staticMap.param.option_useAdvancedGraphics == 1)
            {
                if (p.isMale)
                {
                    layerMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_faces[p.imgAdvFace];
                    layerAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_eyes[p.imgAdvEyes];
                    layerAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_mouths[p.imgAdvMouth];
                    layerAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_hair[p.imgAdvHair];
                    layerAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_jewels[p.imgAdvJewel];
                }
                else
                {
                    layerMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_faces[p.imgAdvFace];
                    layerAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_eyes[p.imgAdvEyes];
                    layerAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_mouths[p.imgAdvMouth];
                    layerAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_hair[p.imgAdvHair];
                    layerAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_jewels[p.imgAdvJewel];

                }
                layerFore.sprite = p.getImageFore();
            }
            else
            {
                layerAdvEyes.sprite = p.map.world.textureStore.person_advClear;
                layerAdvMouth.sprite = p.map.world.textureStore.person_advClear;
                layerAdvHair.sprite = p.map.world.textureStore.person_advClear;
                layerAdvJewel.sprite = p.map.world.textureStore.person_advClear;
            }

            float f = (float)(1 - p.shadow);
            if (f > 1) { f = 1; }
            if (f< 0) { f = 0; }
            border.color = new Color(f, f, f);
            title.text = p.getFullName();
            subtitle.text = (p.title_land == null) ? "" : p.title_land.getName();

            if (p.society.getSovereign() == p)
            {
                border.sprite = p.map.world.textureStore.slotKing;
            }
            else if (p.titles.Count > 0)
            {
                border.sprite = p.map.world.textureStore.slotDuke;
            }
            else if (p.title_land != null)
            {
                border.sprite = p.map.world.textureStore.slotCount;
            }
            else
            {
                border.sprite = p.map.world.textureStore.slotBasic;
            }
        }

        public void recenter()
        {
            offset = Vector3.zero;
            transform.position = new Vector3(0.0f, 0.0f, 0.1f);
            line.SetPosition(0, transform.position);
        }

        public void OnMouseDown()
        {
            if (inner == null)
                return;

            GraphicalSociety.refresh(inner);
            World.staticMap.world.ui.checkData();
        }

        public void OnMouseEnter()
        {
            if (upperRightText.text != "")
                popupBox.SetActive(true);
        }

        public void OnMouseExit()
        {
            popupBox.SetActive(false);
        }

        public void Update()
        {
            // if (loc != null)
            // {
            //     Vector3 delta = GraphicalSociety.offset + loc - transform.position;
            //     if (delta.magnitude > 0.02f)
            //     {
            //         delta *= 0.04f;
            //     }
            //     transform.Translate(delta);
            // }

            if (targetEnabled)
                transform.position = Vector3.Lerp(transform.position, targetPosition+offset, 0.1f);
            line.SetPosition(0, transform.position);

            Vector3 origin = (connection == null) ? transform.position : connection.gameObject.transform.position;
            line.SetPosition(1, new Vector3(origin.x, origin.y, 0.1f));

            line.startColor = Color.Lerp(line.startColor, targetStartColor, 0.1f);
            line.endColor   = Color.Lerp(line.endColor, targetEndColor, 0.1f);

            if (inner != null && GraphicalSociety.loadedSlots.Contains(this) == false)
            {
                apoptose = true;
            }
            if (inner == null && GraphicalSociety.loadedPlaceholders.Count == 0)
            {
                apoptose = true;
            }

            selectable.gameObject.SetActive(this.inner == GraphicalSociety.focus);

            if (apoptose)
            {
                try
                {
                    //GraphicalSociety.loadedSlots.Remove(this);
                    //if (inner != null) { inner.outer = null; }
                    Destroy(this.gameObject);
                }
                catch
                {
                    //Do nothing, wait for next turn to try to delete yourself
                }
            }
        }

        public void checkData()
        {
            bool shouldDarken = false;
            if (GraphicalSociety.state == GraphicalSociety.viewState.HIERARCHY)
            {
                if (GraphicalSociety.focus != null && this.inner != null)
                {
                    if (GraphicalSociety.focus == GraphicalSociety.activeSociety.getSovereign() || this.inner == GraphicalSociety.activeSociety.getSovereign())
                    {
                        //No darken needed
                    }
                    else if (this.inner.getLocation() != null && GraphicalSociety.focus.getLocation() != null)
                    {
                        if (this.inner.getLocation().province != GraphicalSociety.focus.getLocation().province)
                        {
                            shouldDarken = true;
                        }
                    }
                }
            }
            else
            {
                shouldDarken = false;
            }
            if (shouldDarken)
            {
                Color darken = new Color(0.5f, 0.5f, 0.5f, 0.25f);
                layerBack.color = darken;
                if (inner != null)
                {
                    float f = (float)(1 - inner.shadow);
                    f /= 2;
                    if (f > 1) { f = 1; }
                    if (f < 0) { f = 0; }
                    border.color = new Color(f, f, f, 0.25f);
                }
                else
                {
                    border.color = darken;
                }
                title.color = new Color(1, 1, 0, 0.25f);
                subtitle.color = new Color(1, 1, 1, 0.25f);
                textBackground.color = new Color(0, 0, 0, 0.25f);
                layerBack.color = darken;
                layerMid.color = darken;
                layerAdvHair.color = darken;
                layerAdvEyes.color = darken;
                layerAdvMouth.color = darken;
                layerAdvJewel.color = darken;
                layerFore.color = darken;
            }
            else
            {
                if (inner != null)
                {
                    float f = (float)(1 - inner.shadow);
                    if (f > 1) { f = 1; }
                    if (f < 0) { f = 0; }
                    border.color = new Color(f, f, f);
                }
                else
                {
                    border.color = Color.white;
                }
                title.color = new Color(1, 1, 0, 1f);
                subtitle.color = new Color(1, 1, 1, 1f);
                textBackground.color = new Color(0, 0, 0, 0.75f);
                layerBack.color = Color.white;
                layerMid.color = Color.white;
                layerAdvHair.color = Color.white;
                layerAdvEyes.color = Color.white;
                layerAdvMouth.color = Color.white;
                layerAdvJewel.color = Color.white;
                layerFore.color = Color.white;
            }
        }
        /*
        public void setTo(Society soc,Person core)
        {
            checkData();
            if (this.inner == null)
            {
                upperRightText.text = "";
                lowerRightText.text = "";
                title.text = "Empty " + slot.title;
            }
            else
            {
                title.text = inner.getRankAndName();
            }
            setToStats();
        }

        public void setTo(SocLens lens,Person core)
        {
            checkData();
            if (this.inner == null)
            {
                upperRightText.text = "";
                lowerRightText.text = "";
                riseBox.text = "";
                fallBox.text = "";
                title.text = "Empty " + slot.title;
            }else
            {
                title.text = inner.getRankAndName();
                upperRightText.text = lens.getUpperRightText(core, inner);
                upperRightText.color = lens.getUpperRightTextColour(core, inner);
                lowerRightText.text = lens.getLowerRightText(core, inner);
                lowerRightText.color = lens.getLowerRightTextColour(core, inner);
                if (inner.slot.master != null)
                {
                    riseBox.text = "Support for Promotion: " + inner.slot.getRise() + "%";
                }else { riseBox.text = ""; }
            }
            setSubtitle(core);
        }

        public void checkData()
        {
            if (world.ui.state == UIMaster.uiState.WORLD)
            {
                selectable.color = Color.clear;
            }
            else if (world.ui.state == UIMaster.uiState.SELECT_SOC)
            {
                if (inner != null && world.ui.viewSelector.selectable(inner))
                {
                    selectable.color = Color.white;
                }
                else
                {
                    selectable.color = Color.clear;
                }
            }
            if (inner != null && inner.dead) { inner = null; }

            if (inner != null)
            {
                layerBack.sprite = inner.getLayerBack();
                layerMid.sprite = inner.getLayerMid();
                layerFore.sprite = inner.getLayerFore();


                if (inner.enthralled)
                {
                    layerBack.sprite = world.textureStore.sprites["profileEnthralled-vampire_0.png"];
                }

                title.text = inner.getRankAndName();
                border.sprite = inner.getLayerSlot();

                float f = 1 - inner.shadow;
                if (f > 1) { f = 1; }
                if (f < 0) { f = 0; }
                border.color =  new Color(f, f, f);
            }
            else
            {
                //Is setting it to null even a permissable thing?
                layerBack.sprite = null;
                layerMid.sprite = null;
                layerFore.sprite = null;
                border.color = Color.white;
            }


            if (slot != null)
            {
                //border.sprite = slot.getSprite();
            }
        }

        public void setSubtitle(Person other)
        {
            subtitle.text = "";
            if (inner == null) { return; }

            RelObj rel = this.inner.getRel(other);
            string str = "";
            if (rel.state == RelObj.STATE_SPOUSE)
            {
                str = "Spouse ";
            }
            else if (rel.state == RelObj.STATE_RELATIVE)
            {
                str = "Relative ";
            }

            if (rel.parent.advisorTo == rel.target)
            {
                str += "Advisor ";
            }else if (rel.target.advisorTo == rel.parent)
            {
                str += "Boss ";
            }
            subtitle.text = str;
        }

        public void setToStats()
        {
            if (inner != null)
            {
                title.text = inner.getRankAndName();
                //upperRightText.text = "Money: " + (int)inner.cash + "\nMult: " + (int)(100 * inner.getEconMult()) + "\nTrust: " + (int)(100 * inner.globalTrustworthiness);
                upperRightText.text = "Income: " + (int)inner.getIncome() + "\nBase Trust: " + (int)(100 * inner.globalTrustworthiness);
                upperRightText.color = Color.yellow;
                //lowerRightText.text = "Prestige: " + (int)(inner.prestige) + "\nGossips: " + inner.gossipKnown.Count;
                lowerRightText.text = "Prestige: " + (int)(inner.prestige);
                lowerRightText.color = Color.yellow;
            }else
            {
                upperRightText.text = "";
                upperRightText.color = Color.yellow;
                lowerRightText.text = "";
                lowerRightText.color = Color.yellow;
            }
        }

        public void OnMouseOver()
        {
            if (GraphicalSociety.personOver != this)
            {
                GraphicalSociety.personOver = this;
                world.uiSocCanvas.personOverChanges(this.inner);
            }
        }

        public void OnMouseExit()
        {
            if (GraphicalSociety.personOver != null)
            {
                GraphicalSociety.personOver = null;
                world.uiSocCanvas.personOverChanges(null);
            }
            GraphicalSociety.personOver = null;
        }
        */
    }
}
