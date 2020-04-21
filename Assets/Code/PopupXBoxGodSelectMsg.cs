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
        public Text textCredits;
        public Image img;
        public UIMaster ui;
        public God god;
        public GameObject mover;
        public float targetX;

        public void onClick()
        {
            // sound?
            ui.world.chosenGods.Add(god);

            if (god is God_Omni)
            {
                ui.world.prefabStore.getGameOptionsPopup();
                ui.removeBlocker(this.gameObject);
                return;
            }


            if (ui.world.chosenGods.Count < 2)
            {
                List<God> available = new List<God>();
                foreach (God g2 in ui.world.potentialGods)
                {
                    if (g2 is God_Omni) { continue; }//No double use
                    if (g2 == god) { continue; }
                    available.Add(g2);
                }
                ui.addBlockerDontHide(ui.world.prefabStore.getScrollSetGods(available).gameObject);
                ui.removeBlocker(this.gameObject);
            }
            else
            {
                ui.world.prefabStore.getGameOptionsPopup();
                ui.removeBlocker(this.gameObject);
            }
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

        internal void setTo(World world,God item)
        {
            god = item;
            textTitle.text = item.getName();
            textFlavour.text = item.getDescFlavour();
            textBody.text = item.getDescMechanics();
            img.sprite = item.getGodBackground(world);
            textCredits.text = item.getCredits();

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
