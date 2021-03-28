using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupEvent : MonoBehaviour
    {
		public Text title;
		public Text description;
		public Button[] options;
        public UIMaster ui;
        public GameObject optDescBack;
        public Text optDesc;

        public string[] optDescs = new string[]
        {
            "Detailed words here",
            "and here",
            "more here",
            "A multiline \nnonsense\nappears"
        };

        public void Update()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            optDescBack.gameObject.SetActive(false);
            optDesc.text = "";
            foreach (RaycastResult result in results)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (result.gameObject.name == options[i].name)
                    {
                        optDescBack.gameObject.SetActive(true);
                        optDesc.text = optDescs[i];
                    }
                }
            }
        }
        public void populate(EventData data, EventContext ctx)
		{
			title.text = data.name;
			description.text = data.description;

            //Set everything past the current choices to inactive
            for (int i = data.choices.Count; i < options.Length; i++)
            {
                options[i].gameObject.SetActive(false);
            }

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
