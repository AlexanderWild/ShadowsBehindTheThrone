using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class GraphicalLink : MonoBehaviour
    {
        public Link link;
        public LineRenderer lineRenderer;
        public Vector3[] points;
        public SpriteRenderer combatIcon;

        public void setTo(Link l)
        {
            points = new Vector3[2];
            this.link = l;
        }

        public void Update()
        {
            points[0] = GraphicalMap.getLoc(link.a.hex);
            points[1] = GraphicalMap.getLoc(link.b.hex);
            points[0].z = -0.06f;
            points[1].z = -0.06f;
            lineRenderer.SetPositions(points);


            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            combatIcon.sprite = null;
            if (link.a.soc != null && link.b.soc != null)
            {
                this.gameObject.transform.position = (GraphicalMap.getLoc(link.a.hex) + GraphicalMap.getLoc(link.b.hex)
                    + new Vector3(0, 0, -6)) / 2;
                if (link.a.hex.outer != null)
                {
                    this.gameObject.transform.localScale = link.a.hex.outer.transform.localScale;
                }
                
                if (link.a.soc.getRel(link.b.soc).state == DipRel.dipState.war)
                {
                    combatIcon.enabled = true;
                    combatIcon.sprite = link.a.hex.map.world.textureStore.icon_combat;
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                }
                else
                {
                    combatIcon.enabled = false;
                }
            }
        }
    }
}
