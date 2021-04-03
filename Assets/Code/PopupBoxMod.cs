using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxMod : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public GameObject mover;
        public float targetY;
		public Image background;
        public bool usable = true;

		string path;
		ModData data;
		bool owned;

        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        public void setTo(ModData d, string p, bool mine)
		{
			path = p;
			data = d;
			owned = mine;

			title.text = data.title;
			if (owned)
				body.text = path;
			else
				body.text = "Subscribed on the Steam Workshop";
		}

        public float ySize()
        {
            return 100;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
			World.self.audioStore.playClickInfo();
			World.self.prefabStore.popWorkshopItem(data, path, owned);
        }

        public string getTitle()
        {
            return data.title;
        }

        public string getBody()
        {
            return data.description;
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return usable;
        }
    }
}
