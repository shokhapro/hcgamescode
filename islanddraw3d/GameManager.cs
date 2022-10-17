using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private IslandMesh island;
    [SerializeField] private Player player;
    [SerializeField] private MeshRenderer outline;
    private Material _outlineMaterial;
    [SerializeField] private ProgressingText accuracyText;
    [SerializeField] private ProgressingScale accuracyProgress;
    [SerializeField] private ProgressingText rewardMoneyText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Text extraMoneyText;
    [SerializeField] private DecorationButton decorationButtonPrefab;
    [SerializeField] private Transform decorationButtonsParent;

    [Space]
    [SerializeField] private Delays.DelayedEvents onInitLevel;
    [SerializeField] private Delays.DelayedEvents onRestart;
    [SerializeField] private Delays.DelayedEvents onCheck;
    [SerializeField] private Delays.DelayedEvents onCheckPass;
    [SerializeField] private Delays.DelayedEvents onCheckFail;
    [SerializeField] private Delays.DelayedEvents onDecorate;
    [SerializeField] private Delays.DelayedEvents onFinishLevel;

    [Space]
    [SerializeField] private float accuracyThreshold = 0.8f;
    
    [Serializable] public class Level
    {
        public Texture2D map;
        public int money = 25;
    }
    [Space][SerializeField] private Level[] levels;
    
    [Serializable] public class Decoration
    {
        public GameObject prefab;
        public Sprite icon;
        public int price = 0;
        public float randomRotation = 360;
        public int maxCount = 10;
    }
    [Space][SerializeField] private Decoration[] decorations;
    
    private int _activeLevelId;
    private int _activeReward = 0;

    private void Awake()
    {
        Instance = this;
        
        _outlineMaterial = outline.material;
        
        if (!PlayerPrefs.HasKey("levelid")) PlayerPrefs.SetInt("levelid", 0);
        if (!PlayerPrefs.HasKey("money")) PlayerPrefs.SetInt("money", 0);

        foreach (var d in decorations)
            Instantiate(decorationButtonPrefab, decorationButtonsParent).Set(d.prefab, d.icon, d.price, d.randomRotation, d.maxCount);
    }

    private void Start()
    {
        InitLevel(PlayerPrefs.GetInt("levelid"));
        
        UpdateMoney();
    }

    private void InitLevel(int id)
    {
        _activeLevelId = id;

        player.ResetPosition();
        
        island.Clear();

        _outlineMaterial.mainTexture = levels[_activeLevelId].map;

        StartCoroutine(onInitLevel.InvokeCoroutine());
    }

    private float CalcAccuracy()
    {
        var map = levels[_activeLevelId].map;
        var hs = island.GetHeights();
        var size = 128;
        var waterHeight = 0.1f;

        var count = 0;
        var summ = 0;
        
        for (var x = 0; x < size; x++)
        for (var y = 0; y < size; y++)
        {
            var mValue = map.GetPixelBilinear(1f * x / size, 1f * y / size).grayscale > 0.5f;
            var hValue = hs[x * size + y] > waterHeight;
            
            if (!(mValue || hValue)) continue;

            count++;

            if (hValue == mValue) summ++;
        }

        var acc = 1f * summ / count;

        return acc;
    }

    public void Check()
    {
        this.DelayedAction(0.5f, () =>
        {
            var accuracy = CalcAccuracy();

            var percents = Mathf.RoundToInt(accuracy * 100f);
            accuracyText.SetValues(0, percents);
            
            accuracyProgress.SetValues(new Vector3(0, 1, 1), new Vector3(accuracy, 1, 1));
            
            var money = Mathf.RoundToInt(levels[_activeLevelId].money * accuracy);
            rewardMoneyText.SetValues(0, money);
            
            if (accuracy >= accuracyThreshold)
            {
                var nextLevelId = _activeLevelId + 1;
                if (nextLevelId > levels.Length - 1) nextLevelId = 0;
                PlayerPrefs.SetInt("levelid", nextLevelId);

                _activeReward = money;
                AddMoney(_activeReward);
                
                StartCoroutine(onCheckPass.InvokeCoroutine());
            }
            else
                StartCoroutine(onCheckFail.InvokeCoroutine());
        });
        
        StartCoroutine(onCheck.InvokeCoroutine());
    }
    
    public void Restart()
    {
        player.ResetPosition();
        
        island.Clear();
        
        StartCoroutine(onRestart.InvokeCoroutine());
    }

    public void CancelPassResults()
    {
        PlayerPrefs.SetInt("levelid", _activeLevelId);
        
        AddMoney(-_activeReward);
        _activeReward = 0;
    }

    public void Decorate()
    {
        StartCoroutine(onDecorate.InvokeCoroutine());
    }
    
    public void Finish()
    {
        StartCoroutine(onFinishLevel.InvokeCoroutine());
    }

    public void NextLevel()
    {
        InitLevel(PlayerPrefs.GetInt("levelid"));
    }

    private void UpdateMoney()
    {
        moneyText.text = "$" + PlayerPrefs.GetInt("money");
    }
    
    private void AddMoney(int value)
    {
        PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") + value);

        UpdateMoney();
    }

    public bool SpendMoney(int value)
    {
        var money = PlayerPrefs.GetInt("money");

        if (value > money) return false;

        money -= value;
        PlayerPrefs.SetInt("money", money);

        UpdateMoney();

        return true;
    }
    
    public void AddExtraMoney()
    {
        var value = Random.Range(5, 30);

        extraMoneyText.text = "+$" + value;
        
        AddMoney(value);
    }
}
