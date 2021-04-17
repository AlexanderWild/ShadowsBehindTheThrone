using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupIOOptions: MonoBehaviour
    {
        public static bool hasLoadedOpts = false;
        public Map map;
        public Button bDismiss;
        public Button bKeybinds;
        public Button bEdgeScroll;
        public Button bSoundEffects;
        public Button bAdvancedGraphics;
        public Button bUseEvents;
        public UIMaster ui;
        public Text mouseEdgeText;
        public Text soundEffectText;
        public Text autosaveText;
        public Text advancedGraphicsText;
        public Text useEventsText;
        public Text currentSaveLocation;
        public InputField customSaveLocation;
        public Text musicVolumeText;
        public GameObject musicBlock;
        public Text seedString;

        public void Update()
        {
            mouseEdgeText.text = map.param.option_edgeScroll == 1 ? "On" : "Off";
            soundEffectText.text = ui.world.audioStore.effectVolume == 0 ? "Off" : "On";
            autosaveText.text = World.autosavePeriod == -1 ? "Off" : "On";
            musicBlock.SetActive(World.advancedEdition && ui.uiMusic.loadedMusic.Count > 0);
            if (World.advancedEdition == false)
            {
                bAdvancedGraphics.gameObject.SetActive(false);
                advancedGraphicsText.text = "";
            }
            else
            {
                advancedGraphicsText.text = map.param.option_useAdvancedGraphics == 1 ? "On" : "Off";
                useEventsText.text = World.useEvents ? "On" : "Off";

            }
            currentSaveLocation.text = "Currently Saving To: " + World.saveFolder;
            seedString.text = "Game Seed: " + map.seed;
            musicVolumeText.text = World.musicVolume + "%";
        }

        public void toggleEdgeScroll()
        {
            ui.world.audioStore.playClickInfo();
            map.param.option_edgeScroll = map.param.option_edgeScroll == 1 ? 0 : 1;
        }
        public void toggleSoundEffects()
        {
            ui.world.audioStore.effectVolume = (ui.world.audioStore.effectVolume == 0) ? 1 : 0;
            ui.world.audioStore.playClickInfo();
        }
        public void toggleAdvancedGraphics()
        {
            if (ui.world.map.param.option_useAdvancedGraphics == 0 && World.advancedEdition)
            {
                ui.world.map.param.option_useAdvancedGraphics = 1;
            }
            else
            {
                ui.world.map.param.option_useAdvancedGraphics = 0;
            }
            ui.world.audioStore.playClickInfo();
        }
        public void toggleCyclopsGraphics()
        {
            ui.world.map.cyclopsGraphics = !ui.world.map.cyclopsGraphics;
        }
        public void toggleUseEvents()
        {
            World.useEvents = !World.useEvents;
        }

        public static void loadEarly()
        {
            try
            {
                if (File.Exists(World.saveFolder + "settings.txt"))
                {
                    string[] data = File.ReadAllLines(World.saveFolder + "settings.txt");

                    if (data.Length > 2 && !String.IsNullOrWhiteSpace(data[2]))
                    {
                        World.saveFolder = data[2];
                    }
                }
            }
            catch (Exception e)
            {
                //
            }
        }

        public static void load(Map map)
        {
            try
            {
                if (File.Exists(World.saveFolder + "settings.txt"))
                {
                    string[] data = File.ReadAllLines(World.saveFolder + "settings.txt");

                    string[] split = data[0].Split(',');
                    map.param.option_edgeScroll = int.Parse(split[0]);
                    map.world.audioStore.effectVolume = int.Parse(split[1]);
                    World.autosavePeriod = int.Parse(split[2]);
                    World.autodismissAutosave = int.Parse(split[3]);
                    map.param.option_useAdvancedGraphics = int.Parse(split[4]);
                    World.musicVolume = int.Parse(split[5]);
                    World.useEvents = int.Parse(split[6])==1?true:false;

                    if (data.Length > 1 && !String.IsNullOrWhiteSpace(data[1]))
                    {
                        UIKeybinds.loadFromString(data[1]);
                    }
                }
            }catch(Exception e)
            {
                //
            }
            hasLoadedOpts = true;
        }

        public void inputFieldEditEnd()
        {
            World.log("Trying to write " + customSaveLocation.text + " as custom save location");
            if (Directory.Exists(customSaveLocation.text))
            {
                World.log("Success on previous");
                World.saveFolder = customSaveLocation.text + World.separator;
            }
        }
        public void saveToDefault()
        {
            
            World.saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + World.separator + World.saveFolderName + World.separator;
            customSaveLocation.text = World.saveFolder;
        }

        public void bMusicMin()
        {
            World.musicVolume = 0;
        }
        public void bMusicLessLess()
        {
            World.musicVolume -= 10;
            if (World.musicVolume < 0) { World.musicVolume = 0; }
            if (World.musicVolume > 100) { World.musicVolume = 100; }
        }
        public void bMusicLess()
        {
            World.musicVolume -= 1;
            if (World.musicVolume < 0) { World.musicVolume = 0; }
            if (World.musicVolume > 100) { World.musicVolume = 100; }
        }
        public void bMusicMore()
        {
            World.musicVolume += 1;
            if (World.musicVolume < 0) { World.musicVolume = 0; }
            if (World.musicVolume > 100) { World.musicVolume = 100; }
        }
        public void bMusicMoreMore()
        {
            World.musicVolume += 10;
            if (World.musicVolume < 0) { World.musicVolume = 0; }
            if (World.musicVolume > 100) { World.musicVolume = 100; }
        }
        public void bMusicMax()
        {
            World.musicVolume = 100;
        }
        public void dismiss()
        {
            ui.world.audioStore.playClickInfo();

            saveState();

            ui.removeBlocker(this.gameObject);
        }

        public static void saveState()
        {
            if (World.checkSaveFolder() == false) {
                World.staticMap.world.prefabStore.popMsg("Unable to write to directory " + World.saveFolder + ". Settings will not persist without folder access.",true);
            }

            string stateStr = World.staticMap.param.option_edgeScroll + "," + World.staticMap.world.audioStore.effectVolume + "," + World.autosavePeriod 
                + "," + World.autodismissAutosave + "," + World.staticMap.param.option_useAdvancedGraphics + "," + World.musicVolume + ","
                + (World.useEvents?"1":"0");
            stateStr += Environment.NewLine + UIKeybinds.saveToString();
            stateStr += Environment.NewLine + World.saveFolder;

            if (File.Exists(World.saveFolder + "settings.txt"))
            {
                File.Delete(World.saveFolder + "settings.txt");
            }
            File.WriteAllText(World.saveFolder + "settings.txt", stateStr);
        }
        public void autosaveToggle()
        {
            World.autosavePeriod = World.autosavePeriod == -1 ? 10 : -1;
        }
    }
}
