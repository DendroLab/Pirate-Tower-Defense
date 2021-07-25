using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    internal List<Transform> Points = new List<Transform>();

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Points.Add(transform.GetChild(i));
        }
    }

    // Untuk menampilkan garis penghubung dalam window Scene
    // tanpa harus di-Play terlebih dahulu
    private void OnDrawGizmos()
    {
        List<Transform> gizmoPoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            gizmoPoints.Add(transform.GetChild(i));
        }

        for (int i = 0; i < gizmoPoints.Count - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(gizmoPoints[i].position, gizmoPoints[i + 1].position);
        }
    }
}
