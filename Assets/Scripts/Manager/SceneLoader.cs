using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set;}
    public bool isBreaking;
    public SceneObject PlayerScene;
    public SceneObject targetScene;
    public int SceneID;
    public List<SceneNameToID> ScenePairs;
    [SerializeField]string _filePath;
    [SerializeField]string _fileName = "PlayerData";

    private void Awake()
    {
        staticScript._dataNumber = 1;
        _fileName = _fileName + staticScript._dataNumber.ToString() + ".json";
        _filePath = Application.dataPath + "/" + _fileName;
    }

    PlayerSaveData LoadData()
    {
        if (!File.Exists(_filePath)) return null;
        StreamReader rd = new StreamReader(_filePath);
        string json = rd.ReadToEnd();
        rd.Close();

        return JsonUtility.FromJson<PlayerSaveData>(json);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
        if (staticScript._hasData)
        {
            SceneLoad(LoadData()._sceneID);
        }
        else
        {
            SceneLoad();
        }
    }
    public int SelectScene(int i = -1){
        //ここのreturnはbreakingのやつ
        int target=-1;

        //指定なし　→　SceneObjectからロードするならば
        if (i == -1)
        {
            if (isBreaking) return 4;
            foreach (SceneNameToID pair in ScenePairs)
            {
                if (pair.Scene.m_SceneName == targetScene.m_SceneName)
                {
                    target = pair.sceneID;
                    SceneID = target;
                    break;
                }
            }
        }
        else
        {
            return i;
        }
        
        return target;
    }

    public void SetnextSceneID(SceneObject obj)
    {
        foreach(SceneNameToID pair in ScenePairs){
            if(pair.Scene.m_SceneName==targetScene.m_SceneName){
                SceneID=pair.sceneID;
                break;
            }
        }
    }

    public int SearchSceneNumber(SceneObject ob)
    {
        int result = -1;
        //ここに休憩所の番号入る
        if (isBreaking) return 4;
        foreach (SceneNameToID pair in ScenePairs)
        {
            if (ob.m_SceneName == pair.Scene.m_SceneName)
            {
                result = pair.sceneID;
                SceneID = result;
            }
        }
        return result;
    } 

    public void SceneLoad(int i=-1){
        SceneManager.LoadSceneAsync(SelectScene(i),LoadSceneMode.Additive);
        //ステージ移動のboolをfalseにしている
        SceneManagerScript.instance.Finishing=false;
    }
    public void SceneLoadNumber(int num){
        SceneManager.LoadSceneAsync(num,LoadSceneMode.Additive);
        //ステージ移動のboolをfalseにしている
        SceneManagerScript.instance.Finishing=false;
    }
    public void SceneUnLoad(SceneObject ob)
    {
        SceneManager.UnloadSceneAsync(SearchSceneNumber(ob));
    }

}

[System.Serializable]
public class SceneNameToID{
    public SceneObject Scene;
    public int sceneID;
}
