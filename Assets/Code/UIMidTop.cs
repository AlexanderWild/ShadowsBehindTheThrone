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
            worldPanicValue.text = ((int)(100*master.world.map.worldPanic)) + "%";

            string desc = "World Panic represents how concerned the world at large is about the coming darkness. As it rises, nobles with awareness can begin to take actions to either warn each other or"
                + " to take action against you and your enthralled.";

            desc += "\nWorld Panic is increased temporarily by you using power, by the fall of human settlements and by enshadowed nobles.";
            desc += "\nYour current panic level is caused by:";

            List<ReasonMsg> reasons = new List<ReasonMsg>();
            master.world.map.overmind.computeWorldPanic(reasons);
            foreach (ReasonMsg msg in reasons)
            {
                desc += "\n*" + ((int)msg.value) + "% " + msg.msg;
            }

            desc += "\n\nPanic will allow the following actions:";

            desc += "\n\n*" + ((int)(100 * master.world.map.param.panic_canInvestigate)) +
                "% Nobles with awareness above " + ((int)(100 * master.world.map.param.awareness_canInvestigate)) + "% can perform 'Investigate', allowing them to add evidence to enthralled neighbouring them.";

            desc += "\n\n*" + ((int)(100 * master.world.map.param.panic_letterWritingLevel)) +
                "% Nobles with awareness above " + ((int)(100 * master.world.map.param.awareness_letterWritingLevel)) + "% can perform 'Warn Friend', allowing them to warn a neighbouring friendly noble.";

            desc += "\n\n*" + ((int)(100 * master.world.map.param.panic_letterWritingToAllLevel)) +
                "% Nobles with awareness above " + ((int)(100 * master.world.map.param.awareness_letterWritingLevel)) + "% can perform 'Warn Noble', allowing them to warn a nearby noble.";

            desc += "\n\n*" + ((int)(100 * master.world.map.param.panic_cleanseSoulLevel)) +
                "% Nobles with awareness above " + ((int)(100 * master.world.map.param.awareness_cleanseSoulLevel)) + "% can perform 'Cleanse Soul', removing a small amount of shadow from themselves.";

            desc += "\n\n*" + ((int)(100 * master.world.map.param.panic_researchAtUniWithoutAwareness)) +
                "% Nobles at a place of learning (university/library/archive) can begin research to gain awareness even if they have none.";

            desc += "\n\n*" + ((int)(100 * master.world.map.param.panic_canAlly)) +
                "% Nobles with awareness " + ((int)(100 * master.world.map.param.awareness_canProposeLightAlliance)) + "% can propose an alliance against the darkness with other nations.";




            worldPanicDesc.text = desc;
        }

        public void bExpandPanic()
        {
            master.world.audioStore.playClickInfo();
            worldPanicDescBox.SetActive(!worldPanicDescBox.activeInHierarchy);
        }
    }
}
