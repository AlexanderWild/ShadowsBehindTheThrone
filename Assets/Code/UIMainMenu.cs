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
        public Text generateText;
        public World world;

        public void Update()
        {
            continueButton.gameObject.SetActive(world.map != null);
            saveButton.gameObject.SetActive(world.map != null);
        }
    }
}
