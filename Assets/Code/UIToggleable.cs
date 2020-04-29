using UnityEngine;
using System.Collections;

namespace Assets.Code
{
    public class UIToggleable : MonoBehaviour
    {
        public GameObject box;
        public World world;

        public void bToggle()
        {
            world.audioStore.playClickInfo();
            box.SetActive(!box.activeInHierarchy);
        }
    }
}
