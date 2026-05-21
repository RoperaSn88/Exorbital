using UnityEngine;
using UnityEngine.Playables;

public class testStopTimeline : MonoBehaviour
{
    public PlayableDirector[] Directors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Directors[0].Resume();
        }
    }
}
