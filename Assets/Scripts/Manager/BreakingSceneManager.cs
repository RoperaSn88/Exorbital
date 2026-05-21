using UnityEngine;

public class BreakingSceneManager : MonoBehaviour
{
    public Vector3 PlayerSpawnPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerController.instance.transform.position=PlayerSpawnPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
