using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Code
{
    public class PopupSaveDialog: MonoBehaviour
    {
		public Button bSave;
		public Button bCancel;
		public InputField saveName;
        public UIMaster ui;

		public void save()
		{
			String name = World.saveFolder + saveName.text;
			if (name == "")
				name = "quicksave";

			ui.world.audioStore.playClick();
			ui.removeBlocker(this.gameObject);

			ui.world.save(name + ".sv");
		}

		public void cancel()
		{
			ui.world.audioStore.playClick();
			ui.removeBlocker(this.gameObject);
		}
	}
}