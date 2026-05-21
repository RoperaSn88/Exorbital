using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

[System.Serializable]
public class FadeoutPlayableAsset : PlayableAsset
{
    public ExposedReference<Image> fadeoutPanel;
    public FadeoutBehaviour fadeoutBehaviour;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<FadeoutBehaviour>.Create(graph, fadeoutBehaviour);

        // Get PlayableBehaviour
        var behaviour = playable.GetBehaviour();

        // Resolve Reference
        behaviour.targetImage = fadeoutPanel.Resolve(graph.GetResolver());

        return playable;
    }
}

public class FadeoutBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public Image targetImage;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        targetImage.gameObject.SetActive(true);
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        //targetImage.gameObject.SetActive(false);
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
        float progress = (float)(playable.GetTime() / playable.GetDuration());
        targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 1 - progress);
    }
}