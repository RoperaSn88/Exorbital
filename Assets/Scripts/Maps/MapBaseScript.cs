using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MapBaseScript : MonoBehaviour
{
    public enum Kinds
    {
        Middle,
        Start,
        End,
    }

    public Transform StartPos;
    //リストの順番は必ず番号が少ない順に。
    public List<Transform> EndPos;
    public Vector3 CenterToStartVec;
    public List<Vector3> CenterToEndVec;

    /*イメージ
        [0] [1] [2]     0で何もなし
        [3]     [4]　　 1でスタート
        [5] [6] [7]　　 2で終わり
     */
    public MapInfoScriptableObject MapInfo;
    public Vector3 MapNumber;//
    public int TrueNum;
    public int FailureInt;
    public int GenerateEnemyCounts;
    public GameObject EnemySummonPos;


    public void SetPoint()
    {
        CenterToEndVec.Clear();
        CenterToStartVec = StartPos.position - transform.position;
        foreach (Transform Trans in EndPos)
        {
            CenterToEndVec.Add(Trans.position - transform.position);
        }
    }
}
