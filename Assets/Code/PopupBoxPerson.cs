using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxPerson : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        //public Selector_Person selector;
        public Person person;
        public Image layerBack;
        public Image layerMid;
        public Image layerAdvEyes;
        public Image layerAdvMouth;
        public Image layerAdvHair;
        public Image layerAdvJewel;
        public Image layerFore;
        public Image layerBorder;
        public GameObject mover;
        public float targetY;
        public Person agent;


        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        /*
        public void setTo(Person viewer,Person p,Selector_Person selector)
        {
            this.agent = viewer;
            this.person = p;
            title.text = p.getRankAndName();
            this.selector = selector;
            layerBack.sprite = p.getLayerBack();
            layerMid.sprite = p.getLayerMid();
            layerFore.sprite = p.getLayerFore();
            layerBorder.sprite = p.getLayerSlot();

            body.text = "Prestige: " + (int)(p.prestige)
                + "\nIncome: " + (int)(p.getIncome());
            if (viewer == null)
            {
                body2.text = "";
            }
            else
            {
                float l = p.getRel(viewer).getLiking();
                l /= 100;
                if (l > 1) { l = 1; }
                if (l < -1) { l = -1; }
                if (l > 0) { body2.color =  new Color(1 - l, 1, 1 - l); }
                else { body2.color = new Color(1, 1 + l, 1 + l); }

                body2.text = "Liking towards " + viewer.getRankAndName() + ": " + (int)(p.getLiking(viewer));
            }
        }
        */

        public float ySize()
        {
            return 75;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            //selector.selected(person,agent);
            if (person.getLocation() != null)
            {
                try
                {
                    GraphicalMap.panTo(person.getLocation().hex.x, person.getLocation().hex.y);
                }
                catch
                {
                    //Ignore
                }
            }
        }

        public string getTitle()
        {
            //return person.getFullName();
            return "";
        }

        public string getBody()
        {
            ///return selector.getDescriptionOfSelection() ;
            return "";
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return true;
        }

        public void setTo(Person person)
        {
            this.person = person;

            title.text = person.getFullName();
            layerBack.sprite = person.getImageBack();
            layerBorder.sprite = person.getImageBorder();
            layerFore.sprite = person.getImageFore();
            layerMid.sprite = person.getImageMid();


            if (World.staticMap.param.option_useAdvancedGraphics == 1)
            {
                Person p = person;
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
                layerAdvEyes.sprite = World.staticMap.world.textureStore.person_advClear;
                layerAdvMouth.sprite = World.staticMap.world.textureStore.person_advClear;
                layerAdvHair.sprite = World.staticMap.world.textureStore.person_advClear;
                layerAdvJewel.sprite = World.staticMap.world.textureStore.person_advClear;

            }
        }
    }
}
