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
        public Button bDismissAll;
        public UIMaster ui;

        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
        public void dismissDisableHints()
        {
            ui.world.map.hintSystem.disabled = true;
            ui.removeBlocker(this.gameObject);
        }
    }
}
