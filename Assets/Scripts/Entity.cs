using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour{
    private const string coin = "coin";

    public int Coin {
        get { return PlayerPrefs.GetInt(coin); }
        set { PlayerPrefs.SetInt(coin, value); }
    }
}
