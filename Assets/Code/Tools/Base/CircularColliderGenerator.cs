using UnityEngine;
using System.Collections.Generic;

public class CircularColliderGenerator : MonoBehaviour
{
    private const float MinRadius = 0.1f;
    private const int MinCollidersCount = 4;
    private const int MaxCollidersCount = 64;
    private const float MinCollidersHeight = 0.1f;
    private const float MinCollidersDepth = 0.1f;
    private const int GizmoSegments = 32;
    private const float GizmoCubeSize = 0.3f;

    [Header("Circle Settings")]
    [SerializeField] private float _radius = 5f;
    [SerializeField][Range(MinCollidersCount, MaxCollidersCount)] private int _colliderCount = 8;
    [SerializeField] private float _colliderHeight = 2f;
    [SerializeField] private float _colliderDepth = 1f;

    [Header("Visualization")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private Color _gizmoColor = Color.green;
    [SerializeField] private Color _colliderGizmoColor = Color.red;

    private readonly List<GameObject> _colliderObjects = new();

    [ContextMenu("Generate Colliders")]
    public void GenerateColliders()
    {
        ClearColliders();

        if (_colliderCount < MinCollidersCount)
        {
            Debug.LogWarning($"Collider count must be at least {MinCollidersCount}");

            return;
        }

        if (_radius < MinRadius)
        {
            Debug.LogWarning($"Radius must be at least {MinRadius}");

            return;
        }

        if (_colliderHeight < MinCollidersHeight || _colliderDepth < MinCollidersDepth)
        {
            Debug.LogWarning($"Collider height and depth must be at least {MinCollidersHeight}");

            return;
        }

        float angleStep = 360f / _colliderCount;

        for (int i = 0; i < _colliderCount; i++)
        {
            GameObject colliderObject = new($"Collider_{i}");
            colliderObject.transform.SetParent(transform);
            colliderObject.transform.localPosition = Vector3.zero;
            colliderObject.transform.localRotation = Quaternion.identity;

            BoxCollider boxCollider = colliderObject.AddComponent<BoxCollider>();

            float angle = i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 position = direction * _radius;

            colliderObject.transform.localPosition = position;
            colliderObject.transform.localRotation = Quaternion.Euler(0, angle, 0);

            float chordLength = 2f * _radius * Mathf.Sin(Mathf.PI / _colliderCount);

            float overlap = chordLength * 0.01f;
            chordLength += overlap;

            boxCollider.size = new Vector3(chordLength, _colliderHeight, _colliderDepth);
            boxCollider.center = new Vector3(0, 0, -_colliderDepth * 0.5f);

            _colliderObjects.Add(colliderObject);
        }

        Debug.Log($"Generated {_colliderCount} colliders with radius {_radius}");
    }

    [ContextMenu("Clear Colliders")]
    public void ClearColliders()
    {
        foreach (GameObject obj in _colliderObjects)
        {
            if (obj != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(obj);
                }
                else
                {
                    DestroyImmediate(obj);
                }
            }
        }

        _colliderObjects.Clear();

        List<Transform> childrenToRemove = new();

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Collider_"))
            {
                childrenToRemove.Add(child);
            }
        }

        foreach (Transform child in childrenToRemove)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        Debug.Log("Cleared all colliders");
    }

    void OnValidate()
    {
        _radius = Mathf.Max(MinRadius, _radius);
        _colliderCount = Mathf.Clamp(_colliderCount, MinCollidersCount, MaxCollidersCount);
        _colliderHeight = Mathf.Max(MinCollidersHeight, _colliderHeight);
        _colliderDepth = Mathf.Max(MinCollidersDepth, _colliderDepth);
    }

    void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        Gizmos.color = _gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= GizmoSegments; i++)
        {
            float angle = (float)i / GizmoSegments * 360f * Mathf.Deg2Rad;
            Vector3 point = new(Mathf.Sin(angle) * _radius, 0, Mathf.Cos(angle) * _radius);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, point);
            }

            prevPoint = point;
        }

        if (_colliderCount > 0)
        {
            float angleStep = 360f / _colliderCount;
            float chordLength = 2f * _radius * Mathf.Sin(Mathf.PI / _colliderCount);

            Gizmos.color = _colliderGizmoColor;

            for (int i = 0; i < _colliderCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 position = new(Mathf.Sin(angle) * _radius, 0, Mathf.Cos(angle) * _radius);

                Gizmos.DrawWireCube(position, new Vector3(GizmoCubeSize, GizmoCubeSize, GizmoCubeSize));

                DrawColliderGizmo(position, angle * Mathf.Rad2Deg, chordLength, _colliderHeight, _colliderDepth);
            }
        }
    }

    private void DrawColliderGizmo(Vector3 center, float angle, float width, float height, float depth)
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;

        Matrix4x4 colliderMatrix = Matrix4x4.TRS(
            transform.TransformPoint(center),
            transform.rotation * Quaternion.Euler(0, angle, 0),
            Vector3.one
        );

        Gizmos.matrix = colliderMatrix;

        Vector3 colliderCenter = new(0, 0, -depth * 0.5f);
        Vector3 colliderSize = new(width, height, depth);

        Gizmos.DrawWireCube(colliderCenter, colliderSize);
        Gizmos.matrix = originalMatrix;
    }

    [ContextMenu("Check Existing Colliders")]
    public void CheckExistingColliders()
    {
        int existingCount = 0;

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Collider_") && child.GetComponent<BoxCollider>())
            {
                existingCount++;
            }
        }

        Debug.Log($"Found {existingCount} existing colliders");
    }
}