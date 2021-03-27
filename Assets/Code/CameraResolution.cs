using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{

    /// <summary>
    /// Skrypt odpowiada za usatwienie rozdzielczosci kemerze
    /// </summary>
    public class CameraResolution : MonoBehaviour
    {


        #region Pola
        private int ScreenSizeX = 0;
        private int ScreenSizeY = 0;
        #endregion

        #region metody

        #region rescale camera
        private void RescaleCamera()
        {

            if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;

            float targetaspect = 16.0f / 9.0f;
            float windowaspect = (float)Screen.width / (float)Screen.height;
            float scaleheight = windowaspect / targetaspect;
            Camera camera = GetComponent<Camera>();

            if (scaleheight < 1.0f)
            {
                Rect rect = camera.rect;

                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;

                camera.rect = rect;
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleheight;

                Rect rect = camera.rect;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }

            ScreenSizeX = Screen.width;
            ScreenSizeY = Screen.height;
        }
        #endregion

        #endregion

        #region metody unity

        void OnPreCull()
        {
            if (Application.isEditor) return;
            Rect wp = Camera.main.rect;
            Rect nr = new Rect(0, 0, 1, 1);

            Camera.main.rect = nr;
            GL.Clear(true, true, Color.black);

            Camera.main.rect = wp;

        }

        // Use this for initialization
        void Start()
        {
            RescaleCamera();
        }

        // Update is called once per frame
        void Update()
        {
            RescaleCamera();
        }
        #endregion
    }
}
