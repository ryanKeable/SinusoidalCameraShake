using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SinusoidalCameraShake))]

public class SinusoidalCameraShakeEditor : Editor
{
    SinusoidalCameraShake controller;

    public override void OnInspectorGUI()
    {
        controller = target as SinusoidalCameraShake;

        if (GUILayout.Button($"Camera Shake"))
        {
            controller.Shake();
        }

        EditorGUILayout.Space();

        DrawDefaultInspector();

    }

}
