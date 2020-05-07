using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UILeftUnit : MonoBehaviour
    {
        public Text title;
        public Text desc;
        public Text task;
        public Text taskDesc;
        public Text hasMoved;
        public Text nationText;
        public Text evidenceText;
        public Image nationFlag1;
        public Image nationFlag2;
        public Image personBack;
        public Image personMid;
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
                evidenceText.text = "Evidence: " + (int)(100 * unit.person.evidence) + "%";
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

        }

        public void clearPerson()
        {
            personBack.sprite = world.textureStore.icon_mask;
            personFore.sprite = null;
            personMid.sprite = null;
            evidenceText.text = "No Evidence";
        }

        public void setToNull()
        {
            title.text = "No Unit Selected";
            desc.text = "";
            clearPerson();
            hasMoved.text = "";
            taskDesc.text = "";
            evidenceText.text = "";
        }
    }
}
