using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupPlayback : MonoBehaviour
    {
        public Button back;
        public Button replay;
        public Button pause;
        public Image background;
        public PopupBaseHex[,] hexes;
        public Text title;
        public GameObject centreTarget;
        public GameObject midpoint;

        public World world;
        public Map map;

        private bool paused = false;
        float lastStep = -1;
        float betweenSteps = 0.1f;

        int snapshotIndex;
        public void Update()
        {
            float time = Time.time;
            float delta = time - lastStep;
            if (delta > betweenSteps)
            {
                //if (snapshotIndex >= map.stats.snapshots.Count)
                //{
                //    snapshotIndex = 0;
                //}
                
                StatSnapshot shot = map.stats.snapshots[snapshotIndex];

                //int gameTurn = shot.day - map.options.burnInSteps;
                int gameTurn = shot.day;


                title.text = "Turn: " + gameTurn;
                if (gameTurn < 0) { title.text += " (World Gen)"; }

                for (int i = 0; i < shot.hexColIndices.Count; i++)
                {
                    int[] loc = shot.hexColIndices[i];
                    float[] col = shot.hexColValues[i];
                    hexes[loc[0], loc[1]].sprite.color = new Color(col[0],col[1],col[2]); 
                }

                if (!paused && snapshotIndex < map.stats.snapshots.Count-1)
                {
                    snapshotIndex += 1;
                }
                lastStep = time;
            }
        }

        public void setup(World world,Map map)
        {
            this.world = world;
            this.map = map;

            //Scale 1 is for x = 48, y = 32
            float scale = 1;

            float s1 = 48f / map.sizeX;
            float s2 = 32f / map.sizeY;
            scale = Math.Min(s1, s2);

            float ox = 10;
            float oy = 4.7f;
            float stepx = 0.35f*scale;
            float stepy = 0.3f*scale;

            if (s2 > s1)
            {
                oy *= s1 / s2;
            }
            ox = (map.sizeX / 2) * stepx;
            
            PopupBaseHex hidden = world.prefabStore.getBaseHex();
            hidden.sprite.color = new Color(0, 0, 0, 0);
            midpoint = hidden.gameObject;

            hexes = new PopupBaseHex[map.sizeX, map.sizeY];
            for (int i = 0; i < map.sizeX; i++)
            {
                for (int j = 0; j < map.sizeY; j++)
                {
                    PopupBaseHex hex = world.prefabStore.getBaseHex();
                    hex.sprite.color = new Color(0, 0, 0, 0);
                    hexes[i, j] = hex;
                    hex.transform.localScale = new Vector3(0.25f * scale, 0.25f * scale, 1f);
                    hex.transform.SetParent(midpoint.transform);

                    float lx = ox - (stepx * i);
                    float ly = oy - (stepy * j);
                    if (j % 2 == 0) { lx -= stepx * 0.5f; }
                    hex.transform.localPosition = new Vector3(lx, ly, -0.1f);
                }
            }

            world.ui.setToBackground();
        }

        public void buttonReplay()
        {
            snapshotIndex = 0;
        }
        public void buttonPause()
        {
            paused = !paused;
        }
        public void buttonBack()
        {
            Destroy(midpoint);
            world.ui.removeBlocker(this.gameObject);
            world.ui.setToWorld();
        }
    }
}
