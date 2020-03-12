using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    [CustomEditor(typeof(World))]
    public class WorldParams : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Save Params to File"))
            {
                Params param = new Params();
                param.saveToFile();
            }
        }
    }
}
