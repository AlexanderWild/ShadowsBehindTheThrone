using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupTutorialMsg: MonoBehaviour
    {
        public Image img;
        public Text text;
        public Button bDismiss;
        public UIMaster ui;

        public void setTo(int i)
        {
            if (i == 0)
            {
                img.sprite = ui.world.textureStore.tutorial1;
            }
            if (i == 1)
            {
                img.sprite = ui.world.textureStore.tutorial2;
            }
            if (i == 2)
            {
                img.sprite = ui.world.textureStore.tutorial3;
            }
            if (i == 3)
            {
                img.sprite = ui.world.textureStore.tutorial4;
            }
            if (i == 4)
            {
                img.sprite = ui.world.textureStore.tutorial5;
            }
        }

        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
