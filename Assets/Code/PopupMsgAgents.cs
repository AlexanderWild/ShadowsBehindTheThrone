using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupMsgAgents: MonoBehaviour
    {
        public Text text;
        public Text agentATitle;
        public Text agentBTitle;
        public Button bDismiss;
        public Button bDismissAgentA;
        public Button bDismissAgentB;
        public UIMaster ui;
        public Unit agentA;
        public Unit agentB;
        public Image agentAIcon;
        public Image agentBIcon;

        public Image paBack;
        public Image paMid;
        public Image paFore;
        public Image paAdvEyes;
        public Image paAdvHair;
        public Image paAdvMouth;
        public Image paAdvJewel;
        public Image pbBack;
        public Image pbMid;
        public Image pbFore;
        public Image pbAdvEyes;
        public Image pbAdvHair;
        public Image pbAdvMouth;
        public Image pbAdvJewel;

        public void setTo(Unit agentA,Unit agentB)
        {
            if (agentA == null || agentB == null) { throw new Exception("Agent was null"); }
            agentATitle.text = agentA.getName();
            agentBTitle.text = agentB.getName();
            agentAIcon.sprite = agentA.getSprite(agentA.location.map.world);
            agentBIcon.sprite = agentB.getSprite(agentA.location.map.world);

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
            if (agentB.person != null)
            {
                pbBack.sprite = agentB.person.getImageBack();
                pbMid.sprite = agentB.person.getImageMid();
                pbFore.sprite = agentB.person.getImageFore();
                if (World.staticMap.param.option_useAdvancedGraphics == 1)
                {
                    Person p = agentB.person;
                    if (p.isMale)
                    {
                        pbMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_faces[p.imgAdvFace];
                        pbAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_eyes[p.imgAdvEyes];
                        pbAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_mouths[p.imgAdvMouth];
                        pbAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_hair[p.imgAdvHair];
                        pbAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_jewels[p.imgAdvJewel];
                    }
                    else
                    {
                        pbMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_faces[p.imgAdvFace];
                        pbAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_eyes[p.imgAdvEyes];
                        pbAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_mouths[p.imgAdvMouth];
                        pbAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_hair[p.imgAdvHair];
                        pbAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_jewels[p.imgAdvJewel];

                    }
                }
            }
        }

        public void dismissAgentA()
        {
            GraphicalMap.panTo(agentA.location.hex.x, agentA.location.hex.y);
            GraphicalMap.selectedSelectable = agentA;
            ui.removeBlocker(this.gameObject);
        }
        public void dismissAgentB()
        {
            GraphicalMap.panTo(agentB.location.hex.x, agentB.location.hex.y);
            GraphicalMap.selectedSelectable = agentB;
            ui.removeBlocker(this.gameObject);
        }
        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
