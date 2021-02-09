using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxSavegame : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public GameObject mover;
        public float targetY;
        public bool usable = true;
        public Image background;
        public FileInfo option;

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

        public void setTo(FileInfo info,string versionID)
        {
            option = info;
            title.text = info.Name;
            if (versionID != null)
            {
                title.text += "\n[This saved game is from a different version, and may not load correctly. (" + versionID + ")]";
                background.color = new Color(1, 1, 1, 0.5f);
            }
            body.text = "Saved on: " + info.LastWriteTime;
        }

        public float ySize()
        {
            return 100;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            World.self.audioStore.playActivate();
            World.self.load(option.Name);
            World.staticMap.world.ui.setToWorld();
        }

        public string getTitle()
        {
            return option.Name;
        }

        public string getBody()
        {
            string reply = "Saved on: " + option.LastWriteTime;

            return reply;
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
