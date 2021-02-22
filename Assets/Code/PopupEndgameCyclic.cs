using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupEndgameCyclic: MonoBehaviour
    {
        public Button bDismiss;
        public Button bDismissNextAge;
        public UIMaster ui;
        public Image[] images;
        public int index;
        public int prevIndex = 0;
        public float time;
        public int switchCounter = 0;

        public void dismiss()
        {
            ui.world.audioStore.playClick();
            
            ui.removeBlocker(this.gameObject);
            ui.addBlocker(ui.world.prefabStore.getPlayback(ui.world,ui.world.map).gameObject);
        }

        public void dismissToNextAge()
        {
            ui.world.audioStore.playClick();

            World.log("Progressing to next age");

            World.staticMap.overmind.progressToNextAge();

            ui.removeBlocker(this.gameObject);
        }

        public void Update()
        {
            float switchFrames = 30;
            if (time == 0)
            {
                time = Time.time;
            }
            float delta = Time.time - time;
            if (delta > 5)
            {
                switchCounter = (int)switchFrames;
                prevIndex = index;
                index += 1;
                if (index >= images.Length)
                {
                    index = 0;
                }
                time = Time.time;
            }
            foreach (Image img in images)
            {
                img.color = Color.clear;
            }
            if (switchCounter > 0)
            {
                float alpha = 1 - (switchCounter / switchFrames);
                images[prevIndex].color = new Color(1,1,1,1-alpha);
                images[index].color = new Color(1, 1, 1, alpha);
                switchCounter -= 1;
            }
            else
            {
                images[index].color = Color.white;
            }
        }
    }
}
