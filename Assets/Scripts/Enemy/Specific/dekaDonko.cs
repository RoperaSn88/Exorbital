using NUnit.Framework.Internal;
using UnityEngine;

public class dekaDonko : EnemyScript
{
    public GameObject[] Dongriz;

    public void SpawnDongri()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(Dongriz[Random.Range(0, Dongriz.Length)], transform);
        }
    }
}


