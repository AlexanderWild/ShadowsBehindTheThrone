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
        public SelectClickReceiver clickReceiver;

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

        public void setTo(string text)
        {
            if (text[0] == '#')
            {
                float r = 1;
                float g = 1;
                float b = 1;
                float a = 1;
                if (text.Substring(0,4) == "#GLD")
                {
                    r = 1;
                    g = 1;
                    b = 0;
                }
                if (text.Substring(0, 4) == "#GRN")
                {
                    r = 0;
                    g = 0.75f;
                    b = 0;
                }
                if (text.Substring(0, 4) == "#RED")
                {
                    r = 0.75f;
                    g = 0;
                    b = 0;
                }
                if (text[4] == 'T') { a = 0.65f; }
                background.color = new Color(1, 1, 1, a);
                text = text.Substring(5);

                title.color = new Color(r, g, b, a);
            }
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
            map.world.audioStore.playActivate();
            if (clickReceiver != null)
            {
                clickReceiver.selectableClicked(title.text);
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
            return true;
        }
    }
}
