using UnityEngine;

public static class staticScript
{
    public static int _dataNumber;
    public static bool _hasData;
    static bool isTutorial;
    public static float CameraPos = 1;
    public static float CanvasRotate = 0;
    public static bool ImmeLevel = false;
    public static float _bgmVolume;
    public static float _seVolume;

    public static bool checkTutorial()
    {
        return isTutorial;
    }

    public static void SetTutorial(bool isTu)
    {
        Debug.Log($"Set tutorial flug:{isTu}");
        isTutorial = isTu;
    }

    public static float CalcurateCanvasRotate(float x)
    {
        return 5 * Mathf.Pow(x - 1, 2);
    }
}
