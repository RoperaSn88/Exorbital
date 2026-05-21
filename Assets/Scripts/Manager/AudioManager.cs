using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.ComponentModel;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}
    [Range(0f,1f)]
    public float BGMvolume;
    [Range(0f,1f)]
    public float SEvolume;
    public AudioSource BGMsource;
    public AudioSource[] SEsources;

    void Start()
    {
        instance=this;
    }
    public void PlayBGM(AudioClip clip){
        if (BGMsource.isPlaying)
        {
            BGMsource.Stop();
        }
        staticScript._bgmVolume = BGMvolume;
        BGMsource.volume = BGMvolume;
        BGMsource.clip = clip;
        BGMsource.Play();
    }
    float firstvol=0;

    public void StopBGM(float time){
        //DOTweenではうまくいかないのでUpdateで対処
        if(BGMsource.volume==0) return;
        EndTime=time;
        StopTime=0;
        StoppingBGM=true;
        firstvol=BGMvolume;
    }
    bool StoppingBGM=false;
    float StopTime=0;
    float EndTime;
    public void Update()
    {
        if(StoppingBGM){
            if(StopTime<EndTime){
                StopTime+=Time.deltaTime;
                BGMsource.volume=firstvol-BGMvolume*StopTime/EndTime;
            }
            else {
                StoppingBGM=false;
                EndTime=0;
                StopTime=0;
            }
        }
    }

    public void PlaySE(AudioClip clip,float vol = 1f){
        //空のオブジェクトを生成できるようなので、それで済ませる

        GameObject audio = new GameObject();
        AudioSource source =audio.AddComponent<AudioSource>();
        AudioSelfDestruction selfD = audio.AddComponent<AudioSelfDestruction>();
        staticScript._seVolume = SEvolume;
        source.volume = SEvolume * vol;
        source.spatialBlend = 0;
        source.clip = clip;
        source.Play();
    }

    public void PlaySE3D(AudioClip clip,Transform trans){
        GameObject audio = new GameObject();
        AudioSource source = audio.AddComponent<AudioSource>();
        AudioSelfDestruction selfD = audio.AddComponent<AudioSelfDestruction>();
        source.volume = SEvolume;
        source.spatialBlend = 1;
        source.transform.position = trans.position;
        source.clip = clip;
        source.Play();
    }
}