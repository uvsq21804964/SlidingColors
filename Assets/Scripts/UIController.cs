using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public class UIController : MonoBehaviour
{
    static public UIController Instance;

    public CanvasGroup MainMenu;
    public CanvasGroup GamePlay;
    public CanvasGroup GameOverMenu;
    public CanvasGroup NextLevelMenu;

    public Text helpText;

    public Text LevelText;
    public Text MovesRemainingText;
    public Text SolutionText;

    [SerializeField] private GameObject StartGameUI;

    public GameObject RestartGame;
    public GameObject NextLevel;

    public int _help;
    public RewardedAdsManager rewardedAdsManager;

    void Awake() {
        Instance = this;

        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameEnded += OnGameEnded;
    }

    void Start() {
        MainMenu.alpha = 0;
        GamePlay.alpha = 1;
        GameOverMenu.alpha = 0;
        NextLevelMenu.alpha = 0;

        UpdateLevel();
        DisplaySolution();

        GameOverMenu.gameObject.SetActive(false);
        NextLevelMenu.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(false);

        if(GameManager._solution.Count() == PlayerPrefs.GetInt("Help", 0)) {
            Destroy(GamePlay.transform.Find("Help").gameObject);
        }
    }

    void OnDestroy() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameEnded -= OnGameEnded;
    }

    void OnGameStarted() {
        // MainMenu.DOFade(0, 0.2f).OnComplete(() => {
        //     MainMenu.gameObject.SetActive(false);
        // });

        GamePlay.gameObject.SetActive(true);
        GamePlay.DOFade(1, 2.2f);
    }

    void OnGameEnded() {
        GamePlay.DOFade(0, 0.2f).OnComplete(() => {
            MainMenu.gameObject.SetActive(false);
        });

        if(GameManager.Instance._state == GameManager.GameState.Win) {
            NextLevelMenu.gameObject.SetActive(true);
            NextLevel.transform.localScale = Vector3.zero;
            NextLevelMenu.DOFade(1, 0.4f)
            .OnComplete(() => {
                NextLevel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
                    // if(GameManager.Level%3 == 0) InterstitialAdsManager.Instance.ShowAd();
                });
            });
        }
        else {
            Camera.main.DOShakePosition(0.5f, 0.5f, 10, 90, false);
            GameOverMenu.gameObject.SetActive(true);
            RestartGame.transform.localScale = Vector3.zero;
            GameOverMenu.DOFade(1, 0.4f)
            .OnComplete(() => {
                RestartGame.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            });
        }
    }

    public void TriggerVideoHelp() {
        // rewardedAdsManager.ShowRewardedAd();
    }

    public void UpdateSolutionText() {
        _help = PlayerPrefs.GetInt("Help", 0);

        if(_help >= GameManager._solution.Count()) return;

        _help += 1;
        PlayerPrefs.SetInt("Help", _help);
        PlayerPrefs.Save();

        helpText.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutBack);

        if(GameManager._solution.Count() == _help) {
            RemoveHelpPanel();
        }

        DisplaySolution();
    }

    private void RemoveHelpPanel()
    {
        GameObject helpPanel = GamePlay.transform.Find("Help").gameObject;
        helpPanel.transform.DOScale(0, 0.2f).OnComplete(() => {
            Destroy(helpPanel);
        });
    }

    private void DisplaySolution() {
        SolutionText.text = "";

        _help = PlayerPrefs.GetInt("Help", 0);
        if(_help == 0) return;
        
        for(int i = 0; i < _help && i < GameManager._solution.Count() ; i++) {
            switch(GameManager._solution[i]) {
                case 'D':
                    SolutionText.text += "↑ ";
                    break;
                case 'U':
                    SolutionText.text += "↓ ";
                    break;
                case 'L':
                    SolutionText.text += "← ";
                    break;
                case 'R':
                    SolutionText.text += "→ ";
                    break;
            }
        }
        SolutionText.transform.DOPunchScale(Vector3.one * 0.15f, 2.2f);
    }

    public void UpdateLevel() {
        LevelText.text = "LEVEL " + GameManager.Level.ToString();
        LevelText.transform.DOPunchScale(Vector3.one * 0.15f, 2.2f);
    }

    public void Update() {
        int round = GameManager._round;
        int nbMoves = GameManager._solution.Count();
        int remainingRounds = nbMoves - round;
        if(remainingRounds == 1) {
            MovesRemainingText.text = "Last move";
        }
        else if (remainingRounds == 0) {
            MovesRemainingText.text = "";
        }
        else {
            MovesRemainingText.text = remainingRounds.ToString() + " moves remaining";
        }
    }

    public void TriggerStartGame() {
        StartGameUI.SetActive(false);  
        GameManager.Instance.StartGame();
    }
}
