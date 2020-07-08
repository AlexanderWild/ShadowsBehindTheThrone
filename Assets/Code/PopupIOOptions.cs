﻿using System;
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
        public UIMaster ui;
        public Text mouseEdgeText;
        public Text soundEffectText;
        public Text autosaveText;

        public void Update()
        {
            mouseEdgeText.text = map.param.option_edgeScroll == 1 ? "On" : "Off";
            soundEffectText.text = ui.world.audioStore.effectVolume == 0 ? "Off" : "On";
            autosaveText.text = World.autosavePeriod == -1 ? "Off" : "On";
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
                }
            }catch(Exception e)
            {

            }
        }

        public void dismiss()
        {
            ui.world.audioStore.playClickInfo();

            string stateStr = map.param.option_edgeScroll + "," + ui.world.audioStore.effectVolume + "," + World.autosavePeriod;
            if (File.Exists("settings.txt"))
            {
                File.Delete("settings.txt");
            }
            File.WriteAllText("settings.txt", stateStr);

            ui.removeBlocker(this.gameObject);
        }
        public void autosaveToggle()
        {
            World.autosavePeriod = World.autosavePeriod == -1 ? 10 : -1;
        }
    }
}
