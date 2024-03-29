﻿

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;


namespace Assets.Code
{
	//
	// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
	// It handles the basics of starting up and shutting down the SteamAPI for use.
	//
	[DisallowMultipleComponent]
	public class SteamManager : MonoBehaviour
	{
		[Serializable]
		public class WorkshopItemStore
		{
			[Serializable]
			public class PublishedItem
			{
				public string path;
				public PublishedFileId_t fileId;

				public PublishedItem(string p, PublishedFileId_t id)
				{
					path = p;
					fileId = id;
				}
			}

			public List<PublishedItem> items = new List<PublishedItem>();
		}

		public static bool apiShutdown = false;

		public static bool shouldStoreStats = false;
		public static bool s_EverInitialized = false;

		public enum achievement_key { VICTORY ,FIRST_AGENT,ROYAL_BLOOD, FLESH_VICTORY,POLITICS_ONLY,CULT_GROWS, LEARN_THE_TRUTH, MERCANTILISM,DARK_EMPIRE, BROKEN_SOUL,WORLD_UNDER_ICE,
		SAVIOUR,THE_TWINS};
		public static string[] achievementKeys = new string[]
		{
			"VICTORY",
			"FIRST_AGENT",
			"ROYAL_BLOOD",
			"FLESH_VICTORY",
			"POLITICS_ONLY",
			"CULT_GROWS",
			"LEARN_THE_TRUTH",
			"MERCANTILISM",
			"DARK_EMPIRE",
			"BROKEN_SOUL",
			"WORLD_UNDER_ICE",
			"SAVIOUR",
			"THE_TWINS",
		};
		public static bool[] hasAchieved;

		static string publishedItemStoreName = "published.json";
		static Dictionary<string, PublishedFileId_t> publishedItems = null;

		public static SteamManager s_instance;
		protected static SteamManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					return new GameObject("SteamManager").AddComponent<SteamManager>();
				}
				else
				{
					return s_instance;
				}
			}
		}

		protected bool m_bInitialized = false;
		public static bool Initialized
		{
			get
			{
				return Instance.m_bInitialized;
			}
		}

		protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

		[AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
		protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
		{
			Debug.LogWarning(pchDebugText);
		}

		// In case of disabled Domain Reload, reset static members before entering Play Mode.
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void InitOnPlayMode()
		{
			s_EverInitialized = false;
			s_instance = null;
		}

		protected virtual void Awake()
		{
			// Only one instance of SteamManager at a time!
			if (s_instance != null)
			{
				Destroy(gameObject);
				return;
			}
			s_instance = this;

			if (s_EverInitialized)
			{
				// This is almost always an error.
				// The most common case where this happens is when SteamManager gets destroyed because of Application.Quit(),
				// and then some Steamworks code in some other OnDestroy gets called afterwards, creating a new SteamManager.
				// You should never call Steamworks functions in OnDestroy, always prefer OnDisable if possible.
				throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");
			}

			// We want our SteamManager Instance to persist across scenes.
			DontDestroyOnLoad(gameObject);

			if (!Packsize.Test())
			{
				Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
			}

			if (!DllCheck.Test())
			{
				Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
			}

			//Steam API must only run in compiled mode, or it doesn't close properly, causing all manner of issues
			if (Application.isEditor)
            {
				return;
            }

			try
			{
				// If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
				// Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

				// Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
				// remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
				// See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
				if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
				{
					Application.Quit();
					return;
				}
			}
			catch (System.DllNotFoundException e)
			{ // We catch this exception here, as it will be the first occurrence of it.
				Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

				Application.Quit();
				return;
			}

			// Initializes the Steamworks API.
			// If this returns false then this indicates one of the following conditions:
			// [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
			// [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
			// [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
			// [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
			// [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
			// Valve's documentation for this is located here:
			// https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
			m_bInitialized = SteamAPI.Init();
			if (!m_bInitialized)
			{
				Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
				return;
			}

			s_EverInitialized = true;
			World.log("Steamworks Awake method returned successfully");

			hasAchieved = new bool[achievementKeys.Length];
		}

		// This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
		protected virtual void OnEnable()
		{
			if (s_instance == null)
			{
				s_instance = this;
			}

			if (!m_bInitialized)
			{
				return;
			}

			if (m_SteamAPIWarningMessageHook == null)
			{
				// Set up our callback to receive warning messages from Steam.
				// You must launch with "-debug_steamapi" in the launch args to receive warnings.
				m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
				SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
			}

			//// Run Steam client callbacks
			SteamAPI.RunCallbacks();
		}

		// OnApplicationQuit gets called too early to shutdown the SteamAPI.
		// Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
		// Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
		protected virtual void OnDestroy()
		{
			if (s_instance != this)
			{
				return;
			}

			s_instance = null;

			if (!m_bInitialized)
			{
				return;
			}

			shutdownSteamAPI();
		}

		public static void unlockAchievement(achievement_key key)
        {
			if (World.staticMap == null || World.staticMap.automatic) { return; }//Can't unlock achievements in automatic play mode
			if (!apiShutdown && s_EverInitialized)
			{
				if (hasAchieved[(int)key]) { return; }
				hasAchieved[(int)key] = true;
				SteamUserStats.SetAchievement(achievementKeys[(int)key]);
				shouldStoreStats = true;
			}
		}
		public static void reset_all_achievements()
		{
			if (!apiShutdown && s_EverInitialized)
			{
				SteamUserStats.ResetAllStats(true);
			}
		}

		public static void shutdownSteamAPI()
		{
			if (!apiShutdown)
			{
				apiShutdown = true;
				SteamAPI.Shutdown();
				World.log("Steam shutdown call issued");
			}
		}

		public static List<string> getPublishedWorkshopItems()
		{
			return initPublishedWorkshopItems().Keys.ToList();
		}

		public static bool hasPublishedWorkshopItem(string path)
		{
			return initPublishedWorkshopItems().ContainsKey(path);
		}

		static Dictionary<string, PublishedFileId_t> initPublishedWorkshopItems()
		{
			if (publishedItems != null)
				return publishedItems;

			publishedItems = new Dictionary<string, PublishedFileId_t>();

			string path = World.userModFolder + World.separator + publishedItemStoreName;
			if (!File.Exists(path))
				return publishedItems;

			string data = File.ReadAllText(path);
			WorkshopItemStore store = JsonUtility.FromJson<WorkshopItemStore>(data);

			publishedItems = new Dictionary<string, PublishedFileId_t>();
			foreach (var item in store.items)
				publishedItems.Add(item.path, item.fileId);

			return publishedItems;
		}

		static void updatePublishedWorkshopItems()
		{
			WorkshopItemStore store = new WorkshopItemStore();
			foreach (var kv in publishedItems)
				store.items.Add(new WorkshopItemStore.PublishedItem(kv.Key, kv.Value));

			string data = JsonUtility.ToJson(store);
			string path = World.userModFolder + World.separator + publishedItemStoreName;

			File.WriteAllText(path, data);
		}

		public static List<string> getSubscribedWorkshopItems()
		{
			var count = SteamUGC.GetNumSubscribedItems();

			var subbed = new PublishedFileId_t[count];
			SteamUGC.GetSubscribedItems(subbed, count);

			var res = new List<string>();
			foreach (var id in subbed)
			{
				var state = (EItemState)SteamUGC.GetItemState(id);
				if (!state.HasFlag(EItemState.k_EItemStateInstalled))
					continue;

				ulong size;
				string path;
				uint time;

				SteamUGC.GetItemInstallInfo(id, out size, out path, 256, out time);
				res.Add(path);
			}

			return res;
		}

		public static void createWorkshopItem(ModData data, string path, Action<bool> callback)
		{
			var call = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
			
			var result = CallResult<CreateItemResult_t>.Create();
			result.Set(call, (param, error) => {
				if (error || param.m_eResult != EResult.k_EResultOK)
				{
					callback(false);
				}
				else
				{
					initPublishedWorkshopItems().Add(path, param.m_nPublishedFileId);
					updatePublishedWorkshopItems();

					updateWorkshopItem(data, path, callback);
				}
			});
		}

		public static void updateWorkshopItem(ModData data, string path, Action<bool> callback)
		{
			var items = initPublishedWorkshopItems();
			if (!items.ContainsKey(path))
				throw new Exception("item must be published before it is updated.");

			var handle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), items[path]);
			SteamUGC.SetItemTitle(handle, data.title);
			SteamUGC.SetItemDescription(handle, data.description);
			SteamUGC.SetItemTags(handle, data.tags);
			SteamUGC.SetItemPreview(handle, data.previewImage);
			
			SteamUGC.SetItemContent(handle, path);
			SteamUGC.SetItemVisibility(handle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
			var call = SteamUGC.SubmitItemUpdate(handle, "");

			var result = CallResult<SubmitItemUpdateResult_t>.Create();
			result.Set(call, (param, error) => {
				callback(!error && param.m_eResult == EResult.k_EResultOK);
			});
		}

		protected virtual void Update()
		{
			if (!m_bInitialized)
			{
				return;
			}
			if (apiShutdown)
            {
				return;
            }

			SteamAPI.RunCallbacks();
			if (shouldStoreStats)
			{
				shouldStoreStats = false;
				SteamUserStats.StoreStats();
			}
		}
	}
}