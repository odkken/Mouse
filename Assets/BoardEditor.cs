using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Misc;
using MoreLinq;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    [CustomEditor(typeof(Board))]
    [CanEditMultipleObjects]
    public class BoardEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            var b = (target as Board);

            if (GUILayout.Button("Load"))
            {
                b.LoadFromFile(true);
            }



        }
    }
}