using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupGameOptions: MonoBehaviour
    {
        public InputField suspicionGain;
        public InputField powerGain;
        public InputField seedField;
        public Button bDismiss;
        public UIMaster ui;

        public Button bEasy;
        public Button bMedium;
        public Button bHard;

        public int susGainPercent = 100;
        public int powerGainPercent = 100;
        public int currentSeed;

        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
            ui.world.bStartGameSeeded(currentSeed,this);
        }

        public void setEasy()
        {
            susGainPercent = 75;
            powerGainPercent = 150;
            setTextFieldsToCurrentValues();
        }

        public void setMedium()
        {
            susGainPercent = 100;
            powerGainPercent = 100;
            setTextFieldsToCurrentValues();
        }
        public void setHard()
        {
            susGainPercent = 150;
            powerGainPercent = 50;
            setTextFieldsToCurrentValues();
        }
        public void setTextFieldsToCurrentValues()
        {
            World.log("Setting seed " + currentSeed);
            suspicionGain.text = "" + susGainPercent;
            powerGain.text = "" + powerGainPercent;
            seedField.text = "" + currentSeed;
        }
        public void onEditEnd()
        {
            try
            {
                int susGain = int.Parse(suspicionGain.text);
                if (susGain >= 0 && susGain < 1000)
                {
                    susGainPercent = susGain;
                }
            }catch(Exception e){ }
            try
            {
                int val = int.Parse(powerGain.text);
                if (val >= 0 && val < 1000)
                {
                    powerGainPercent = val;
                }
            }
            catch (Exception e) { }
            try
            {
                int val = int.Parse(seedField.text);
                    currentSeed = val;
            }
            catch (Exception e) { }

            //Reset all values. If they're fucked, they're good now
            setTextFieldsToCurrentValues();
        }
    }
}
