using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupMsgAgentsDeath: MonoBehaviour
    {
        public Text text;
        public Text agentATitle;
        public Button bDismiss;
        public Button bDismissAgentA;
        public UIMaster ui;
        public Unit agentA;
        public Image agentAIcon;

        public Image paBack;
        public Image paMid;
        public Image paFore;
        public Image paAdvEyes;
        public Image paAdvHair;
        public Image paAdvMouth;
        public Image paAdvJewel;

        public void setTo(Unit agentA)
        {
            if (agentA == null) { throw new Exception("Agent was null"); }
            agentATitle.text = agentA.getName();
            agentAIcon.sprite = agentA.getSprite(agentA.location.map.world);

            if (agentA.person != null)
            {
                paBack.sprite = agentA.person.getImageBack();
                paMid.sprite = agentA.person.getImageMid();
                paFore.sprite = agentA.person.getImageFore();
                if (World.staticMap.param.option_useAdvancedGraphics == 1)
                {
                    Person p = agentA.person;
                    if (p.isMale)
                    {
                        paMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_faces[p.imgAdvFace];
                        paAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_eyes[p.imgAdvEyes];
                        paAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_mouths[p.imgAdvMouth];
                        paAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_hair[p.imgAdvHair];
                        paAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_jewels[p.imgAdvJewel];
                    }
                    else
                    {
                        paMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_faces[p.imgAdvFace];
                        paAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_eyes[p.imgAdvEyes];
                        paAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_mouths[p.imgAdvMouth];
                        paAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_hair[p.imgAdvHair];
                        paAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_jewels[p.imgAdvJewel];

                    }
                }
            }
        }

        public void dismissAgentA()
        {
            GraphicalMap.panTo(agentA.location.hex.x, agentA.location.hex.y);
            GraphicalMap.selectedSelectable = null;
            ui.removeBlocker(this.gameObject);
        }
        public void dismiss()
        {
            GraphicalMap.selectedSelectable = null;
            ui.removeBlocker(this.gameObject);
        }
    }
}
