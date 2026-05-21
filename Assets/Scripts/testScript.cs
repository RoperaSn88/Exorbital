using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{
    int Mask;
    bool Grounded;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] obgs = GameObject.FindGameObjectsWithTag("EndArea");
        foreach(var o in obgs){
            Debug.Log("end:" + o.name);
        }
    }

}
