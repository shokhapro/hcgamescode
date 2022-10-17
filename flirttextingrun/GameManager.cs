using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private Level[] levelsForBoys;
    [SerializeField] private int levelsForBoysCount = 5;
    [SerializeField] private Level[] levelsForGirls;
    [SerializeField] private int levelsForGirlsCount = 5;
    [Space]
    [SerializeField] private TextInText[] names;
    [SerializeField] private Image[] photos;
    [SerializeField] private GateObject gate1Prefab;
    [SerializeField] private GateObject gate2Prefab;
    [SerializeField] private Image[] levelsPhotos;
    [SerializeField] private GameObject levelsNextText;
    [SerializeField] private GameObject levelsRetryText;
    [SerializeField] private GameObject levelsWinLevelIcon;
    [SerializeField] private TextInText levelNumber;
    [SerializeField] private GameObject[] forBoysObjects;
    [SerializeField] private GameObject[] forGirlsObjects;
    [Space]
    [SerializeField] private Delays.DelayedEvents onStartLevel;
    [SerializeField] private Delays.DelayedEvents onWin;
    [SerializeField] private Delays.DelayedEvents onFail;

    [Serializable]
    public class Level
    {
        public string name;
        public Sprite photo;
        public Dialog dialog;

        [Serializable]
        public class Dialog
        {
            public Block[] blocks;
            
            [Serializable]
            public class Block
            {
                public string[] oppMessage;
                public int win = 0;
                public string meMessage1;
                public int nextBlockId1;
                public string meMessage2;
                public int nextBlockId2;
            }
        }
    }

    private bool _forGirls;
    private int _levelId;
    private Level.Dialog _dialog = null;
    private const float RunDialogBlockDelay = 1f;
    private const float GateStartPosition = 33f;
    
    private void Awake()
    {
        Instance = this;
        
        if (!PlayerPrefs.HasKey("levelid")) PlayerPrefs.SetInt("levelid", 0);
        if (!PlayerPrefs.HasKey("levelidgirls")) PlayerPrefs.SetInt("levelidgirls", 0);
        if (!PlayerPrefs.HasKey("forgirls")) PlayerPrefs.SetInt("forgirls", 0);
    }
    
    private void Start()
    {
        NextLevel();
    }

    private void OpenLevel(bool forGirls, int id)
    {
        _forGirls = forGirls;
        _levelId = id;
        
        for (var i = 0; i < forBoysObjects.Length; i++)
            forBoysObjects[i].SetActive(!forGirls);
        for (var i = 0; i < forGirlsObjects.Length; i++)
            forGirlsObjects[i].SetActive(forGirls);
        
        StartCoroutine(onStartLevel.InvokeCoroutine());
        
        levelNumber.Set((_levelId + 1).ToString());
        
        var l = forGirls ? levelsForGirls[id] : levelsForBoys[id];
        
        for (var i = 0; i < names.Length; i++)
            names[i].Set(l.name);
        for (var i = 0; i < photos.Length; i++)
            photos[i].sprite = l.photo;

        _dialog = l.dialog;
        
        TinySauce.OnGameStarted();
    }

    public void StartDialog()
    {
        RunDialogBlock(0);
    }

    public void RunDialogBlock(int id)
    {
        if (_dialog == null) return;

        if (id < 0 || id >= _dialog.blocks.Length) return;

        var b = _dialog.blocks[id];

        StartCoroutine(run());

        IEnumerator run()
        {
            if (b.oppMessage.Length > 0)
            {
                for (var i = 0; i < b.oppMessage.Length; i++)
                {
                    yield return new WaitForSeconds(1.5f);
                    ChatManager.Instance.AddMessage(true, b.oppMessage[i], (b.win == 1 && i == b.oppMessage.Length - 1));
                }
            }
            
            if (b.win == 1)
            {
                StartCoroutine(onWin.InvokeCoroutine());

                for (var i = 0; i < 4; i++)
                {
                    Sprite s = null;
                    var ls = _forGirls ? levelsForGirls : levelsForBoys;
                    var n = _levelId + i;
                    if (n >= 0 && n < ls.Length) s = ls[n].photo;
                    levelsPhotos[i].sprite = s;
                }
                levelsNextText.SetActive(true);
                levelsRetryText.SetActive(false);
                levelsWinLevelIcon.SetActive(true);
                
                var l = PlayerPrefs.GetInt("forgirls") == 1 ? "levelidgirls" : "levelid";
                var nextLevelId = _levelId + 1;
                if (nextLevelId == (PlayerPrefs.GetInt("forgirls") == 1 ? levelsForGirlsCount : levelsForBoysCount)) nextLevelId = 0;
                PlayerPrefs.SetInt(l, nextLevelId);
            }
            else if (b.win == -1)
            {
                StartCoroutine(onFail.InvokeCoroutine());
                
                for (var i = 0; i < 4; i++)
                {
                    Sprite s = null;
                    var ls = _forGirls ? levelsForGirls : levelsForBoys;
                    var n = _levelId - 1 + i;
                    if (n >= 0 && n < ls.Length) s = ls[n].photo;
                    levelsPhotos[i].sprite = s;
                }
                levelsNextText.SetActive(false);
                levelsRetryText.SetActive(true);
                levelsWinLevelIcon.SetActive(_levelId > 0);
            }
            else
            {
                GateObject g1 = null;
                GateObject g2 = null;
                if (!b.meMessage1.Equals(""))
                {
                    g1 = Instantiate(gate1Prefab);
                    g1.transform.position = new Vector3(g1.transform.position.x, g1.transform.position.y, GateStartPosition);
                    g1.Set(b.meMessage1);
                    g1.onHit.AddListener(() => RunDialogBlock(b.nextBlockId1));
                }
                if (!b.meMessage2.Equals(""))
                {
                    g2 = Instantiate(gate2Prefab);
                    g2.transform.position = new Vector3(g2.transform.position.x, g2.transform.position.y, GateStartPosition);
                    g2.Set(b.meMessage2);
                    g2.onHit.AddListener(() => RunDialogBlock(b.nextBlockId2));
                }

                if (!g1.Equals(null) && !g2.Equals(null))
                {
                    g1.onHit.AddListener(() => g2.GetComponent<Collider>().enabled = false);
                    g2.onHit.AddListener(() => g1.GetComponent<Collider>().enabled = false);
                }
            }

            yield return null;
        }
    }

    public void OnLevelEnd(int score)
    {
        TinySauce.OnGameFinished(score);
    }

    public void NextLevel()
    {
        OpenLevel(PlayerPrefs.GetInt("forgirls") == 1, PlayerPrefs.GetInt(PlayerPrefs.GetInt("forgirls") == 1 ? "levelidgirls" : "levelid"));
    }
}
