using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineManager : MonoBehaviour
{
    public static TimeLineManager instance;
    public PlayableDirector BaseDirector;
    public TimelineClass[] TimelineClasses;
    bool isPlaying;
    bool isPaused;

    PlayableTrack targetTrack;
    [SerializeField] TextMeshProUGUI targetText;

    void Start()
    {
        instance = this;
    }
    public void StartTimeline(int id)
    {
        isPlaying = true;
        TimelineAsset targetAsset = null;
        foreach (var c in TimelineClasses)
        {
            if (c.ID == id) targetAsset = c.targetTimeline;
            Debug.Log($"Timeline.{id}");
        }
        if (targetAsset == null)
        {
            Debug.LogError("Not Found such a ID in TimelineClass!");
            return;
        }

        BaseDirector.playableAsset = targetAsset;
        targetTrack = BaseDirector.playableAsset as PlayableTrack;
        BaseDirector.Play();
    }

    public void StopTimeline()
    {
        isPaused = false;
        isPlaying = false;
        //if (targetText) targetText.text = "";
    }

    public void PauseTimeline()
    {
        isPaused = true;
        BaseDirector.Pause();
    }

    public void ResumeTimeline()
    {
        isPaused = false;
        BaseDirector.Resume();
    }

    public void ResetText()
    {
        targetText.text = "";
    }
    

    void Update()
    {
        //再生中にキーが押されたらコマの最後まで飛ばしたい
        if (isPlaying && !isPaused)
        {

        }
        //ポーズ中にキーが押されたら再生したい
        if (isPlaying && isPaused)
        {
            if (Input.GetButtonDown("CrossButton")||Input.GetKeyDown(KeyCode.Space))
            {
                ResumeTimeline();
            }
        }
    }
}

[System.Serializable]
public class TimelineClass
{
    public TimelineAsset targetTimeline;
    public int ID;
}
