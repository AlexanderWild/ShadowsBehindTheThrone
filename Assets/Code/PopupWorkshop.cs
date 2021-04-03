using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupWorkshop : MonoBehaviour
    {
		public UIMaster ui;
		public Button bDismiss;

		public void dismiss()
        {
            ui.world.audioStore.playClickInfo();
            ui.removeBlocker(this.gameObject);
        }

		public void showUserMods()
		{
			ui.world.audioStore.playClickInfo();
			ui.world.prefabStore.popScrollSetUserMods();
		}
	}
}
