using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// マップのタイプを定義する列挙型
public enum MapType
{
    Normal,          // 通常マップ（敵生成あり）
    Safe,           // 安全マップ（敵生成なし）
    Challenge,      // チャレンジマップ（強敵あり）
    Treasure        // 宝箱マップ（敵生成なし、宝箱のみ）
}

public class MapBaseScriptVer2 : MonoBehaviour
{
    public NeighborClass MapInfo;

    //スタートとゴールを設定していたが、それだとマップ生成に制限がかかるため
    //抜けれる箇所をリスト化しておく
    //public Transform StartPos;
    //public Transform EndPos;

    [Header("マップタイプ設定")]
    public MapType mapType = MapType.Normal;

    public List<MapLoopholeVer2> Loopholes;
    public List<EnemySpawnClass> EnemySpawnPoses;
    public List<Transform> TreasureSpawnPoses;
    //かわりにスタートマップのスポーン地点としてStartPosを採用
    public Transform StartSpawnPos;
    public Transform EndSpawnPos;
    public Vector3 CenterToStart;
    Vector3 CenterToEnd;
    public int TrueNum;
    public int EnterNum;
    public Vector3 MapVec;
    public Vector3 MapNumber;
    public List<int> Intss;
    
    
    public void SaveMapNumbers(List<Vector3> Lists,Vector3 BaseVec,int TrueNum){
        EnterNum=TrueNum;
        MapVec=BaseVec;
        int count=0;
        //番号の判別、どうする？
        MapScaleClass BaseClass=null;
        foreach(MapScaleClass MapClass in MapInfo.MapScale){
            if(MapClass.LoopholeNumber==TrueNum){
                BaseClass=MapClass;

                foreach(MapLoopHoleClass MapClassChild in BaseClass.Classes){
                    //これ、マップの生成番号とスケールの規模を設定しないと正しく動作しない
                    //なので、番号ごとにマップスケールを設定するようにしよう。値は絶対値。
                    //forではかけないのでwhileで。
                    int y=0;
                    while(Mathf.Abs(y)<=Mathf.Abs(MapClassChild.Scale.y)){
                        int z=0;
                        while(Mathf.Abs(z)<=Mathf.Abs(MapClassChild.Scale.z)){
                            int x=0;
                            while(Mathf.Abs(x)<=Mathf.Abs(MapClassChild.Scale.x)){
                                if(!Lists.Contains(MapVec+new Vector3(x,y,z)))Lists.Add(MapVec+new Vector3(x,y,z));
                                else Debug.Log($"Coveringinging");

                                if(MapClassChild.Scale.x>0) x++;
                                else x--;
                            }
                            if(MapClassChild.Scale.z>0)z++;
                            else z--;
                        }
                        if(MapClassChild.Scale.y>0) y++;
                        else y--;
                    }
                    //Debug.Log($"Base:{MapVec},Add:{BaseClass.Classes[count].EndVec},Object:{gameObject.name}");
                    MapVec+=BaseClass.Classes[count].EndVec;
                    count++;

                    //出口が複数あるときをわけてみたかったが、ここではあまり意味ないかも？
                    //必要なのは番号を振る時かも。
                }

            }
        }

        //入口によって、マップの保存の仕方も変えてあげないといけない
        

        //これ各出口に結果を変えてあげないといけない...

    }

    public bool CheckNotCoverMap(List<Vector3> Lists,Vector3 BaseVec,int TrueNum,List<FailLoophoolClass> fails)
    {
        bool check=false;
        MapVec=BaseVec;
        int count=0;
        //番号の判別、どうする？
        MapScaleClass BaseClass=null;
        foreach(MapScaleClass MapClass in MapInfo.MapScale){
            if(MapClass.LoopholeNumber==TrueNum){
                BaseClass=MapClass;
            }
        }
        if(BaseClass==null){
            Debug.Log("Not Match");
            return false;
        }

        List<Vector3> failVecs=new List<Vector3>();
        foreach(FailLoophoolClass fail in fails){
            Vector3 checkVec=fail.BaseMap.MapNumber;
            foreach(MapExitClass exit in fail.BaseMap.MapInfo.MapExits){
                if(exit.ExitNum==fail.ExitNum && exit.fromNum==fail.BaseMap.EnterNum){
                    switch(exit.ExitNum){
                        case 1:
                            checkVec+=exit.ExitVec;
                            failVecs.Add(checkVec + new Vector3(0,0,0));
                            failVecs.Add(checkVec + new Vector3(0,0,1));
                            failVecs.Add(checkVec + new Vector3(1,0,0));
                            failVecs.Add(checkVec + new Vector3(-1,0,0));
                            break;
                        case 3:
                            checkVec+=exit.ExitVec;
                           // Debug.Log($"ExitVec:{exit.ExitVec},AddedVec3:{Vec}");
                            failVecs.Add(checkVec + new Vector3(0,0,0));
                            failVecs.Add(checkVec + new Vector3(-1,0,0));
                            failVecs.Add(checkVec + new Vector3(0,0,1));
                            failVecs.Add(checkVec + new Vector3(0,0,-1));
                            break;
                        case 5:
                            checkVec+=exit.ExitVec;
                            //Debug.Log($"ExitVec:{exit.ExitVec},AddedVec5:{Vec}");
                            failVecs.Add(checkVec + new Vector3(0,0,0));
                            failVecs.Add(checkVec + new Vector3(1,0,0));
                            failVecs.Add(checkVec + new Vector3(0,0,1));
                            failVecs.Add(checkVec + new Vector3(0,0,-1));
                            break;
                        case 7:
                            checkVec+=exit.ExitVec;
                            //Debug.Log($"ExitVec:{exit.ExitVec},AddedVec7:{Vec}");
                            failVecs.Add(checkVec + new Vector3(0,0,0));
                            failVecs.Add(checkVec + new Vector3(1,0,0));
                            failVecs.Add(checkVec + new Vector3(-1,0,0));
                            failVecs.Add(checkVec + new Vector3(0,0,-1));
                            break;
                    }
                }
            }
        }

        foreach(MapLoopHoleClass MapClass in BaseClass.Classes){
            //これ、マップの生成番号とスケールの規模を設定しないと正しく動作しない
            //なので、番号ごとにマップスケールを設定するようにしよう。値は絶対値。
            //forではかけないのでwhileで。
            int y=0;
            while(Mathf.Abs(y)<=Mathf.Abs(MapClass.Scale.y)){
                int z=0;
                while(Mathf.Abs(z)<=Mathf.Abs(MapClass.Scale.z)){
                    int x=0;
                    while(Mathf.Abs(x)<=Mathf.Abs(MapClass.Scale.x)){
                        //ここに番号ごとに出口がどれくらい空いているのかを調べたいね
                        if(Lists.Contains(MapVec+new Vector3(x,y,z))){
                            check=true;
                            //Debug.Log($"{MapVec+new Vector3(x,y,z)} already set!");
                        }
                        if(failVecs.Contains(MapVec+new Vector3(x,y,z))){
                            check=true;
                            //Debug.Log($"{MapVec+new Vector3(x,y,z)} already set in fail!");
                        }

                        if(MapClass.Scale.x>0) x++;
                        else x--;
                    }
                    if(MapClass.Scale.z>0) z++;
                    else z--;
                }
                if(MapClass.Scale.y>0) y++;
                else y--;
            }

            MapVec+=BaseClass.Classes[count].EndVec;
            count++;
        }

        

        return !(check);
    }

　  //選ばれたマップにて、ほかの出口がかぶさらなければ
    public bool CheckCanSetNextPoint(List<Vector3> Lists,Vector3 BaseVec,int TrueNum,List<FailLoophoolClass> fails){
        bool check=true;
        foreach(MapExitClass mapExit in MapInfo.MapExits){
            //選ばれた出口だけはのぞく
            //除いた上で、fromが入口であることを踏まえて
            //Debug.Log("ExitNum:" + (mapExit.ExitNum!=TrueNum));
            //Debug.Log("fromNum:" +(mapExit.fromNum == TrueNum));

            if(mapExit.ExitNum!=TrueNum && mapExit.fromNum == TrueNum){
                //Debug.Log("GO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                switch(mapExit.ExitNum){
                    case 1:
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,1)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(-1,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(1,0,0)))check=false;
                        //Debug.Log($"1 waaa {BaseVec+mapExit.ExitVec}");
                        break;
                    case 3:
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,1)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(-1,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,-1)))check=false;
                        //Debug.Log($"3 waaa {BaseVec+mapExit.ExitVec}");
                        break;
                    case 5:
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,1)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(1,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,-1)))check=false;
                        //Debug.Log($"5 waaa {BaseVec+mapExit.ExitVec}");
                        break;
                    case 7:
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(0,0,-1)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(-1,0,0)))check=false;
                        if(Lists.Contains(BaseVec+mapExit.ExitVec+new Vector3(1,0,0)))check=false;
                        //Debug.Log($"7 waaa {BaseVec+mapExit.ExitVec}");
                        break;
                }
            }
        }

        foreach(FailLoophoolClass fail in fails){
            MapBaseScriptVer2 baseMap=fail.BaseMap;
            Vector3 Vec=Vector3.zero;
            Vec+=baseMap.MapNumber;
            //Debug.Log($"Vec:{Vec}");
            foreach(MapExitClass exit in fail.BaseMap.MapInfo.MapExits){
                if(exit.ExitNum==fail.ExitNum && exit.fromNum==fail.BaseMap.EnterNum){
                    switch(exit.ExitNum){
                        case 1:
                            Vec+=exit.ExitVec;
                            //Debug.Log($"ExitVec:{exit.ExitVec},AddedVec1:{Vec}");
                            if(Lists.Contains(Vec + new Vector3(0,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(0,0,1)))check=false;
                            if(Lists.Contains(Vec + new Vector3(1,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(-1,0,0)))check=false;
                            break;
                        case 3:
                            Vec+=exit.ExitVec;
                            //Debug.Log($"ExitVec:{exit.ExitVec},AddedVec3:{Vec}");
                            if(Lists.Contains(Vec + new Vector3(0,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(-1,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(0,0,1)))check=false;
                            if(Lists.Contains(Vec + new Vector3(0,0,-1)))check=false;
                            break;
                        case 5:
                            Vec+=exit.ExitVec;
                            //Debug.Log($"ExitVec:{exit.ExitVec},AddedVec5:{Vec}");
                            if(Lists.Contains(Vec + new Vector3(0,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(1,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(0,0,1)))check=false;
                            if(Lists.Contains(Vec + new Vector3(0,0,-1)))check=false;
                            break;
                        case 7:
                            Vec+=exit.ExitVec;
                            //Debug.Log($"ExitVec:{exit.ExitVec},AddedVec7:{Vec}");
                            if(Lists.Contains(Vec + new Vector3(0,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(1,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(-1,0,0)))check=false;
                            if(Lists.Contains(Vec + new Vector3(0,0,-1)))check=false;
                            break;
                    }
                }
            }
        }

        
        return check;
    }

    //正しい抜け道をここで選ぶ。
    public Transform SelectTrueLoophole(List<Vector3> MapNumbers,Vector3 MapNumber,List<FailLoophoolClass> fails){
        List<int> ints=new List<int>();
        int count=0;
        Intss=ints;
        //i=1だったらintsにその番号をぶっこむ
        foreach(int i in MapInfo.Neighbor){
            if(i==1) {
                ints.Add(count);
                // Debug.Log("Add:"+count);
            }
            count++;
        }
        bool checkbool=false;
        //intsからランダムに番号を選んで正しい番号にする
        while(true){
            if(ints.Count==0){
                Debug.Log("No Loophole here");
                checkbool=true;
                break;
            }
            //空いている出口の番号をランダムに取り出す
            //ランダムだが、選ばれた出口がある程度すいてなければ削除
            //入口がfromと一致していたら
            TrueNum=ints[UnityEngine.Random.Range(0,ints.Count)];
            MapExitClass SelectExit=null;
            foreach(MapExitClass mapExit in MapInfo.MapExits){
                if(TrueNum==mapExit.ExitNum && EnterNum==mapExit.fromNum){
                    SelectExit=mapExit;
                    break;
                }
            }
            if(SelectExit==null)continue;
            //Debug.Log($"Select:{gameObject.name},MapNumber:{MapNumber},ExitVec:{SelectExit.ExitVec}");
            Vector3 testNumber=MapNumber+SelectExit.ExitVec;
            bool NumberCheck=true;
            switch(TrueNum){
                case 1:
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,1))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(1,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(-1,0,0))) NumberCheck=false;
                    break;
                case 3:
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,1))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(-1,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,-1))) NumberCheck=false;
                    break;
                case 5:
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,1))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,-1))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(1,0,0))) NumberCheck=false;
                    break;
                case 7:
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(1,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(-1,0,0))) NumberCheck=false;
                    if(MapNumbers.Contains(testNumber + new Vector3(0,0,-1))) NumberCheck=false;
                    break;
            }

            if(NumberCheck){
                ints.Remove(TrueNum);
                foreach(int i in ints){
                    FailLoophoolClass failLoophool=new FailLoophoolClass();
                    failLoophool.ExitNum=i;
                    failLoophool.BaseMap=this;
                    foreach(MapLoopholeVer2 hole in Loopholes){
                        if(hole.num==i){
                            failLoophool.ExitPos=hole.transform.position;
                        }
                }

                fails.Add(failLoophool);
                }
                break;
            }else{
                //ハズレの出口は記録
                //Debug.Log("Remove:"+TrueNum);
                ints.Remove(TrueNum);

                
            }
            
        };
        
        if(checkbool) return null;
        foreach(MapLoopholeVer2 Loophole in Loopholes){
            if(Loophole.num==TrueNum){
                return Loophole.transform;
            }
        }
        Debug.Log("Nullllllll");
        return null;

    }


    //抜け道とマップの位置の計算?

    /// <summary>
    /// このマップが敵を生成するかどうかを返す
    /// </summary>
    public bool ShouldSpawnEnemies()
    {
        return mapType == MapType.Normal || mapType == MapType.Challenge;
    }

    /// <summary>
    /// このマップが宝箱を生成するかどうかを返す
    /// </summary>
    public bool ShouldSpawnTreasures()
    {
        return mapType == MapType.Treasure || TreasureSpawnPoses.Count > 0;
    }
}
[System.Serializable]
public class NeighborClass {
    [Header("0が左上、8が右下の3x3で、\n1が道があるイメージ")]
    public int[] Neighbor = new int[9];
    [Header("マップのでかさはn個の長方形が\nどうつながっているかのイメージ\nただし大きさは実際のを各方向-1したやつ\nExitNumが次の長方形への道\n-1ならおしまい")]
    [Header("これ、入口がn番目ならでやってる、方角北が1")]
    public MapScaleClass[] MapScale;
    public MapExitClass[] MapExits;
}

[System.Serializable]
public class MapScaleClass
{
    
    public int LoopholeNumber;
    public MapLoopHoleClass[] Classes;
}

[System.Serializable]
public class MapLoopHoleClass{
    public Vector3 Scale;
    public Vector3 EndVec;
}

[System.Serializable]
public class MapExitClass{
    public int ExitNum;
    public int fromNum;
    public Vector3 ExitVec;
}