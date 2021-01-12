using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxText : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public GameObject mover;
        public float targetY;
        public bool usable = true;
        public Image background;

        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);

            if (selectable() == false)
            {
                background.color = new Color(1, 1, 1, 0.5f);
            }
        }

        public void setTo(string text)
        {
            title.text = text;
        }

        public float ySize()
        {
            return 80;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            if (map.overmind.enthralled != null)
            {
                map.world.audioStore.playActivate();
            }
        }

        public string getTitle()
        {
            return "";
        }

        public string getBody()
        {
            return title.text;
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            //What an incredible line of code, so maintainable, well freaking done very good
            return title.text.Contains("(WorldGen)") == false;
        }
    }
}
