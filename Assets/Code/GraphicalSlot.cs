using System;
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
        public SpriteRenderer border;
        public SpriteRenderer layerBack;
        public SpriteRenderer layerMid;
        public SpriteRenderer layerFore;
        public SpriteRenderer selectable;
        public Text title;
        public Text subtitle;
        public Text upperRightText;
        public Text lowerRightText;
        public Text riseBox;
        public Text fallBox;
        public LineRenderer line;

        public Color goodColor;
        public Color neutralColor;
        public Color badColor;

        public Vector3 targetPosition = Vector3.zero;
        public Color targetColor;

        public bool apoptose;
        public int param_apoptosisTime = 30;
        public int apoptosisCount = 0;

        public void setTo(Person p)
        {
            inner = p;

            layerBack.sprite = p.getImageBack();
            layerMid.sprite  = p.getImageMid();
            layerFore.sprite = p.getImageFore();

            title.text = p.getFullName();
            subtitle.text = (p.title_land == null) ? "" : p.title_land.getName();
            // FIXME
            upperRightText.text = lowerRightText.text = riseBox.text = fallBox.text = "";

            targetColor = neutralColor;
        }

        public void OnMouseDown()
        {
            GraphicalSociety.refresh(inner);
            World.staticMap.world.ui.checkData();
        }

        public void OnMouseEnter()
        {
            GraphicalSociety.showHover(inner);
        }

        public void OnMouseExit()
        {
            GraphicalSociety.showHover(null);
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

            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
            line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0.1f));
            line.SetPosition(1, new Vector3(0.0f, 0.0f, 0.1f));

            line.startColor = Color.Lerp(line.startColor, targetColor, 0.1f);
            line.endColor   = line.startColor;

            if (GraphicalSociety.loadedSlots.Contains(this) == false)
            {
                apoptose = true;
            }

            if (apoptose)
            {
                try
                {
                    //GraphicalSociety.loadedSlots.Remove(this);
                    //if (inner != null) { inner.outer = null; }
                    Destroy(this.gameObject);
                }
                catch (Exception e)
                {
                    //Do nothing, wait for next turn to try to delete yourself
                }
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
