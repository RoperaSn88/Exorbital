using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MapScriptableObject")]
public class MapInfoScriptableObject : ScriptableObject
{
    public int MapCount;
    public Vector3 MapScale;
    public int[] Neighbors=new int[8];

    //Vector4(x,y,z,Number)‚ÅoŒû‚Ì”Ô†‚É‚æ‚Á‚Ä‚ÌMapNumber‚ğ•Ï‰»‚³‚¹‚éB
    public Vector4[] ExitNumber;
    public bool FallNumber;
    public bool ClimbNumber;
}
