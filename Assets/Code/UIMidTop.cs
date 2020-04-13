using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIMidTop : MonoBehaviour
    {
        public UIMaster master;
        public Text powerText;
        public Text turnText;
        public Text victoryText;
        public InputField cheatField;
        public GameObject worldPanicBlock;
        public GameObject worldPanicDescBox;
        public Text worldPanicValue;
        public Text worldPanicDesc;


        public void Start()
        {
        }

        public void checkData()
        {
            powerText.text = "POWER: " + ((int)Math.Ceiling(master.world.map.overmind.power) + "/" + master.world.map.param.overmind_maxPower);
            turnText.text = "Turn: " + master.world.map.turn;
            victoryText.text = "Enshadowment: " + (int)(100*master.world.map.data_avrgEnshadowment) + "/" + (int)(100*master.world.map.param.victory_targetEnshadowmentAvrg)+"%"
                + "\nHuman Settlements: " + master.world.map.data_nSocietyLocations;
            
            worldPanicBlock.SetActive(master.world.map.param.useAwareness == 1);
            worldPanicValue.text = ((int)master.world.map.worldPanic) + "%";
        }

        public void bExpandPanic()
        {
            worldPanicDescBox.SetActive(!worldPanicDescBox.activeInHierarchy);
        }
    }
}
