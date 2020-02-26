using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupXBoxGodSelectMsg: MonoBehaviour, PopupXScrollable
    {
        public Text textTitle;
        public Text textBody;
        public Text textFlavour;
        public Text textAbilities;
        public Image img;
        public UIMaster ui;
        public God god;
        public GameObject mover;
        public float targetX;

        public void onClick()
        {
            ui.world.prefabStore.getGameOptionsPopup();
            ui.world.chosenGod = god;
            ui.removeBlocker(this.gameObject);
        }

        public void Update()
        {
            Vector3 loc = new Vector3(targetX, mover.transform.position.y, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        public string getBody()
        {
            return god.getDescFlavour();
        }

        public string getTitle()
        {
            return god.getName();
        }

        public void setTargetX(float y)
        {
            targetX = y;
        }

        public float xSize()
        {
            return 1366;
        }

        internal void setTo(God item)
        {
            god = item;
            textTitle.text = item.getName();
            textFlavour.text = item.getDescFlavour();
            textBody.text = item.getDescMechanics();

            if (item is God_Omni)
            {
                textAbilities.text = "This dark god has access to all abilities.";
            }
            else
            {
                foreach (Ability a in item.getUniquePowers())
                {
                    textAbilities.text += "-" + a.getName() + "\n";
                }
            }
        }
    }
}
