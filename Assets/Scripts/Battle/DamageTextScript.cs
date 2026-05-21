using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTextScript : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void EndAnimation()
    {
        Destroy(gameObject);
    }
}
