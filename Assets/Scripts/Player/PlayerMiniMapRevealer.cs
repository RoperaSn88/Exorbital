using UnityEngine;

public class PlayerMiniMapRevealer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        RevealMap(other);
    }

    void OnTriggerStay(Collider other)
    {
        RevealMap(other);
    }

    void RevealMap(Collider other)
    {
        if (SceneManagerScript.instance == null || other == null) return;

        MapBaseScriptVer2 map = other.GetComponentInParent<MapBaseScriptVer2>();
        if (map == null) return;

        SceneManagerScript.instance.RevealVisitedMiniMap(map);
    }
}
