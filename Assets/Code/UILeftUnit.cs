using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class UILeftUnit : MonoBehaviour , IComparer<RelObj>
    {
        public Text title;
        public Text desc;
        public Text task;
        public Text taskDesc;
        public Text hasMoved;
        public Text nationText;
        public Text evidenceText;
        public Text specialText;
        public Text specialDesc;
        public Image nationFlag1;
        public Image nationFlag2;
        public Image personBack;
        public Image personMid;
        public Image personAdvEyes;
        public Image personAdvMouth;
        public Image personAdvHair;
        public Image personAdvJewel;
        public Image personFore;
        public Image personBorder;
        public World world;

        public void setTo(Unit unit)
        {
            if (unit == null) { setToNull(); return; }

            title.text = unit.getName();
            desc.text = unit.getDesc();
            task.text = unit.getTaskShort();
            taskDesc.text = unit.getTaskDesc();

            if (unit.person == null) { clearPerson(); }
            else
            {
                personBack.sprite = unit.person.getImageBack();
                personMid.sprite = unit.person.getImageMid();
                personFore.sprite = unit.person.getImageFore();
                if (World.staticMap.param.option_useAdvancedGraphics == 1)
                {
                    Person p = unit.person;
                    if (p.isMale)
                    {
                        personMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_faces[p.imgAdvFace];
                        personAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_eyes[p.imgAdvEyes];
                        personAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_mouths[p.imgAdvMouth];
                        personAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_hair[p.imgAdvHair];
                        personAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].m_jewels[p.imgAdvJewel];
                    }
                    else
                    {
                        personMid.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_faces[p.imgAdvFace];
                        personAdvEyes.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_eyes[p.imgAdvEyes];
                        personAdvMouth.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_mouths[p.imgAdvMouth];
                        personAdvHair.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_hair[p.imgAdvHair];
                        personAdvJewel.sprite = p.map.world.textureStore.cultures[p.culture.graphicsIndex].f_jewels[p.imgAdvJewel];
                    }
                    personFore.sprite = p.getImageFore();
                }
                else
                {
                    personAdvEyes.sprite = world.textureStore.person_advClear;
                    personAdvMouth.sprite = world.textureStore.person_advClear;
                    personAdvHair.sprite = world.textureStore.person_advClear;
                    personAdvJewel.sprite = world.textureStore.person_advClear;

                }

                personFore.color = Color.white;
            }

            if (unit.isEnthralled())
            {
                if (unit.movesTaken == 0) { hasMoved.text = "Can Move"; }
                else { hasMoved.text = "Has Taken Turn"; }
            }
            else
            {
                hasMoved.text = "Not an Enthralled Agent";
            }

            nationText.text = unit.society.getName();
            if (unit.parentLocation != null)
            {
                nationText.text += "\nFrom " + unit.parentLocation.getName();
            }
            nationFlag1.color = unit.society.color;
            nationFlag2.color = unit.society.color2;


            specialDesc.text = unit.specialInfoLong();
            specialText.color = unit.specialInfoColour();
            specialText.text = unit.specialInfo();
        }

        public void bViewRelationships()
        {
            if (GraphicalMap.selectedSelectable is Unit == false) { return; }
            Unit u = (Unit)GraphicalMap.selectedSelectable;
            if (u.person == null) { return; }
            if (u.person.state == Person.personState.enthralledAgent) { return; }
            if (u.person.state == Person.personState.enthralled) { return; }

            List<RelObj> rels = new List<RelObj>();
            foreach (RelObj rel in u.person.relations.Values)
            {
                if (Math.Abs(rel.getLiking()) > 10)
                {
                    rels.Add(rel);
                }
            }
            rels.Sort(this);

            world.ui.addBlocker(world.prefabStore.getScrollSetRelations(rels).gameObject);
        }

        public void bSetAsHostile()
        {
            if (GraphicalMap.selectedSelectable == null || GraphicalMap.selectedSelectable is Unit == false) { return; }
            Unit u = (Unit)GraphicalMap.selectedSelectable;
            u.flaggedAsEnthralledHostile = !u.flaggedAsEnthralledHostile;
            if (u.flaggedAsEnthralledHostile)
            {
                world.prefabStore.popMsgAgent(u, u, "All your enthralled units will now automatically attack " + u.getName() + " if they enter into the same location.");
            }
            else
            {
                world.prefabStore.popMsgAgent(u, u, "Your enthralled agents will only attack " + u.getName() + " if " + u.getName() + " is hostile to them.");
            }
            GraphicalMap.checkData();
        }
        public void clearPerson()
        {
            personBack.sprite = world.textureStore.icon_mask;
            personFore.sprite = null;
            personMid.sprite = null;
            personFore.color = new Color(0, 0.1f, 0);
        }

        public void setToNull()
        {
            title.text = "No Unit Selected";
            desc.text = "";
            clearPerson();
            hasMoved.text = "";
            taskDesc.text = "";
            evidenceText.text = "";
            specialDesc.text = "No unit selected";
            specialDesc.color = Color.white;
            specialText.text = "";
        }

        public int Compare(RelObj x, RelObj y)
        {
            double a = Math.Abs(x.getLiking());
            double b = Math.Abs(y.getLiking());
            return Math.Sign(b - a);
        }
    }
}
