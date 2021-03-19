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

		List<EventData.Outcome> outcomes = new List<EventData.Outcome>();

		public void populate(EventData data)
		{
			title.text = data.name;
			description.text = data.description;

			int n = 0;
			foreach (var o in data.outcomes)
			{
				options[n].GetComponentInChildren<Text>().text = o.name;
				options[n].gameObject.SetActive(true);
				options[n].onClick.AddListener(delegate { dismiss(o.description); });

				outcomes.Add(o);
				n += 1;
			}
		}

        public void dismiss(string choice)
        {
            ui.removeBlocker(this.gameObject);
			ui.world.prefabStore.popMsg(choice, true);
        }
    }
}
