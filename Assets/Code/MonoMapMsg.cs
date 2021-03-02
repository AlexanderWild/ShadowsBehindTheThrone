using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class MonoMapMsg : MonoBehaviour
    {
        public Text info;
        public Button button;

        public Hex targetHex;
        public Image background;
        
        public void onClick()
        {
            if (targetHex != null)
            {
                GraphicalMap.panTo(targetHex.x, targetHex.y);
            }
        }

        public void SetInfo(MsgEvent v)
        {
            info.text = v.msg;
            targetHex = v.hex;

            int priority = (int)(v.priority);
            if (priority == MsgEvent.LEVEL_BLUE)
            {
                background.color = new Color(0, 0, 0.7f);
                info.color = Color.white;
            }
            if (v.beneficial)
            {
                if (priority == MsgEvent.LEVEL_GREEN)
                {
                    background.color = new Color(0, 0.8f, 0);

                }
                else if (priority == MsgEvent.LEVEL_DARK_GREEN)
                {
                    background.color = new Color(0, 0.6f, 0);

                }
                else if (priority == MsgEvent.LEVEL_DARK_GREEN2)
                {
                    background.color = new Color(0, 0.3f, 0);

                }
            }
            else
            {
                if (priority == MsgEvent.LEVEL_RED)
                {
                    background.color = new Color(0.8f, 0, 0);
                }
                else if (priority == MsgEvent.LEVEL_ORANGE)
                {
                    background.color = new Color(0.8f, 0.5f, 0);
                }
                else if (priority == MsgEvent.LEVEL_YELLOW)
                {
                    background.color = new Color(0.8f, 0.8f, 0);
                }
            }
            if (priority == MsgEvent.LEVEL_GRAY)
            {
                background.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
