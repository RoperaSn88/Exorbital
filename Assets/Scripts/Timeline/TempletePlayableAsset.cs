using TMPro;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TemplatePlayableAsset : PlayableAsset
{
    [SerializeField]
    private ExposedReference<TMP_Text> targetText;

    //public TemplatePlayableBehaviour template = new TemplatePlayableBehaviour();
    public TextShowBehaviour TextBehaviour = new TextShowBehaviour();
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {

        var playable = ScriptPlayable<TextShowBehaviour>.Create(graph, TextBehaviour);

        // Get PlayableBehaviour
        var behaviour = playable.GetBehaviour();

        // Resolve Reference
        behaviour.targetText = targetText.Resolve(graph.GetResolver());

        return playable;
    }
}