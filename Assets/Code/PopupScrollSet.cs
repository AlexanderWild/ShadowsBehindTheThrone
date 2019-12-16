using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupScrollSet : MonoBehaviour
    {
        public Button next;
        public Button prev;
        public Button select;
        public Button cancel;
        public Text title;
        public Text body;
        public UIMaster ui;

        public List<PopupScrollable> scrollables = new List<PopupScrollable>();
        public int index;
        public int setToIndex = -1;

        public void Update()
        {
            if (scrollables.Count == 0) { return; }

            float scaling = (Camera.main.pixelHeight*1f)/600;
            float yStep = scrollables[0].ySize()*scaling;
            float y = yStep * index;
            float ySize = (Camera.main.pixelHeight);

            for (int i = 0; i < scrollables.Count; i++)
            {
                scrollables[i].setTargetY(y+(ySize/2));
                y -= yStep;
            }

            if (setToIndex != index)
            {
                setToIndex = index;
                if (scrollables[index].overwriteSidebar())
                {
                    title.text = scrollables[index].getTitle();
                    body.text = scrollables[index].getBody();
                }
            }
            select.gameObject.SetActive(scrollables[index].selectable());
        }
        public void bSelect()
        {
            if (scrollables.Count > 0)
            {
                scrollables[index].clicked(ui.world.map);
            }
            try
            {
                ui.removeBlocker(this.gameObject);
            }catch(Exception e) { }
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
