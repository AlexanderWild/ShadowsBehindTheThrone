using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupKeybinds: MonoBehaviour
    {
		public UIMaster ui;
		public Button bDismiss;

		public Button bPanUp;
		public Button bPanDown;
		public Button bPanLeft;
		public Button bPanRight;
		public Button bZoomIn;
		public Button bZoomOut;

		public Button bNation;
		public Button bProvince;
		public Button bVoteEffect;
		public Button bInfiltration;
		public Button bMyLiking;
		public Button bTheirLiking;
		public Button bAwareness;
		public Button bMySuspicion;
		public Button bTheirSuspicion;
		public Button bClearMasks;

		Button activeButton = null;
		UIKeybinds.Action activeAction;

		public void Start()
		{
			updateButton(UIKeybinds.Action.PAN_UP);
			updateButton(UIKeybinds.Action.PAN_DOWN);
			updateButton(UIKeybinds.Action.PAN_LEFT);
			updateButton(UIKeybinds.Action.PAN_RIGHT);
			updateButton(UIKeybinds.Action.ZOOM_IN);
			updateButton(UIKeybinds.Action.ZOOM_OUT);

			updateButton(UIKeybinds.Action.MASK_NATION);
			updateButton(UIKeybinds.Action.MASK_PROVINCE);
			updateButton(UIKeybinds.Action.MASK_VOTE_EFFECT);
			updateButton(UIKeybinds.Action.MASK_INFILTRATION);
			updateButton(UIKeybinds.Action.MASK_MY_LIKING);
			updateButton(UIKeybinds.Action.MASK_THEIR_LIKING);
			updateButton(UIKeybinds.Action.MASK_AWARENESS);
			updateButton(UIKeybinds.Action.MASK_MY_SUSPICION);
			updateButton(UIKeybinds.Action.MASK_THEIR_SUSPICION);
			updateButton(UIKeybinds.Action.MASK_CLEAR_MASKS);
		}

		public void OnGUI()
		{
			if (activeButton == null)
				return;

			Event keyEvent = Event.current;
			if (!keyEvent.isKey)
				return;

			switch (keyEvent.keyCode)
			{
				case KeyCode.LeftControl:
				case KeyCode.LeftAlt:
				case KeyCode.Backspace:
				case KeyCode.LeftShift:
				case KeyCode.PageDown:
					return;

				case KeyCode.Escape:
					updateButton(activeAction);
					activeButton = null;
					return;

				default:
					UIKeybinds.mappings[activeAction] = keyEvent.keyCode;
					updateButton(activeAction);

					activeButton = null;
					return;
			}
		}

		public void dismiss()
        {
            ui.world.audioStore.playClickInfo();

            //saveState();

			ui.uiInputs.disable = false;
            ui.removeBlocker(this.gameObject);
        }

		public void onPanUp() { beginRebind(UIKeybinds.Action.PAN_UP); }
		public void onPanDown() { beginRebind(UIKeybinds.Action.PAN_DOWN); }
		public void onPanLeft() { beginRebind(UIKeybinds.Action.PAN_LEFT); }
		public void onPanRight() { beginRebind(UIKeybinds.Action.PAN_RIGHT); }
		public void onZoomIn() { beginRebind(UIKeybinds.Action.ZOOM_IN); }
		public void onZoomOut() { beginRebind(UIKeybinds.Action.ZOOM_OUT); }

		public void onNation() { beginRebind(UIKeybinds.Action.MASK_NATION); }
		public void onProvince() { beginRebind(UIKeybinds.Action.MASK_PROVINCE); }
		public void onVoteEffect() { beginRebind(UIKeybinds.Action.MASK_VOTE_EFFECT); }
		public void onInfiltration() { beginRebind(UIKeybinds.Action.MASK_INFILTRATION); }
		public void onMyLiking() { beginRebind(UIKeybinds.Action.MASK_MY_LIKING); }
		public void onTheirLiking() { beginRebind(UIKeybinds.Action.MASK_THEIR_LIKING); }
		public void onAwareness() { beginRebind(UIKeybinds.Action.MASK_AWARENESS); }
		public void onMySuspicion() { beginRebind(UIKeybinds.Action.MASK_MY_SUSPICION); }
		public void onTheirSuspicion() { beginRebind(UIKeybinds.Action.MASK_THEIR_SUSPICION); }
		public void onClearMasks() { beginRebind(UIKeybinds.Action.MASK_CLEAR_MASKS); }

		void beginRebind(UIKeybinds.Action a)
		{
			if (activeButton)
				return;

			Button b = buttonForAction(a);
			b.GetComponentInChildren<Text>().text = "...";

			activeButton = b;
			activeAction = a;
		}

		void updateButton(UIKeybinds.Action a)
		{
			Button b = buttonForAction(a);
			b.GetComponentInChildren<Text>().text = UIKeybinds.mappings[a].ToString();
		}

		Button buttonForAction(UIKeybinds.Action a)
		{
			switch (a)
			{
				case UIKeybinds.Action.PAN_UP: return bPanUp;
				case UIKeybinds.Action.PAN_DOWN: return bPanDown;
				case UIKeybinds.Action.PAN_LEFT: return bPanLeft;
				case UIKeybinds.Action.PAN_RIGHT: return bPanRight;
				case UIKeybinds.Action.ZOOM_IN: return bZoomIn;
				case UIKeybinds.Action.ZOOM_OUT: return bZoomOut;

				case UIKeybinds.Action.MASK_NATION: return bNation;
				case UIKeybinds.Action.MASK_PROVINCE: return bProvince;
				case UIKeybinds.Action.MASK_VOTE_EFFECT: return bVoteEffect;
				case UIKeybinds.Action.MASK_INFILTRATION: return bInfiltration;
				case UIKeybinds.Action.MASK_MY_LIKING: return bMyLiking;
				case UIKeybinds.Action.MASK_THEIR_LIKING: return bTheirLiking;
				case UIKeybinds.Action.MASK_AWARENESS: return bAwareness;
				case UIKeybinds.Action.MASK_MY_SUSPICION: return bMySuspicion;
				case UIKeybinds.Action.MASK_THEIR_SUSPICION: return bTheirSuspicion;
				case UIKeybinds.Action.MASK_CLEAR_MASKS: return bClearMasks;

				default: return null;
			}
		}
	}
}