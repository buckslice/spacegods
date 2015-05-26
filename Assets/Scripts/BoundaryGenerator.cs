using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// used to be an editor script (check repo if interested)
[RequireComponent(typeof(PolygonCollider2D))]
public class BoundaryGenerator : MonoBehaviour {

    // 2 player, 3 player and 4 player
    public float[] boundarySize = { 20f, 35f, 50f };

    private float padding = 5f;
    public int points = 30;

    public void generate(int numPlayers) {
        if (numPlayers < 2) {
            Debug.Log("BoundaryGenerator error: less than 2 players");
            return;
        }
        if (numPlayers - 2 >= boundarySize.Length) {
            Debug.Log("BoundaryGenerator error: no boundary size defined for " + numPlayers + " players");
            return;
        }

        float radius = boundarySize[numPlayers - 2];
        PlanetSpawner.current.boundaryRadius = radius - 2f;

        PolygonCollider2D poly = gameObject.GetComponent<PolygonCollider2D>();

        List<Vector2> ps = new List<Vector2>();
        ps.Add(new Vector2(-radius - padding, radius + padding));
        ps.Add(new Vector2(-radius - padding, -radius - padding));
        ps.Add(new Vector2(radius + padding, -radius - padding));
        ps.Add(new Vector2(radius + padding, radius + padding));
        ps.Add(new Vector2(-radius - padding, radius + padding));

        for (int i = 0; i <= points; i++) {
            float angle = (float)i / points * Mathf.PI * 2f;
            Vector2 v = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            ps.Add(v);
        }

        poly.points = ps.ToArray();

        Transform quad = transform.Find("Quad");
        quad.localScale = new Vector3(radius * 5f, radius * 5f, 1f);

    }

}
