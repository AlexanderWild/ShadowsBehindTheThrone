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
        public Map map;
        public Button bDismiss;
        public Button bEdgeScroll;
        public Button bSoundEffects;
        public Button bAdvancedGraphics;
        public UIMaster ui;
        public Text mouseEdgeText;
        public Text soundEffectText;
        public Text autosaveText;
        public Text advancedGraphicsText;

        public Text seedString;

        public void Update()
        {
            mouseEdgeText.text = map.param.option_edgeScroll == 1 ? "On" : "Off";
            soundEffectText.text = ui.world.audioStore.effectVolume == 0 ? "Off" : "On";
            autosaveText.text = World.autosavePeriod == -1 ? "Off" : "On";
            if (World.advancedEdition == false)
            {
                bAdvancedGraphics.gameObject.SetActive(false);
                advancedGraphicsText.text = "";
            }
            else
            {
                advancedGraphicsText.text = map.param.option_useAdvancedGraphics == 1 ? "On" : "Off";

            }
            seedString.text = "Game Seed: " + map.seed;
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

        public static void load(Map map)
        {
            try
            {
                if (File.Exists("settings.txt"))
                {
                    string data = File.ReadAllText("settings.txt");
                    string[] split = data.Split(',');
                    map.param.option_edgeScroll = int.Parse(split[0]);
                    map.world.audioStore.effectVolume = int.Parse(split[1]);
                    World.autosavePeriod = int.Parse(split[2]);
                    World.autodismissAutosave = int.Parse(split[3]);
                }
            }catch(Exception e)
            {

            }
        }

        public void dismiss()
        {
            ui.world.audioStore.playClickInfo();

            saveState();

            ui.removeBlocker(this.gameObject);
        }

        public static void saveState()
        {
            string stateStr = World.staticMap.param.option_edgeScroll + "," + World.staticMap.world.audioStore.effectVolume + "," + World.autosavePeriod + "," + World.autodismissAutosave;
            if (File.Exists("settings.txt"))
            {
                File.Delete("settings.txt");
            }
            File.WriteAllText("settings.txt", stateStr);
        }
        public void autosaveToggle()
        {
            World.autosavePeriod = World.autosavePeriod == -1 ? 10 : -1;
        }
    }
}
