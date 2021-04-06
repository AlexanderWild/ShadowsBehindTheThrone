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
		public Text descriptionH;
        public Text descriptionP;
        public Button[] options;
        public UIMaster ui;
        public GameObject optDescBack;
        public Text optDesc;
        public Image imgH;
        public Image imgP;
        public Text imgCredit;
        public Text modCredit;

        public string[] optDescs = new string[]
        {
            "",
            "",
            "",
            ""
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
                        if (optDescs[i] != null && optDescs[i].Length > 1)
                        {
                            optDescBack.gameObject.SetActive(true);
                            optDesc.text = optDescs[i];
                        }
                    }
                }
            }
        }
        public void populate(EventData data, EventContext ctx,Map map)
		{
			title.text = bindReferences(data.name,ctx,map);
			descriptionH.text = bindReferences(data.description,ctx,map);
            descriptionP.text = bindReferences(data.description,ctx,map);

            imgCredit.text = data.imgCredit;
            modCredit.text = data.modCredit;

            Sprite image = null;
            string imgKey = "default.01.jpg";
            if (data.image != null && data.image.Length > 1)
            {
                imgKey = data.image;
            }
            if (EventManager.loadedModImages.ContainsKey(imgKey)){
                image = EventManager.loadedModImages[imgKey];
                World.log("Narr event dims " + image.rect.width + "," + image.rect.height);
                if (image.rect.width > image.rect.height * 1.3)
                {
                    //Horizontal mode
                    descriptionP.gameObject.SetActive(false);
                    descriptionH.gameObject.SetActive(true);
                    imgH.sprite = image;
                    imgP.gameObject.SetActive(false);
                    imgH.gameObject.SetActive(true);
                    World.log("Narr event landscape mode");
                }
                else
                {
                    //Portrait mode
                    descriptionH.gameObject.SetActive(false);
                    descriptionP.gameObject.SetActive(true);
                    imgP.sprite = image;
                    imgH.gameObject.SetActive(false);
                    imgP.gameObject.SetActive(true);
                    World.log("Narr event portrait mode");
                }
            }
            else
            {
                World.self.prefabStore.popMsg("Unable to find image referenced by event. Name: " + imgKey);
            }

            //Set everything past the current choices to inactive
            for (int i = data.choices.Count; i < options.Length; i++)
            {
                options[i].gameObject.SetActive(false);
            }

			int n = 0;
			foreach (var c in data.choices)
			{
				options[n].GetComponentInChildren<Text>().text = c.name;
				options[n].onClick.AddListener(delegate { dismiss(c, ctx); });
                if (c.description != null)
                {
                    optDescs[n] = bindReferences(c.description, ctx, map);
                }
                else
                {
                    optDescs[n] = null;
                }

                options[n++].gameObject.SetActive(true);
			}
		}

        private string bindReferences(string description, EventContext ctx, Map map)
        {
            string updated = description;
            if (map.overmind.enthralled != null){updated = updated.Replace("%ENTHRALLED_NAME", map.overmind.enthralled.getFullName()); }
            if (ctx.person != null)
            {
                updated = updated.Replace("%PERSON_NAME", ctx.person.getFullName());
                string sov = "nobody";
                if (ctx.society.sovereign.heldBy != null) { }
                updated = updated.Replace("%SOVEREIGN", sov);
                if (ctx.person.isMale)
                {
                    updated = updated.Replace("%she", "he");
                    updated = updated.Replace("%She", "He");
                    updated = updated.Replace("%Her", "His");
                    updated = updated.Replace("%her", "his");
                    updated = updated.Replace("%Hers", "His");
                    updated = updated.Replace("%hers", "his");
                    updated = updated.Replace("%he", "he");
                    updated = updated.Replace("%He", "He");
                    updated = updated.Replace("%His", "His");
                    updated = updated.Replace("%his", "his");
                    updated = updated.Replace("%Him", "Him");
                    updated = updated.Replace("%him", "him");
                }
                else
                {
                    updated = updated.Replace("%she", "she");
                    updated = updated.Replace("%She", "She");
                    updated = updated.Replace("%Her", "Her");
                    updated = updated.Replace("%her", "her");
                    updated = updated.Replace("%Hers", "Hers");
                    updated = updated.Replace("%hers", "hers");
                    updated = updated.Replace("%he", "she");
                    updated = updated.Replace("%He", "She");
                    updated = updated.Replace("%His", "Her");
                    updated = updated.Replace("%his", "her");
                    updated = updated.Replace("%Him", "Her");
                    updated = updated.Replace("%him", "her");

                }
            }

            if (ctx.unit != null)
            {
                updated = updated.Replace("%UNIT_NAME", ctx.unit.getName());
            }
            if (ctx.location != null)
            {
                updated = updated.Replace("%LOCATION_NAME", ctx.location.getName(false));
            }
            return updated;
        }

        public void dismiss(EventData.Choice c, EventContext ctx)
        {
			var o = EventManager.chooseOutcome(c, ctx);

            ui.world.audioStore.playClickSelect();

            ui.removeBlocker(this.gameObject);
            if (o.description != null && o.description.Length > 0)
            {
                ui.world.prefabStore.popMsg(bindReferences(o.description,ctx,ui.world.map), true);
            }
        }
    }
}
