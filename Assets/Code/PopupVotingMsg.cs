using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupVotingMsg: MonoBehaviour
    {
        public Text title;
        public Text subtitle;
        public Text textBody;
        public Text textFlavour;
        public Button bDismiss;
        public UIMaster ui;


        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
