using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class UIKeybinds
	{
		public enum Action
		{
			PAN_UP,
			PAN_DOWN,
			PAN_LEFT,
			PAN_RIGHT,
			ZOOM_IN,
			ZOOM_OUT,

			MASK_NATION,
			MASK_PROVINCE,
			MASK_VOTE_EFFECT,
			MASK_INFILTRATION,
			MASK_MY_LIKING,
			MASK_THEIR_LIKING,
			MASK_AWARENESS,
			MASK_MY_SUSPICION,
			MASK_THEIR_SUSPICION,
			MASK_CLEAR_MASKS
		};

		public static Dictionary<Action, KeyCode> mappings = new Dictionary<Action, KeyCode>
		{
			{ Action.PAN_UP,    KeyCode.W },
			{ Action.PAN_DOWN,  KeyCode.S },
			{ Action.PAN_LEFT,  KeyCode.A },
			{ Action.PAN_RIGHT, KeyCode.D },
			{ Action.ZOOM_IN,   KeyCode.Z },
			{ Action.ZOOM_OUT,  KeyCode.X },

			{ Action.MASK_NATION,          KeyCode.Alpha1 },
			{ Action.MASK_PROVINCE,        KeyCode.Alpha2 },
			{ Action.MASK_VOTE_EFFECT,     KeyCode.Alpha3 },
			{ Action.MASK_INFILTRATION,    KeyCode.Alpha4 },
			{ Action.MASK_MY_LIKING,       KeyCode.Alpha5 },
			{ Action.MASK_THEIR_LIKING,    KeyCode.Alpha6 },
			{ Action.MASK_AWARENESS,       KeyCode.Alpha7 },
			{ Action.MASK_MY_SUSPICION,    KeyCode.Alpha8 },
			{ Action.MASK_THEIR_SUSPICION, KeyCode.Alpha9 },
			{ Action.MASK_CLEAR_MASKS,     KeyCode.Alpha0 },
		};

		public static bool getKey(Action a, bool continuous = false)
		{
			if (continuous)
				return Input.GetKey(mappings[a]);
			else
				return Input.GetKeyDown(mappings[a]);
		}
	}
}
