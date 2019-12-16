using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupXScroll : MonoBehaviour
    {
        public Button next;
        public Button prev;
        public Button cancel;
        public Text body;
        public UIMaster ui;

        public List<PopupXScrollable> scrollables = new List<PopupXScrollable>();
        public int index;
        public int setToIndex = -1;

        public void Update()
        {
            if (scrollables.Count == 0) { return; }
            float scaling = (Camera.main.pixelWidth * 1f) / 1000;

            float xStep = scrollables[0].xSize()*scaling;

            float y = (-xStep * index);
            float xSize = (Camera.main.pixelWidth);

            for (int i = 0; i < scrollables.Count; i++)
            {
                scrollables[i].setTargetX(y+(xSize/2));
                y += xStep;
            }

            if (setToIndex != index)
            {
                setToIndex = index;
                body.text = scrollables[index].getBody();
            }
        }

        public void bNext()
        {
            index += 1;
            if (index >= scrollables.Count)
            {
                index = 0;
            }
        }

        public void bPrev()
        {
            index -= 1;
            if (index < 0)
            {
                index = scrollables.Count - 1;
            }
        }
        public void bCancel()
        {
            ui.removeBlocker(this.gameObject);
        }


    }
}
