using UnityEngine;
using UnityEngine.VFX;

public class VFXEventEmitter : MonoBehaviour
{
    public VisualEffect vfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            vfx.SendEvent("OnPlay");
        }
    }
}
