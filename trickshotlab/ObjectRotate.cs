using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ObjectRotate : MonoBehaviour, ITouchObject
{
    [SerializeField] private Transform obj;
    [SerializeField] private Vector2 range = new Vector2();
    
    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onStop;

    private Transform _t;
    private Vector3 _ta;
    private Vector3 _startAng;
    private float _touchStartAng;
    private float _lastTouchAng;
    private int _touchAngCircles;
    private bool _noRange;

    private void Awake()
    {
        _t = transform;

        _noRange = simpang(range.x).Equals(simpang(range.y));

        float simpang(float value)
        {
            var a = value;
            a = a % 360f;
            if (a < 0) a = 360f - a;

            return a;
        }
    }

    private void Start()
    {
        _ta = _t.eulerAngles;
        
        var a = ClampInRange(_ta);
        
        _ta = a;

        _t.eulerAngles = _ta;

        if (obj)
        {
            //obj.position = _t.position;

            obj.eulerAngles = _ta;
        }
    }
    
    public void OnStart(Vector3 pos)
    {
        _startAng = _ta;

        _touchStartAng = TwoPointsAngle(_t.position, pos);

        _lastTouchAng = _touchStartAng;

        _touchAngCircles = 0;
        
        onStart.Invoke();
    }

    public void OnMoving(Vector3 pos)
    {
        float ta = TwoPointsAngle(_t.position, pos);

        if (_lastTouchAng > 90f && ta < -90f) _touchAngCircles++;
        if (_lastTouchAng < -90f && ta > 90f) _touchAngCircles--;

        _lastTouchAng = ta;
        
        float daz = _touchAngCircles * 360f + ta - _touchStartAng;

        var a = _startAng + new Vector3(0f, 0f, daz);
        
        a = ClampInRange(a);

        _ta = a;

        _t.eulerAngles = _ta;
        
        if (obj)
            obj.eulerAngles = _ta;
    }
    
    public void OnStop()
    {
        onStop.Invoke();
    }

    private Vector3 ClampInRange(Vector3 angle)
    {
        if (_noRange)
            return angle;

        var a = new Vector3(angle.x, angle.y, Mathf.Clamp(angle.z, range.x, range.y));

        return a;
    }

    private float TwoPointsAngle(Vector2 p1, Vector2 p2)
    {
        var a = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Mathf.PI;

        return a;
    }
}
