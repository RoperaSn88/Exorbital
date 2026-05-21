using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniEnemyGenerater : MonoBehaviour
{
    [SerializeField] List<EnemyScript> Enemys;
    [SerializeField] float Interval;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GenerateEnemy",0,Interval);
    }


    void GenerateEnemy()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length>17) return;
        if (SceneManagerScript.instance.EnemyControllF)
        {
            int Size = SceneManagerScript.instance.GenerateSize;
            while (Size > 0)
            {
                EnemyScript Enemy = Enemys[Random.Range(0, Enemys.Count)];
                if (Enemy.Size <= Size)
                {
                    EnemyScript SummonEnemy = Instantiate(Enemy, gameObject.transform).GetComponent<EnemyScript>();
                    Size -= Enemy.Size;
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
