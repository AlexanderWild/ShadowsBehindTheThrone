using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupEvent : MonoBehaviour
    {
		public Text title;
		public Text description;
		public Button[] options;
        public UIMaster ui;

		public void populate(EventData data, EventContext ctx)
		{
			title.text = data.name;
			description.text = data.description;

			int n = 0;
			foreach (var o in data.outcomes)
			{
				if (n > 2)
					break;

				options[n].GetComponentInChildren<Text>().text = o.name;
				options[n].onClick.AddListener(delegate { dismiss(o, ctx); });

				options[n++].gameObject.SetActive(true);
			}
		}

        public void dismiss(EventData.Outcome o, EventContext ctx)
        {
			foreach (var e in o.effects)
				EventRuntime.evaluate(e, ctx);

            ui.removeBlocker(this.gameObject);
			ui.world.prefabStore.popMsg(o.description, true);
        }
    }
}
