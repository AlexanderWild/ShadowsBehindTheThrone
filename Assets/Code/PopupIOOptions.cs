using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupIOOptions: MonoBehaviour
    {
        public Map map;
        public Button bDismiss;
        public Button bEdgeScroll;
        public UIMaster ui;
        public Text mouseEdgeText;

        public void Update()
        {
            mouseEdgeText.text = map.param.option_edgeScroll == 1 ? "On" : "Off";
        }

        public void toggleEdgeScroll()
        {
            map.param.option_edgeScroll = map.param.option_edgeScroll == 1 ? 0 : 1;
        }
        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }
    }
}
