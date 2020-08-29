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
        public InputField awarenessGain;
        public InputField investigatorPercentField;
        public InputField seedField;
        public InputField sizeXField;
        public InputField sizeYField;
        public InputField historicalField;
        public Button bDismiss;
        public UIMaster ui;
        

        public Button bEasy;
        public Button bMedium;
        public Button bHard;
        public Toggle tAwareness;
        public Toggle tInvestigatorsSee;
        public Toggle tPaladins;
        public Button bSeedZero;
        public Button bSeedOne;
        public Button bAllOptsOn;

        public bool investigatorsSee = false;
        public bool useAwareness = false;
        public bool usePaladins = false;
        public int susGainPercent = 100;
        public int powerGainPercent = 100;
        public int awarenessGainSpeed = 100;
        public int investigatorCount = 100;
        public int currentSeed;
        public int sizeX = 32;
        public int sizeY = 24;
        public int burnIn = 100;

        public void dismiss()
        {
            useAwareness = tAwareness.isOn;
            investigatorsSee = tInvestigatorsSee.isOn;
            usePaladins = tPaladins.isOn;
            ui.removeBlocker(this.gameObject);
            ui.world.bStartGameSeeded(currentSeed,this);
        }

        public void toggleOption()
        {
            ui.world.audioStore.playClickInfo();
        }

        public void setSeed0()
        {
            currentSeed = 0;
            setTextFieldsToCurrentValues();
        }
        public void setSeed1()
        {
            currentSeed = 1;
            setTextFieldsToCurrentValues();
        }
        public void activateAllGameOpts()
        {
            tAwareness.isOn = true;
            tInvestigatorsSee.isOn = true;
            tPaladins.isOn = true;
        }
        public void setEasy()
        {
            ui.world.audioStore.playClickInfo();

            susGainPercent = 75;
            powerGainPercent = 150;
            awarenessGainSpeed = 50;
            investigatorCount = 50;
            setTextFieldsToCurrentValues();
        }

        public void setMedium()
        {
            ui.world.audioStore.playClickInfo();

            susGainPercent = 100;
            powerGainPercent = 100;
            awarenessGainSpeed = 100;
            investigatorCount = 100;
            setTextFieldsToCurrentValues();
        }
        public void setHard()
        {
            ui.world.audioStore.playClickInfo();

            susGainPercent = 150;
            powerGainPercent = 50;
            awarenessGainSpeed = 150;
            investigatorCount = 150;
            setTextFieldsToCurrentValues();
        }
        public void setTextFieldsToCurrentValues()
        {
            World.log("Setting seed " + currentSeed);
            suspicionGain.text = "" + susGainPercent;
            powerGain.text = "" + powerGainPercent;
            awarenessGain.text = "" + awarenessGainSpeed;
            investigatorPercentField.text = "" + investigatorCount;
            seedField.text = "" + currentSeed;
            sizeXField.text = "" + sizeX;
            sizeYField.text = "" + sizeY;
            historicalField.text = "" + burnIn;
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
                int awar = int.Parse(awarenessGain.text);
                if (awar >= 0 && awar < 1000)
                {
                    awarenessGainSpeed = awar;
                }
            }
            catch (Exception e) { }
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
                int val = int.Parse(investigatorPercentField.text);
                if (val >= 0 && val < 1000)
                {
                    investigatorCount = val;
                }
            }
            catch (Exception e) { }
            try
            {
                int val = int.Parse(seedField.text);
                    currentSeed = val;
            }
            catch (Exception e) { }
            try
            {
                sizeX = int.Parse(sizeXField.text);
                if (sizeX < 8) { sizeX = 8; }
            } catch (Exception e) { }
            try
            {
                sizeY = int.Parse(sizeYField.text);
                if (sizeY < 8) { sizeY = 8; }
            }
            catch (Exception e) { }
            try
            {
                burnIn = int.Parse(historicalField.text);
                if (burnIn < 1) { burnIn = 1; }
            }
            catch (Exception e) { }

            //Reset all values. If they're fucked, they're good now
            setTextFieldsToCurrentValues();
        }
    }
}
