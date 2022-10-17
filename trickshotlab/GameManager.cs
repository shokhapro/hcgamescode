using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelManager[] levels;
    [Space]
    [SerializeField] private GameObject blockScreen;
    [SerializeField] private Text levelNumber;
    [SerializeField] private Text starCounter;
    [SerializeField] private AnimationPlay starCounterAnimation;
    [SerializeField] private TextCountAnimation scoreCounter;
    [Space]
    [SerializeField] private UnityEvent onLevelOpen;
    [SerializeField] private UnityEvent onLevelStart;
    [SerializeField] private UnityEvent onLevelFinish;
    [SerializeField] private UnityEvent onLevelRetry;
    
    private LevelManager _activeLevel = null;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("level")) PlayerPrefs.SetInt("level", 0);
        if (!PlayerPrefs.HasKey("score")) PlayerPrefs.SetInt("score", 0);
        
        if (blockScreen)
        {
            LevelManager.blockControl.AddListener(() => { blockScreen.SetActive(true); });
            LevelManager.unblockControl.AddListener(() => { blockScreen.SetActive(false); });
        }
        
        LevelManager.onStart.AddListener(onLevelStart.Invoke);
        
        onLevelFinish.AddListener(DeviceVibrate);
        onLevelFinish.AddListener(UpdateScoreCounter);
        LevelManager.onFinish.AddListener(onLevelFinish.Invoke);

        LevelManager.onStarPlus.AddListener(() => UpdateStarCounter());
    }

    private void Start()
    {
        OpenLevel(PlayerPrefs.GetInt("level"));
    }

    private void OpenLevel(int i)
    {
        if (_activeLevel != null)
            Destroy(_activeLevel.gameObject);

        _activeLevel = Instantiate(levels[i]);
        
        onLevelOpen.Invoke();

        if (levelNumber)
            levelNumber.text = "Lvl " + (i + 1).ToString();

        UpdateStarCounter(false);
    }

    public void NextLevel()
    {
        int score = PlayerPrefs.GetInt("score") + (_activeLevel ? _activeLevel.GetStarCount() : 0);
        int level = PlayerPrefs.GetInt("level") + 1;

        if (level >= levels.Length)
        {
            level = 0;
            score = 0;
        }

        OpenLevel(level);

        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("level", level);
    }

    public void ActiveLevelRun()
    {
        if (!_activeLevel) return;
        
        _activeLevel.Run();
    }
    
    public void ActiveLevelStop()
    {
        if (!_activeLevel) return;
        
        _activeLevel.Stop();

        UpdateStarCounter(false);
    }

    public void ActiveLevelRetry()
    {
        if (!_activeLevel) return;
        
        _activeLevel.Stop(false);
        
        onLevelRetry.Invoke();

        UpdateStarCounter(false);
    }

    private void UpdateStarCounter(bool anim = true)
    {
        if (!starCounter) return;
        
        starCounter.text = _activeLevel ? _activeLevel.GetStarCount().ToString() : "0";

        if (anim)
            if (starCounterAnimation)
                starCounterAnimation.Play();
    }
    
    private void UpdateScoreCounter()
    {
        if (!scoreCounter) return;

        var value = PlayerPrefs.GetInt("score");
        var plus = _activeLevel ? _activeLevel.GetStarCount() : 0;

        scoreCounter.value = value;
        scoreCounter.plus = plus;
    }

    private void DeviceVibrate()
    {
        Handheld.Vibrate();
    }
}
