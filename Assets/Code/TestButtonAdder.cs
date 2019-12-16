using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Code
{
    public class TestButtonAdder : MonoBehaviour
    {
        public GameObject testButton;

        // Use this for initialization
        void Start()
        {
            fillActionsInTab();
        }

        public void fillActionsInTab()
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject sp = Instantiate(testButton, this.gameObject.transform);
                sp.transform.Translate(new Vector3(0, i*128, 0));
                Button script = sp.GetComponent<Button>();
                script.onClick.AddListener(delegate { bOnClick(i); });
            }
        }

        public void bOnClick(int i)
        {
            World.log("Msg: " + i);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
