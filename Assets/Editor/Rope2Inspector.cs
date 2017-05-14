/*=============={KEEP INFO INTACT}==================
===   Name: Rope Script Inspector Version 1      ===
===   Company: Reverie Interactive               ===
===   ----------------------------------------   ===
===   Written By: Jacob Fletcher                 ===
===   Release: September, 26, 2010               ===
===   ----------------------------------------   ===
===   Copyright: Reverie Interactive             ===
===   License: Free Use if this box is left alone  =
==================================================*/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// This example shows a custom inspector for an
// object "MyPlayer", which has two variables, speed and ammo.
[CustomEditor(typeof(Rope2))]
public class Rope2Inspector : Editor
{
    bool showStandardInspector = false;

    override public void OnInspectorGUI()
    {
        Rope2 rs = (Rope2)target;
        
        EditorGUILayout.Space();
        GUILayout.Label("Use Editor To Edit Rope: RI Editors -> Rope Editor");

        EditorGUILayout.Space();
        if (showStandardInspector = EditorGUILayout.Toggle("Classic View", showStandardInspector))
        {
            DrawDefaultInspector();
        }
    }
}