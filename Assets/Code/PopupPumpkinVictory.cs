using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupPumpkinVictory: MonoBehaviour
    {
        public Text text;
        public Image horseman;
        public Image pumpkin;
        public Image pumpkinSmile;
        public Button bDismiss;
        public UIMaster ui;
        public float firstTime;
        public bool firstTimeSet = false;

        public void Update()
        {
            if (firstTimeSet == false)
            {
                firstTimeSet = true;
                firstTime = Time.time;
            }
            float colLayer1 = Math.Max(0,1 - ((Time.time - firstTime)*0.3f));
            float colLayer2 = Math.Min(1,Math.Max(0, 1 - ((Time.time - firstTime - 3) * 0.3f)));
            horseman.color = new Color(colLayer1, colLayer1, colLayer1);
            pumpkin.color = new Color(colLayer2, colLayer2, colLayer2);
        }
        
        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
