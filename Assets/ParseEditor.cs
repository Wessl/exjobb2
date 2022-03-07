using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Parse))]
public class ParseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Parse myTarget = (Parse)target;
        if(GUILayout.Button("PARSE FILE"))
        {
            myTarget.ParseFile();
        }
        if(GUILayout.Button("Use parsed results"))
        {
            myTarget.UseParsedFile();
        }
    }
}
