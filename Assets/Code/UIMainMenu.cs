using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIMainMenu : MonoBehaviour
    {
        public Button continueButton;
        public Button saveButton;
        public Button startStreamlinedButton;
        public Button startButton;
        public Text generateText;
        public Text advancedEditionText;
        public Text versionNumberText;
        public World world;

        public void Update()
        {
            continueButton.gameObject.SetActive(world.map != null);
            saveButton.gameObject.SetActive(world.map != null);
            //startStreamlinedButton.gameObject.SetActive(world.map == null);
            //startButton.gameObject.SetActive(world.map == null);
            advancedEditionText.gameObject.SetActive(World.advancedEdition);
            versionNumberText.text = "Version " + World.versionNumber + "." + World.subversionNumber;
        }
    }
}
