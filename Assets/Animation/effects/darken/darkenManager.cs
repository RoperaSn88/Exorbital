using System.Collections;
using UnityEngine;

public class darkenManager : MonoBehaviour
{
    public AudioClip summonSound;
    public Collider col;
    public ParticleSystem par1;
    public ParticleSystem par2;
    public IEnumerator Start()
    {
        par1.Play();
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlaySE3D(summonSound, transform);
        par2.Play();
        col.enabled = true;
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }
}
