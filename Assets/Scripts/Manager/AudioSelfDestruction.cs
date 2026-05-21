using UnityEngine;

public class AudioSelfDestruction : MonoBehaviour
{
    public AudioSource Source;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Source=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Source.isPlaying) Destroy(gameObject);
    }
}
