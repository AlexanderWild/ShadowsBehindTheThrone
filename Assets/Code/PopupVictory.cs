using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupVictory: MonoBehaviour
    {
        public Button bDismiss;
        public UIMaster ui;


        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);

            ui.addBlocker(ui.world.prefabStore.getPlayback(ui.world,ui.world.map).gameObject);
        }
    }
}
