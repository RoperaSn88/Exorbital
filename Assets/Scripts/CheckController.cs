using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        float LHorizontal = Input.GetAxis("Horizontal");
        float LVertical = Input.GetAxis("Vertical");
        float RHorizontal = Input.GetAxisRaw("RHorizontal");
        float RVertical = Input.GetAxisRaw("RVertical");

        if (RVertical != 0) Debug.Log($"RVertical:{RVertical}");
        if (RHorizontal != 0) Debug.Log($"RHorizontal:{RHorizontal}");

        if (Input.GetButtonDown("CircleButton")) Debug.Log("Circle");

        if (Input.GetButtonDown("CrossButton")) Debug.Log("Cross");
        if (Input.GetButtonDown("SquareButton")) Debug.Log("Square");
        if (Input.GetButtonDown("TriangleButton")) Debug.Log("Triangle");
        if (Input.GetAxis("HorizontalButton") == 1) Debug.Log("Horizontal");
        if (Input.GetAxis("VerticalButton") == -1) Debug.Log("Vertical");
        if (Input.GetButtonDown("L1Button")) Debug.Log("L1");
        if (Input.GetButtonDown("R1Button")) Debug.Log("R1");
        // if (Input.GetAxis("L2Button") == -1) Debug.Log("R2");
        // if (Input.GetAxis("R2Button") == 1) Debug.Log("L2");
        // Debug.Log("R2: " + Input.GetAxis("R2Button") + ",L2: " + Input.GetAxis("L2Button"));
        #endif
    }
}
