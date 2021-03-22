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
			foreach (var c in data.choices)
			{
				if (n > 2)
					break;

				options[n].GetComponentInChildren<Text>().text = c.name;
				options[n].onClick.AddListener(delegate { dismiss(c, ctx); });

				options[n++].gameObject.SetActive(true);
			}
		}

        public void dismiss(EventData.Choice c, EventContext ctx)
        {
			var o = EventManager.chooseOutcome(c, ctx);

            ui.removeBlocker(this.gameObject);
			ui.world.prefabStore.popMsg(o.description, true);
        }
    }
}
