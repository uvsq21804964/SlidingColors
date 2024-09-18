using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour {
  
   public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

}