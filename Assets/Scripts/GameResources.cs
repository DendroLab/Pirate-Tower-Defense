using UnityEngine;

public class GameResources
{
    private static string coin = "coin";
    private static string level = "level";

    public static int Coin
    {
        get { return PlayerPrefs.GetInt(coin); }
        set { PlayerPrefs.SetInt(coin, value); }
    }

    public static int[] Level
    {
        get { return PlayerPrefsX.GetIntArray(level); }
        set { PlayerPrefsX.SetIntArray(level, value); }
    }
}
