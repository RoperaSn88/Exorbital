using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
// A behaviour that is attached to a playable
public class TemplatePlayableBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public GameObject templateGameObject { get; set; }

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("Timeline:start");
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        Debug.Log("Timeline:stop");

    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("Timeline:behaviourPlay");
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log("Timeline:behaviourPause");
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        Debug.Log("Timeline:Update");
    }
}