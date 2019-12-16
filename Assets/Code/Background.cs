using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Background : MonoBehaviour
    {
        public void Start()
        {

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            float cameraHeight = Camera.main.orthographicSize * 2;
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            Vector2 scale = new Vector2((Camera.main.aspect * cameraHeight) / spriteSize.x, cameraHeight / spriteSize.y);

            transform.localScale = scale;
        }

        public void Update()
        {

        }
    }
}
