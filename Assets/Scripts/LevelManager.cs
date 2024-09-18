using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int level;

    void Awake() {
        Instance = this;
    }

    void Start() {
        level = 1;
        if(PlayerPrefs.HasKey("Level")) {
            level = PlayerPrefs.GetInt("Level");
        }
    }

    public void IncrementLevel() {
        level = PlayerPrefs.GetInt("Level");
        level++;
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.Save();
    }
}
