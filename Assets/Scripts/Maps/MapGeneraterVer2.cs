using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class MapGeneraterVer2 : MonoBehaviour
{
    public SceneObject thisScene;
    public SceneObject NextScene;
    public Color color;
    public GameObject treasurePrefab;
    public MapBaseScriptVer2 StartMap;
    public GameObject EndArea;
    [Header("マップの大きさの設定")]
    public int MapCount = 5;
    public int MiniMapCountMin;
    public int MiniMapCountMax;
    public int ChallengeMapCounts = 0;
    
    public Transform MapParent;
    public List<MapBaseScriptVer2> MapMaterials;
    public List<MapBaseScriptVer2> MiniEnds;
    public List<MapBaseScriptVer2> Ends;
    public SpecificMapInfo SpecificMapInfo;
    public List<MapBaseScriptVer2> ChallengeMaps;
    [Header("ここからはステージの情報")]
    public String StageName;
    public List<GameObject> Enemys;
    public List<GameObject> EliteEnemys;
    public AudioClip BackGroundMusic;

    int TrueNum;
    public List<Vector3> MapNumbers;
    public List<FailLoophoolClass> Fails;
    public List<MapBaseScriptVer2> CopyMapMaterials;
    public List<MapBaseScriptVer2> CopyMiniEnds;
    private List<MapBaseScriptVer2> CopyChallenges;
    public List<FailLoophoolClass>  EndFails;

    // 生成されたマップのインスタンスを追跡（敵生成用）
    private List<MapBaseScriptVer2> GeneratedMaps;

    public NavMeshSurface NavMesh;
    public Vector3 ReSpawnVec;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateMap());
    }

    public IEnumerator GenerateMap()
    {
        // 生成されたマップのリストを初期化
        if (GeneratedMaps == null)
            GeneratedMaps = new List<MapBaseScriptVer2>();
        else
            GeneratedMaps.Clear();

        List<EnemySpawnClass> EnemySpawns=new List<EnemySpawnClass>();
        List<Transform> TreasureSpawns=new List<Transform>();
        MapBaseScriptVer2 StartMapS = Instantiate(StartMap.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
        StartMapS.transform.position = Vector3.zero;
        GeneratedMaps.Add(StartMapS); // 生成されたマップを追跡
        yield return null;
        Vector3 MapNumber=Vector3.zero;

        PlayerController.instance.transform.position = StartMapS.StartSpawnPos.position;
        ReSpawnVec=StartMapS.StartSpawnPos.position;

        //MapNumbers.Add(MapNumber);
        Vector3 SaveVec = StartMapS.SelectTrueLoophole(MapNumbers,MapNumber,Fails).position;


        StartMapS.SaveMapNumbers(MapNumbers,MapNumber,TrueNum);
        //次につなげる番号を受けつぐ。スタートが右方向だったら5になるはず。
        TrueNum=StartMapS.TrueNum;
        MapNumber=RematchMapNumber(TrueNum,0,MapNumber,StartMapS);
        // マップタイプに基づいて敵スポーン位置を収集
        if(StartMapS.ShouldSpawnEnemies() && StartMapS.EnemySpawnPoses.Count!=0)
            foreach(EnemySpawnClass trans in StartMapS.EnemySpawnPoses) EnemySpawns.Add(trans);

        //スタートの生成が完了

        

        while (MapCount > 0)
        {
            
            MapBaseScriptVer2 SelectMaterial = null;
            
            CopyMapMaterials = new List<MapBaseScriptVer2>(MapMaterials);
            CopyChallenges = new List<MapBaseScriptVer2>(ChallengeMaps);
            
            bool AllMapCantSet = false;

            //普通のマップを生成するか、特殊なマップを生成するかを選択する。
            int specificMapRandomer = UnityEngine.Random.Range(0, 100);
            SpecificMapInfo.ChallengeMapCounts = ChallengeMapCounts;
            if(specificMapRandomer > SpecificMapInfo.randomer && SpecificMapInfo.ChallengeMapCounts > 0){
                /// 特殊マップを道中に生成する
                /// 現段階は強敵マップのみ実装する
                SpecificMapInfo.IsChallenging = true;
            }else {
                SpecificMapInfo.IsChallenging = false;
            }

            while (true)
            {
                //まずは作るマップを選ぶ
                //生成が完全にうまくいかなかったらハズレの出口として生成して別のハズレを新規ルートと採用する。  
                if (CopyMapMaterials.Count == 0)
                {
                    Debug.Log("All Map cant set");
                    AllMapCantSet = true;
                    break;
                }

                //マップをランダムに選ぶ


                if (SpecificMapInfo.IsChallenging)
                {
                    SelectMaterial = CopyChallenges[UnityEngine.Random.Range(0, CopyChallenges.Count)];
                    Debug.LogWarning("チャレンジマップが選ばれました");
                }
                else
                {
                    SelectMaterial = CopyMapMaterials[UnityEngine.Random.Range(0, CopyMapMaterials.Count)];
                }
                //作るマップが今あるマップと混じらないかはあとにする
                //とりあえずは番号が一致するかだけで判断する
                bool check = false;
                //選ばれたやつの抜け道が今のやつとかみあいそうか調べる
                foreach (MapLoopholeVer2 hole in SelectMaterial.Loopholes)
                {
                    if (MatchMapNum(TrueNum, hole.num))
                    {
                        //番号がかみ合いそうならokってことで
                        //Debug.Log($"Selected:{SelectMaterial}");
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    //Debug.Log("NotFound");
                    if(SpecificMapInfo.IsChallenging) CopyChallenges.Remove(SelectMaterial);
                    else CopyMapMaterials.Remove(SelectMaterial);
                    continue;
                }

                if (SelectMaterial.CheckNotCoverMap(MapNumbers, MapNumber, ReverseNum(TrueNum), Fails))
                {
                    //Debug.Log($"NotCover:{SelectMaterial}");
                    if (SelectMaterial.CheckCanSetNextPoint(MapNumbers, MapNumber, ReverseNum(TrueNum), Fails))
                    {
                        //両方満たしているときでしか抜け出せない
                        //Debug.Log($"CompletelySelected:{SelectMaterial}");
                        break;
                    }
                    else
                    {
                        Debug.Log($"Cant set {SelectMaterial}[CanSetNextPoint]");
                        CopyMapMaterials.Remove(SelectMaterial);
                    }

                }
                else
                {
                    Debug.Log($"Cant set {SelectMaterial}[NotCoverMap]");
                    CopyMapMaterials.Remove(SelectMaterial);
                }
            }

            //すべてのマップが生成できなかったら出口を生成する
            if (AllMapCantSet)
            {

                bool checkk = false;
                CopyMiniEnds = new List<MapBaseScriptVer2>(MiniEnds);
                foreach (MapBaseScriptVer2 map in CopyMiniEnds)
                {
                    foreach (MapLoopholeVer2 hole in map.Loopholes)
                    {
                        if (hole.num == ReverseNum(TrueNum))
                        {
                            Debug.Log($"Mini End {map.name}");
                            SelectMaterial = map;
                            checkk = true;
                        }
                    }
                }

                ///出口を実際に生成する
                if (checkk)
                {
                    MapBaseScriptVer2 GeneratedMap = Instantiate(SelectMaterial.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
                    GeneratedMaps.Add(GeneratedMap); // 生成されたマップを追跡
                    Transform LoopHoleVec = transform;
                    foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes) holee.CenterToThisLoopHole = (GeneratedMap.transform.position - holee.transform.position);
                    foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes)
                    {
                        if (MatchMapNum(TrueNum, holee.num))
                        {
                            LoopHoleVec = holee.transform;
                            GeneratedMap.Loopholes.Remove(holee);
                            GeneratedMap.MapInfo.Neighbor[ReverseNum(TrueNum)] = 0;
                            break;
                        }
                    }

                    GeneratedMap.SaveMapNumbers(MapNumbers, MapNumber, ReverseNum(TrueNum));
                    GeneratedMap.MapNumber = MapNumber;

                    GeneratedMap.CenterToStart = GeneratedMap.transform.position - LoopHoleVec.position;
                    LoopHoleVec.position = SaveVec;
                    Debug.Log($"SaveVec:{SaveVec}");
                    GeneratedMap.transform.position = SaveVec + GeneratedMap.CenterToStart;
                    LoopHoleVec.position = SaveVec;
                    //ここでハズレ出口は生成完了
                    //次に別のハズレ出口を正解にする
                    if (Fails.Count == 0)
                    {
                        Debug.Log("No Mini Exit");
                        break;
                    }
                    else
                    {
                        FailLoophoolClass fail = Fails[UnityEngine.Random.Range(0, Fails.Count)];
                        Fails.Remove(fail);
                        SaveVec = fail.ExitPos;
                        TrueNum = fail.ExitNum;
                        foreach (MapExitClass exit in fail.BaseMap.MapInfo.MapExits)
                        {
                            if (exit.ExitNum == fail.ExitNum && exit.fromNum == fail.BaseMap.TrueNum)
                            {
                                MapNumber = fail.BaseMap.MapNumber + exit.ExitVec;
                            }
                        }

                        Debug.LogWarning("True Map Changed!!");
                        AllMapCantSet = false;
                        SelectMaterial = null;
                        //MapCount=0;
                        continue;
                    }
                }
                else
                {
                    Debug.Log("Cant Set Mini Exit");
                    break;
                }
            }

            //生成し始める
            if (SelectMaterial.CheckNotCoverMap(MapNumbers, MapNumber, ReverseNum(TrueNum), Fails))
            {
                //生成する
                MapBaseScriptVer2 GeneratedMap = Instantiate(SelectMaterial.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
                GeneratedMaps.Add(GeneratedMap); // 生成されたマップを追跡
                //チェックしたが、実際どの抜け道が合うか選ぶ
                Transform LoopHoleVec = transform;
                foreach (MapLoopholeVer2 hole in GeneratedMap.Loopholes) hole.CenterToThisLoopHole = (GeneratedMap.transform.position - hole.transform.position);
                foreach (MapLoopholeVer2 hole in GeneratedMap.Loopholes)
                {
                    if (MatchMapNum(TrueNum, hole.num))
                    {
                        LoopHoleVec = hole.transform;
                        //Debug.Log($"Found:{LoopHoleVec.name}");
                        //選ばれたやつはリストと隣情報から消さないと
                        GeneratedMap.Loopholes.Remove(hole);
                        GeneratedMap.MapInfo.Neighbor[ReverseNum(TrueNum)] = 0;
                        break;
                    }
                }

                //マップサイズを保存する
                GeneratedMap.SaveMapNumbers(MapNumbers, MapNumber, ReverseNum(TrueNum));
                //入口の番号を保存しておく
                GeneratedMap.MapNumber = MapNumber;

                //中心から入り口までの距離を図る
                GeneratedMap.CenterToStart = GeneratedMap.transform.position - LoopHoleVec.position;
                //入口のポイントを噛み合う位置においておく
                LoopHoleVec.position = SaveVec;
                // Debug.Log($"SaveVec:{SaveVec}");
                //中心は、今いる入口のところに計算した距離分おいてあげれば設置可能
                GeneratedMap.transform.position = SaveVec + GeneratedMap.CenterToStart;
                //もとに戻してあげる。
                LoopHoleVec.position = SaveVec;



                SaveVec = GeneratedMap.SelectTrueLoophole(MapNumbers, MapNumber, Fails).position;

                //出口によってMapNumberの値を変化させる
                TrueNum = GeneratedMap.TrueNum;
                int EnterNum = GeneratedMap.EnterNum;
                MapNumber = RematchMapNumber(TrueNum, EnterNum, MapNumber, GeneratedMap);

                //敵の生成位置の保存 - マップタイプに基づいて判断
                if (GeneratedMap.ShouldSpawnEnemies() && GeneratedMap.EnemySpawnPoses.Count != 0)
                {
                    foreach (EnemySpawnClass trans in GeneratedMap.EnemySpawnPoses) EnemySpawns.Add(trans);
                }

                //宝箱の生成位置の保存
                if (GeneratedMap.TreasureSpawnPoses.Count != 0)
                {
                    foreach (Transform trans in GeneratedMap.TreasureSpawnPoses) TreasureSpawns.Add(trans);
                }

                // Debug.Log($"TrueNum:{TrueNum}");
                MapCount--;
                SelectMaterial = null;

                if (SpecificMapInfo.CheckSelectSpecific())
                {
                    if (SpecificMapInfo.IsChallenging) SpecificMapInfo.ChallengeMapCounts--;
                    SpecificMapInfo.ResetBools();
                }
            }
            else
            {
                Debug.Log($"MapNumber:{MapNumber} had already set!");
                break;
            }
        }

        //ゴールの生成
        //ただし、４方向すべてにゴールを作らないといけない。
        List<MapBaseScriptVer2> AllEnds=new List<MapBaseScriptVer2>(Ends);
        MapBaseScriptVer2 EndMaterial=null;
        foreach (MapBaseScriptVer2 end in AllEnds){
            foreach(MapLoopholeVer2 hole in end.Loopholes){
                if(hole.num==ReverseNum(TrueNum)){
                    EndMaterial=end;
                }
            }
        }

        MapBaseScriptVer2 GeneratedMap2=Instantiate(EndMaterial.gameObject,MapParent).GetComponent<MapBaseScriptVer2>();
        GeneratedMaps.Add(GeneratedMap2); // 生成されたマップを追跡
        Transform LoopHoleVec2=transform;
        foreach(MapLoopholeVer2 holee in GeneratedMap2.Loopholes)holee.CenterToThisLoopHole=(GeneratedMap2.transform.position-holee.transform.position);
        foreach(MapLoopholeVer2 holee in GeneratedMap2.Loopholes){
            if(MatchMapNum(TrueNum,holee.num)){
                LoopHoleVec2=holee.transform;
                GeneratedMap2.Loopholes.Remove(holee);
                GeneratedMap2.MapInfo.Neighbor[ReverseNum(TrueNum)]=0;
                break;
            }
        }

        GeneratedMap2.SaveMapNumbers(MapNumbers,MapNumber,ReverseNum(TrueNum));
        GeneratedMap2.MapNumber=MapNumber;

        GeneratedMap2.CenterToStart = GeneratedMap2.transform.position - LoopHoleVec2.position;
        LoopHoleVec2.position = SaveVec;
        
        GeneratedMap2.transform.position = SaveVec + GeneratedMap2.CenterToStart;
        LoopHoleVec2.position = SaveVec;

        //Instantiate(EndArea,GeneratedMap2.EndSpawnPos);

        //あとはハズレの道の生成を行う。
        //ハズレの行き止まり用のfailも作っておく
        EndFails=new List<FailLoophoolClass>();

        int count=Fails.Count;
        int failEndExitNum = 0;
        while (count > 0)
        {//ここ二回目の生成！！！！！
            FailLoophoolClass fail = Fails[0];
            Fails.RemoveAt(0);
            MapBaseScriptVer2 SelectMaterial = null;
            int ran = UnityEngine.Random.Range(MiniMapCountMin, MiniMapCountMax);
            TrueNum = fail.ExitNum;

            //はずれ用のmapVectorとしてfailMapVector
            Vector3 failMapNumber = fail.BaseMap.MapNumber;
            //Debug.LogWarning($"newVEEEC:{failMapNumber}");
            foreach (MapExitClass exit in fail.BaseMap.MapInfo.MapExits)
            {
                if (exit.ExitNum == fail.ExitNum && exit.fromNum == fail.BaseMap.EnterNum)
                {
                    failMapNumber += exit.ExitVec;
                    break;
                }
            }
            //Debug.LogWarning($"FailVEEEC:{failMapNumber}");
            //位置としてfailSaveVecを立てている
            Vector3 failSaveVec = fail.ExitPos;

            bool AllMapCantSet = false;
            while (ran > 0)
            {
                List<MapBaseScriptVer2> CopyMaterials = new List<MapBaseScriptVer2>(MapMaterials);
                while (true)
                {
                    //まずは作るマップを選ぶ
                    if (CopyMaterials.Count == 0)
                    {
                        //生成が完全にうまくいかなかったらハズレの出口として生成して別のハズレを新規ルートと採用する。  
                        Debug.Log("All Map cant set");
                        AllMapCantSet = true;
                        break;
                    }
                    SelectMaterial = CopyMaterials[UnityEngine.Random.Range(0, CopyMaterials.Count)];

                    //作るマップが今あるマップと混じらないかはあとにする
                    //とりあえずは番号が一致するかだけで判断する
                    bool check = false;
                    //選ばれたやつの抜け道が今のやつとかみあいそうか調べる
                    foreach (MapLoopholeVer2 hole in SelectMaterial.Loopholes)
                    {
                        if (MatchMapNum(TrueNum, hole.num))
                        {
                            //番号がかみ合いそうならokってことで
                            //Debug.Log($"Selected:{SelectMaterial}");
                            check = true;
                            break;
                        }
                    }
                    if (!check)
                    {
                        // Debug.Log("NotFound");
                        CopyMaterials.Remove(SelectMaterial);
                        continue;
                    }

                    if (SelectMaterial.CheckNotCoverMap(MapNumbers, failMapNumber, ReverseNum(TrueNum), Fails))
                    {
                        //Debug.Log($"NotCover:{SelectMaterial}");
                        if (SelectMaterial.CheckCanSetNextPoint(MapNumbers, failMapNumber, ReverseNum(TrueNum), Fails))
                        {
                            //両方満たしているときでしか抜け出せない
                            // Debug.Log($"CompletelySelected:{SelectMaterial}");
                            break;
                        }
                        else
                        {
                            // Debug.Log($"Cant set {SelectMaterial}[CanSetNextPoint]");
                            CopyMaterials.Remove(SelectMaterial);
                        }

                    }
                    else
                    {
                        //Debug.Log($"Cant set {SelectMaterial}[NotCoverMap]");
                        CopyMaterials.Remove(SelectMaterial);
                    }
                }



                if (AllMapCantSet)
                {

                    bool checkk = false;
                    CopyMiniEnds = new List<MapBaseScriptVer2>(MiniEnds);
                    foreach (MapBaseScriptVer2 map in CopyMiniEnds)
                    {
                        foreach (MapLoopholeVer2 hole in map.Loopholes)
                        {
                            if (hole.num == ReverseNum(TrueNum))
                            {
                                //Debug.Log($"Mini End {map.name}");
                                SelectMaterial = map;
                                checkk = true;
                            }
                        }
                    }
                    //ここいじる必要あり！！！
                    if (checkk)
                    {
                        MapBaseScriptVer2 GeneratedMap = Instantiate(SelectMaterial.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
                        GeneratedMaps.Add(GeneratedMap); // 生成されたマップを追跡
                        Transform LoopHoleVec = transform;
                        foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes) holee.CenterToThisLoopHole = (GeneratedMap.transform.position - holee.transform.position);
                        foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes)
                        {
                            if (MatchMapNum(TrueNum, holee.num))
                            {
                                LoopHoleVec = holee.transform;
                                GeneratedMap.Loopholes.Remove(holee);
                                GeneratedMap.MapInfo.Neighbor[ReverseNum(TrueNum)] = 0;
                                break;
                            }
                        }
                        //出口の生成？

                        GeneratedMap.SaveMapNumbers(MapNumbers, failMapNumber, ReverseNum(TrueNum));
                        GeneratedMap.MapNumber = failMapNumber;

                        GeneratedMap.CenterToStart = GeneratedMap.transform.position - LoopHoleVec.position;
                        LoopHoleVec.position = failSaveVec;
                        //Debug.Log($"SaveVec:{SaveVec}");

                        GeneratedMap.transform.position = failSaveVec + GeneratedMap.CenterToStart;
                        LoopHoleVec.position = failSaveVec;
                        //Debug.Log($"出口の生成1{GeneratedMap.name},{GeneratedMap.transform.position}");
                        ran = 0;
                        //敵の生成位置を保存
                        if (GeneratedMap.TreasureSpawnPoses.Count != 0)
                        {
                            foreach (Transform trans in GeneratedMap.TreasureSpawnPoses) TreasureSpawns.Add(trans);
                        }
                        break;

                    }
                    else
                    {
                        Debug.Log("Cant Set Mini Exit");
                        break;
                    }
                }
                else
                {
                    //普通の生成
                    MapBaseScriptVer2 GeneratedMapFail = Instantiate(SelectMaterial.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
                    GeneratedMaps.Add(GeneratedMapFail); // 生成されたマップを追跡
                    Transform LoopHoleVec = transform;
                    foreach (MapLoopholeVer2 hole in GeneratedMapFail.Loopholes) hole.CenterToThisLoopHole = (GeneratedMapFail.transform.position - hole.transform.position);
                    foreach (MapLoopholeVer2 hole in GeneratedMapFail.Loopholes)
                    {
                        if (MatchMapNum(TrueNum, hole.num))
                        {
                            LoopHoleVec = hole.transform;
                            GeneratedMapFail.Loopholes.Remove(hole);
                            GeneratedMapFail.MapInfo.Neighbor[ReverseNum(TrueNum)] = 0;
                            break;
                        }
                    }

                    GeneratedMapFail.SaveMapNumbers(MapNumbers, failMapNumber, ReverseNum(TrueNum));
                    GeneratedMapFail.MapNumber = failMapNumber;
                    GeneratedMapFail.CenterToStart = GeneratedMapFail.transform.position - LoopHoleVec.position;
                    LoopHoleVec.position = failSaveVec;
                    //Debug.Log($"SaveVec:{SaveVec}");
                    GeneratedMapFail.transform.position = failSaveVec + GeneratedMapFail.CenterToStart;
                    LoopHoleVec.position = failSaveVec;

                    failSaveVec = GeneratedMapFail.SelectTrueLoophole(MapNumbers, failMapNumber, EndFails).position;
                    // Debug.Log($"FailSaveVec:{failSaveVec}");
                    TrueNum = GeneratedMapFail.TrueNum;
                    int EnterNum = GeneratedMapFail.EnterNum;
                    //正しい道にはいけているが、ハズレの道には正しくMapNumberが適応されていない。failExitVecで他の出口にも適応されるように設定する。
                    Vector3 failExitMapNumber = failMapNumber;
                    failMapNumber = RematchMapNumber(TrueNum, EnterNum, failMapNumber, GeneratedMapFail);
                    // マップタイプに基づいて敵生成位置を収集
                    if (GeneratedMapFail.ShouldSpawnEnemies() && GeneratedMapFail.EnemySpawnPoses.Count != 0)
                    {
                        foreach (EnemySpawnClass trans in GeneratedMapFail.EnemySpawnPoses) EnemySpawns.Add(trans);
                    }
                    //宝箱の生成位置の保存
                    if (GeneratedMapFail.TreasureSpawnPoses.Count != 0)
                    {
                        foreach (Transform trans in GeneratedMapFail.TreasureSpawnPoses) TreasureSpawns.Add(trans);
                    }

                    //Debug.Log($"TrueNum:{TrueNum}");
                    ran--;
                    SelectMaterial = null;


                    if (EndFails.Count > 0)
                    {
                        //道分岐がでてきたらすぐ片方を行き止まりにする
                        //MapNumberの設定がうまくできていない
                        foreach (var failclass in EndFails)
                        {
                            
                            //ハズレ出口の番号
                            failEndExitNum = failclass.ExitNum;
                            //ハズレ出口の元のマップの入口番号
                            int failEndEnterNum = failclass.BaseMap.EnterNum;
                            Vector3 failEndVec = failclass.ExitPos;
                            CopyMiniEnds = new List<MapBaseScriptVer2>(MiniEnds);
                            bool checkk = false;
                            foreach (MapBaseScriptVer2 map in CopyMiniEnds)
                            {
                                foreach (MapLoopholeVer2 hole in map.Loopholes)
                                {
                                    if (hole.num == ReverseNum(failEndExitNum))
                                    {
                                        // Debug.LogWarning($"Mini End@as@fjogj {failEndExitNum}");
                                        SelectMaterial = map;
                                        checkk = true;
                                        break;
                                    }
                                    if (checkk) break;
                                }
                            }

                            if (checkk)
                            {
                                // Debug.Log($"failEndEnterNum:{failEndEnterNum}, failEndExitNum:{failEndExitNum}");
                                Vector3 tmpMapNumber= RematchMapNumber(failEndExitNum,failEndEnterNum,failExitMapNumber, GeneratedMapFail);
                                // Debug.Log($"tmpMapNumber:{tmpMapNumber}");
                                MapBaseScriptVer2 GeneratedMap = Instantiate(SelectMaterial.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
                                GeneratedMaps.Add(GeneratedMap); // 生成されたマップを追跡
                                Transform LoopHoleVect = transform;
                                foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes) holee.CenterToThisLoopHole = (GeneratedMap.transform.position - holee.transform.position);
                                foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes)
                                {
                                    if (MatchMapNum(failEndExitNum, holee.num))
                                    {
                                        LoopHoleVect = holee.transform;
                                        GeneratedMap.Loopholes.Remove(holee);
                                        GeneratedMap.MapInfo.Neighbor[ReverseNum(failEndExitNum)] = 0;
                                        break;
                                    }
                                }
                                // Debug.Log($"FailMapNumber:{failMapNumber},MapNum,{failEndExitNum}");
                                GeneratedMap.SaveMapNumbers(MapNumbers, tmpMapNumber, ReverseNum(failEndExitNum));
                                GeneratedMap.MapNumber = failMapNumber;

                                GeneratedMap.CenterToStart = GeneratedMap.transform.position - LoopHoleVect.position;
                                LoopHoleVect.position = failEndVec;

                                GeneratedMap.transform.position = failEndVec + GeneratedMap.CenterToStart;
                                LoopHoleVect.position = failEndVec;
                                // Debug.Log($"出口の生成3{GeneratedMap.name},{GeneratedMap.transform.position}");
                                if (GeneratedMap.TreasureSpawnPoses.Count != 0)
                                {
                                    foreach (Transform trans in GeneratedMap.TreasureSpawnPoses) TreasureSpawns.Add(trans);
                                }

                            }
                            else
                            {
                                Debug.LogError("WHY!!!!!!!");
                            }
                        }
                    }

                    EndFails.Clear();

                }

                if (ran == 0)
                {
                    //終わりを作る
                    // Debug.Log("終わりを生成");
                    CopyMiniEnds = new List<MapBaseScriptVer2>(MiniEnds);
                    bool checkk = false;
                    foreach (MapBaseScriptVer2 map in CopyMiniEnds)
                    {
                        foreach (MapLoopholeVer2 hole in map.Loopholes)
                        {
                            if (hole.num == ReverseNum(TrueNum))
                            {
                                //Debug.Log($"Mini End:{map.name}");
                                SelectMaterial = map;
                                checkk = true;
                            }
                        }
                    }

                    if (checkk)
                    {
                        MapBaseScriptVer2 GeneratedMap = Instantiate(SelectMaterial.gameObject, MapParent).GetComponent<MapBaseScriptVer2>();
                        GeneratedMaps.Add(GeneratedMap); // 生成されたマップを追跡
                        Transform LoopHoleVect = transform;
                        foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes) holee.CenterToThisLoopHole = (GeneratedMap.transform.position - holee.transform.position);
                        foreach (MapLoopholeVer2 holee in GeneratedMap.Loopholes)
                        {
                            if (MatchMapNum(TrueNum, holee.num))
                            {
                                LoopHoleVect = holee.transform;
                                GeneratedMap.Loopholes.Remove(holee);
                                GeneratedMap.MapInfo.Neighbor[ReverseNum(TrueNum)] = 0;
                                // Debug.Log($";zhdg;ozxdg;hse {TrueNum},{holee.num}");
                                break;
                            }
                        // Debug.Log($";zhdg;ozxdg;hse {TrueNum},{holee.num}");
                        }
                        
                        GeneratedMap.SaveMapNumbers(MapNumbers, failMapNumber, ReverseNum(TrueNum));
                        GeneratedMap.MapNumber = failMapNumber;
                        // Debug.Log($"vecvec:{GeneratedMap.transform.position - LoopHoleVect.position},loop:{LoopHoleVect.position}");
                        GeneratedMap.CenterToStart = GeneratedMap.transform.position - LoopHoleVect.position;
                        LoopHoleVect.position = failSaveVec;

                        GeneratedMap.transform.position = failSaveVec + GeneratedMap.CenterToStart;

                        // Debug.Log($"FFFFFFFailMapNumber:{failMapNumber},MapNum,{TrueNum},pos:{GeneratedMap.transform.position}");
                        LoopHoleVect.position = failSaveVec;
                        if (GeneratedMap.TreasureSpawnPoses.Count != 0)
                        {
                            foreach (Transform trans in GeneratedMap.TreasureSpawnPoses) TreasureSpawns.Add(trans);
                        }

                    }
                }
            }
            count--;
        }

        NavMesh.BuildNavMesh();
        //敵の生成
        //細かくばらまくのは今後にして、とりあえず生成してほしい箇所に敵を生成させる

        foreach (EnemySpawnClass cls in EnemySpawns)
        {
            for (int i = 0; i < cls.Counts; i++)
            {
                Instantiate(Enemys[UnityEngine.Random.Range(0, Enemys.Count)], cls.Trans);
                SceneManagerScript.instance.maxEnemyCount++;
            }

            if(EliteEnemys.Count > 0) if(cls.isMiddle) Instantiate(EliteEnemys[UnityEngine.Random.Range(0, EliteEnemys.Count)], cls.Trans);
        }

        //宝箱の生成
        foreach(Transform trans in TreasureSpawns){
            Instantiate(treasurePrefab,trans);
        }
        yield return null;

        // マップ生成統計をログに出力
        LogMapGenerationStats();

        //始めるぞ
        
        SceneManagerScript.instance.StartStage(this);
    }

    // Update is called once per frame

    Vector3 RematchMapNumber(int TrueNum,int EnterNum,Vector3 MapNumber,MapBaseScriptVer2 MapBase){
        //Debug.Log($"Rematching:{MapNumber},{TrueNum}");
        foreach(MapExitClass exitClass in MapBase.MapInfo.MapExits){
            if((exitClass.ExitNum==0||TrueNum==exitClass.ExitNum) && EnterNum==exitClass.fromNum){
                // Debug.Log($"{MapBase.name},{MapBase.transform.position},{MapNumber + exitClass.ExitVec} FOUND!!!!!!!!!!!!!!!!");
                return MapNumber + exitClass.ExitVec;
            } 
        }

        return new Vector3(0,0,0);

    }

    bool MatchMapNum(int TrueNum,int SelectedNum){
        if(TrueNum==5 && SelectedNum==3) return true;
        else if(TrueNum==3 && SelectedNum==5) return true;
        else if(TrueNum==1&&SelectedNum==7) return true;
        else if(TrueNum==7&&SelectedNum==1) return true;
        else {
            //Debug.LogError("Not Match MapNumber!!");
            return false;
        }
    }

    int ReverseNum(int i){
        if(i==5) return 3;
        else if(i==3) return 5;
        else if(i==1) return 7;
        else if(i==7) return 1;
        else {
            Debug.Log("ReverseNum Not Found");
            return 0;
        }
    }


#if UNITY_EDITOR
    /// <summary>
    /// 生成されたマップの統計情報を取得する
    /// </summary>
    public MapGenerationStats GetMapGenerationStats()
    {
        MapGenerationStats stats = new MapGenerationStats();

        if (GeneratedMaps == null) return stats;

        foreach (MapBaseScriptVer2 map in GeneratedMaps)
        {
            stats.TotalMaps++;

            switch (map.mapType)
            {
                case MapType.Normal:
                    stats.NormalMaps++;
                    break;
                case MapType.Safe:
                    stats.SafeMaps++;
                    break;
                case MapType.Challenge:
                    stats.ChallengeMaps++;
                    break;
                case MapType.Treasure:
                    stats.TreasureMaps++;
                    break;
            }

            if (map.ShouldSpawnEnemies())
            {
                stats.MapsWithEnemies++;
            }
            else
            {
                stats.MapsWithoutEnemies++;
            }
        }

        return stats;
    }

    /// <summary>
    /// マップ生成統計をログに出力する
    /// </summary>
    public void LogMapGenerationStats()
    {
        MapGenerationStats stats = GetMapGenerationStats();
        Debug.Log($"=== マップ生成統計 ===");
        Debug.Log($"総マップ数: {stats.TotalMaps}");
        Debug.Log($"通常マップ: {stats.NormalMaps}");
        Debug.Log($"安全マップ: {stats.SafeMaps}");
        Debug.Log($"チャレンジマップ: {stats.ChallengeMaps}");
        Debug.Log($"宝箱マップ: {stats.TreasureMaps}");
        Debug.Log($"敵生成あり: {stats.MapsWithEnemies}");
        Debug.Log($"敵生成なし: {stats.MapsWithoutEnemies}");
    }
#endif
}

[System.Serializable]
public class FailLoophoolClass{
    public int ExitNum;
    public MapBaseScriptVer2 BaseMap;
    public Vector3 ExitPos;
}

[System.Serializable]
public class EnemySpawnClass{
    public int Counts;
    public bool isMiddle;
    public Transform Trans;
}

[System.Serializable]
public class SpecificMapInfo{
    //生成される確率? 初期設定は50にしておく
    public int randomer = 50;

    // 強化された敵を倒したら報酬が得られるマップの生成数
    public int ChallengeMapCounts;
    public bool IsChallenging;

    // 保管庫マップの生成数
    public int StorageMapCounts;
    public bool IsStoraging;

    //鍵扉の生成数
    public int LockedTreasureMapCounts;
    public bool IsLocked;

    // 残りの生成数を調べる 
    public int Sum(){
        return ChallengeMapCounts + StorageMapCounts + LockedTreasureMapCounts;
    }

    public bool CheckSelectSpecific(){
        return IsChallenging || IsStoraging || IsLocked;
    }

    public void ResetBools(){
        IsChallenging = false;
        IsStoraging = false;
        IsLocked = false;
    }

}

