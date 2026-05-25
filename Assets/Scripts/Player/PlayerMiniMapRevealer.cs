using System.Collections.Generic;
using UnityEngine;

public class PlayerMiniMapRevealer : MonoBehaviour
{
    readonly HashSet<int> _revealedColliderIds = new HashSet<int>();

    void Start()
    {
        Collider ownCollider = GetComponent<Collider>();
        if (ownCollider == null) return;

        Bounds bounds = ownCollider.bounds;
        Collider[] overlaps = Physics.OverlapBox(
            bounds.center,
            bounds.extents,
            transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider overlap in overlaps)
        {
            if (overlap == ownCollider) continue;
            RevealMap(overlap);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        RevealMap(other);
    }

    void OnDisable()
    {
        _revealedColliderIds.Clear();
    }

    void RevealMap(Collider other)
    {
        if (SceneManagerScript.instance == null || other == null) return;
        int colliderId = other.GetInstanceID();
        if (_revealedColliderIds.Contains(colliderId)) return;

        MapBaseScriptVer2 map = other.GetComponentInParent<MapBaseScriptVer2>();
        if (map == null) return;

        SceneManagerScript.instance.RevealVisitedMiniMap(map);
        _revealedColliderIds.Add(colliderId);
    }
}
