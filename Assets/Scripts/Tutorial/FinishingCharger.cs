using UnityEngine;

public class FinishingCharger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance._specialTechCoolTime = 179.5f;
            Destroy(gameObject);
        }
    }
}
