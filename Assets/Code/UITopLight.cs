using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UITopLight : MonoBehaviour
    {
        public UIMaster master;
        public GameObject all;
        public World world;
        public Text buttonText;

        public void Start()
        {
        }

        public void Update()
        {
            all.gameObject.SetActive(world.map != null && world.map.overmind.lightbringerCasters != null);
        }

        public void checkData()
        {
            buttonText.text = "Lightbringer Ritual\nTurns Remaining " + (world.map.param.awareness_turnsForLightRitual - world.map.overmind.lightRitualProgress);

        }

        public void bPopupDialog()
        {
            if (world.map != null && world.map.overmind != null && world.map.overmind.lightbringerCasters != null)
            {
                world.prefabStore.popLightbringerMsg(world.map.overmind.lightbringerCasters);

            }
        }
    }
}
