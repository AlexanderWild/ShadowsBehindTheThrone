using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupWorkshopItem : MonoBehaviour
    {
		public UIMaster ui;
		public Text title;
		public Text description;
		public Text status;

		public Button bDismiss;
		public Button bPublish;
		public Button bUpdate;

		string path;
		ModData data;

		public void populate(ModData d, string p, bool owned)
		{
			data = d;
			path = p;

			title.text = data.title;
			description.text = data.description;

			if (!owned)
			{
				bPublish.gameObject.SetActive(false);
				bUpdate.gameObject.SetActive(false);
			}
			else if (SteamManager.hasPublishedWorkshopItem(path))
			{
				bPublish.gameObject.SetActive(false);
				bUpdate.gameObject.SetActive(true);
			}
			else
			{
				bPublish.gameObject.SetActive(true);
				bUpdate.gameObject.SetActive(false);
			}
		}

		public void dismiss()
        {
            ui.world.audioStore.playClickInfo();
            ui.removeBlocker(this.gameObject);
        }

		public void publish()
		{
			status.gameObject.SetActive(true);
			status.text = "Loading...";

			bPublish.gameObject.SetActive(false);
			SteamManager.createWorkshopItem(data, path, res => {
				if (res)
					status.text = "Success!";
				else
					status.text = "Error!";
			});
		}

		public void update()
		{
			status.gameObject.SetActive(true);
			status.text = "Loading...";

			bUpdate.gameObject.SetActive(false);
			SteamManager.updateWorkshopItem(data, path, res => {
				if (res)
					status.text = "Success!";
				else
					status.text = "Error!";
			});
		}
	}
}
