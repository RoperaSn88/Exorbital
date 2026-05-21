using System.Collections;
using UnityEngine;

public class gou : EnemyScript
{
    public GameObject Darken;

    //アニメーターで再生させる
    public void darken()
    {
        StartCoroutine(darkenCoroutine());
    }

    public IEnumerator darkenCoroutine()
    {
        var PlayerPos = PlayerController.instance.transform.position;
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            Transform trans = Instantiate(Darken).transform;
            trans.position = new Vector3(Random.Range(PlayerPos.x - 2, PlayerPos.x + 2), PlayerPos.y - 0.2f, Random.Range(PlayerPos.z - 2, PlayerPos.z + 2));
            AttackPointScript aps = trans.GetComponent<AttackPointScript>();
            aps.UserData = GetComponent<FinallyCalcuratedBattleData>();
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    public override void DeadAction()
    {
        base.DeadAction();
    }
}
