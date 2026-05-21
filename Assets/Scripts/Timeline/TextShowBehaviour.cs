using UnityEngine;
using UnityEngine.Playables;
using TMPro;

[System.Serializable]
public class TextShowBehaviour : PlayableBehaviour
{
    public PlayableDirector director;
    public TMP_Text targetText { get; set; }
    [TextArea]
    public string TextContent;
    //public bool pauseScheduled = false;


    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
        Debug.Log(director);
        
    }
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if (targetText != null)
        {
            float progress = (float)(playable.GetTime() / playable.GetDuration());
            var current = Mathf.Lerp(0, targetText.text.Length, progress);
            var count = Mathf.CeilToInt(current);

            targetText.maxVisibleCharacters = count;
        }
        //Debug.Log("Flaming");
    }

    public override void OnGraphStart(Playable playable)
    {
        //targetText.text = "";
    }

    public override void OnGraphStop(Playable playable)
    {
        //targetText.text = "";
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //director = (playable.GetGraph().GetResolver() as PlayableDirector);
        targetText.text = TextContent;
        //Debug.Log($"Director:{director.playableGraph.IsValid()}");
    }

    // public override void OnBehaviourPause(Playable playable, FrameData info)
    // {
    //     if (pauseScheduled)
    //     {
    //         director.Pause();
    //         pauseScheduled = false;

    //     }
    //     else
    //     {
    //         Debug.Log("puasesitenai");
    //     }
    // }
}
