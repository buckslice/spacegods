using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BoundaryGenerator : EditorWindow {

    private float radius;
    private float padding;
    private int points;

    [MenuItem("Window/Boundary Generator")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(BoundaryGenerator));
    }

    void OnGUI() {
        GUILayout.Label("Generation Settings", EditorStyles.boldLabel);

        radius = EditorGUILayout.FloatField("Radius", radius);
        padding = EditorGUILayout.FloatField("Padding", padding);
        points = EditorGUILayout.IntField("Points", points);

        Rect r = EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(r, GUIContent.none)) {
            generate();
        }
        GUILayout.Label("Generate");
        EditorGUILayout.EndHorizontal();

    }

    private void generate() {
        GameObject go = new GameObject("Boundary");
        PolygonCollider2D poly = go.AddComponent<PolygonCollider2D>();

        List<Vector2> ps = new List<Vector2>();
        //ps.Add(new Vector2(0, radius + 5f));
        ps.Add(new Vector2(-radius - padding, radius + padding));
        //ps.Add(new Vector2(-radius - 5f, 0));
        ps.Add(new Vector2(-radius - padding, -radius - padding));
        //ps.Add(new Vector2(0, -radius - 5f));
        ps.Add(new Vector2(radius + padding, -radius - padding));
        //ps.Add(new Vector2(radius + 5f, 0));
        ps.Add(new Vector2(radius + padding, radius + padding));
        //ps.Add(new Vector2(0, radius + 5f));
        ps.Add(new Vector2(-radius - padding, radius + padding));

        for (int i = 0; i <= points; i++) {
            float angle = (float)i / points * Mathf.PI * 2f;
            Vector2 v = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            ps.Add(v);
        }

        poly.points = ps.ToArray();
    }

}
