using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    //[Serializable,HideInInspector]
    public class StatSnapshot
    {
        public int day;
        public List<int[]> hexColIndices = new List<int[]>();
        public List<float[]> hexColValues = new List<float[]>();
    }
}
