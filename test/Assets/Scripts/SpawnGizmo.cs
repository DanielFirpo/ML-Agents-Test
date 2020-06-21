using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGizmo : MonoBehaviour
{
    [SerializeField]
    private Color color = new Color(0.5f, 0.5f, 0.5f, 0.2f);

    void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
