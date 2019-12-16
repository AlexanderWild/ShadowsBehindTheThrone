using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupImgMsg: MonoBehaviour
    {
        public Text textBody;
        public Text textFlavour;
        public Button bDismiss;
        public Image img;
        public UIMaster ui;


        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
