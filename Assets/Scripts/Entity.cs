using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    private const string coin = "coin";
    private const string level = "level";

    public int Coin
    {
        get { return PlayerPrefs.GetInt(coin); }
        set { PlayerPrefs.SetInt(coin, value); }
    }

    public int[] Level
    {
        get { return PlayerPrefsX.GetIntArray(level); }
        set { PlayerPrefsX.SetIntArray(level, value); }
    }
}
