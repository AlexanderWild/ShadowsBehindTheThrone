using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupMsgHint: MonoBehaviour
    {
        public Text text;
        public Text title;
        public Button bDismiss;
        public UIMaster ui;

        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
