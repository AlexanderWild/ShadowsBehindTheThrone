using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class ParticleCombat : MonoBehaviour
    {
        public SpriteRenderer sRend;
        public float scale;
        public Hex a;
        public Hex b;
        public int frames;
        public Map map;
        public int lastCheck = -1;

        public void Update()
        {
            if (scale > 1f)
            {
                return;
            }
            if (GraphicalMap.lastMapChange != lastCheck)
            {
                lastCheck = GraphicalMap.lastMapChange;
                gameObject.transform.position = ((GraphicalMap.getLoc(a) + GraphicalMap.getLoc(b)) / 2) + new Vector3(0, 0, -8);
            }

            frames += 1;
            float scaleStep = 0.02f;
            scale = frames * scaleStep;
            //gameObject.transform.localScale = new Vector3(scale,scale,1);

            float c = 1 - (scale * scale);
            if (c < 0) { c = 0; }
            sRend.color = new Color(1, 1, 1, c);

            if (scale > 1f)
            {
                gameObject.transform.localScale = new Vector3(0, 0, 1);
                Destroy(this.gameObject);
            }
        }
    }
}
