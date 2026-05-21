using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloorDetector : MonoBehaviour
{
    public List<GameObject> Floors;



    private void OnTriggerStay(Collider other)
    { 
        if(other.CompareTag("Floor"))  //Ç±ÇÍÇ≈èàóùåyÇ≠Ç»ÇÈÇ©ÅH
        {
            if(!other.GetComponent<FloorInformation>().HittingF)
            {
                other.GetComponent<FloorInformation>().HittingF = true;
                Floors.Add(other.gameObject);
            }
        }
        
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))  
        {
            if (other.GetComponent<FloorInformation>().HittingF)
            {
                other.GetComponent<FloorInformation>().HittingF = false;
                Floors.Remove(other.gameObject);
            }
        }
    }
    
}
