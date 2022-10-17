using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Vector2 cameraField = new Vector2(1f, 9f);
    [SerializeField] private float startPoint = 1f;
    [SerializeField] private float finishPoint = 9f;

    public static UnityEvent blockControl = new UnityEvent();
    public static UnityEvent unblockControl = new UnityEvent();
    
    public static UnityEvent onStart = new UnityEvent();
    public static UnityEvent onFinish = new UnityEvent();

    public static UnityEvent onStarPlus = new UnityEvent();

    private CameraMotion _cm;
    private Ball _b;
    private LevelObject[] _los;

    private bool _isRunning = false;
    private int _stars = 0;
    private float _timerStartTime;
    private float _timerEndTime;
    
    private void Awake()
    {
        blockControl.AddListener(() => { TouchControl.Enabled = false; });
        unblockControl.AddListener(() => { TouchControl.Enabled = true; });
        
        _cm = FindObjectOfType<CameraMotion>();
        
        if (_cm)
            _cm.SetField(cameraField);
        
        _b = FindObjectOfType<Ball>();

        if (_cm)
            _cm.SetFollowingTarget(_b.transform);

        _los = FindObjectsOfType<LevelObject>();
        
        //all on level load operations
    }

    private void Start()
    {
        _stars = 0;
        //set vars

        foreach (var lo in _los)
            lo.OnPreview();

        //

        if (_cm)
        {
            _cm.SetFollowingActive(false);
            
            _cm.SetPosition(startPoint);

            blockControl.Invoke();

            this.Delay(0.75f, () =>
            {
                var t = _cm.MoveToPosition(finishPoint);
                
                this.Delay(t + 0.5f, () =>
                {
                    var t2 = _cm.MoveToPosition(startPoint, true);
                
                    this.Delay(t2, () =>
                    {
                        unblockControl.Invoke();
                    
                        foreach (var lo in _los)
                            lo.OnStop();
                    
                        onStart.Invoke();
                    });
                });
            });
        }
    }

    public void Run()
    {
        if (_isRunning) return;
        _isRunning = true;
        
        _timerStartTime = _timerEndTime = Time.time;
        //set vars

        foreach (var lo in _los)
            lo.OnRun();

        if (_b)
            _b.Run();

        if (_cm)
            _cm.SetFollowingActive(true);
        
        //
    }
    
    public void Stop(bool faded = true)
    {
        if (!_isRunning) return;
        _isRunning = false;

        if (_cm)
        {
            _cm.SetFollowingActive(false);

            if (faded)
            {
                TouchControl.CurrentInactionListeners.Add(new TimeAction(2f, () =>
                {
                    if (Mathf.Abs(_cm.GetPosition().x - startPoint) > 1f)
                    {
                        var t = _cm.MoveToPosition(startPoint, true);
                    
                        blockControl.Invoke();
                        this.Delay(t, unblockControl.Invoke);
                    }
                }));
            }
            else
            {
                _cm.SetPosition(startPoint);

                unblockControl.Invoke();
            }
        }
        
        if (_b)
            _b.Stop();

        foreach (var lo in _los)
            lo.OnStop();

        _stars = 0;
        _timerEndTime = Time.time;
        //set vars
        
        //
    }
    
    public void Finish()
    {
        if (_cm)
        {
            _cm.SetFollowingActive(false);
            
            blockControl.Invoke();
            
            /*this.Delay(2f, () =>
            {
                var t = _cm.MoveToPosition(startPoint);
            
                this.Delay(t + 0.5f, () => _cm.MoveToPosition(finishPoint));
            });*/
        }

        if (_b)
        {
            _b.Relax();
            _b.AddForce(new Vector2(0f, -10f));
        }
        
        _timerEndTime = Time.time;

        onFinish.Invoke();
    }

    public void StarPlus()
    {
        _stars++;
        
        onStarPlus.Invoke();
    }

    public int GetStarCount()
    {
        return _stars;
    }
    
    public float GetTimerValue()
    {
        return _timerEndTime - _timerStartTime;
    }
}
