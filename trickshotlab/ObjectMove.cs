using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ObjectMove : MonoBehaviour, ITouchObject
{
    [SerializeField] private Transform obj;
    [SerializeField] private Collider field;
    [Space]
    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onStop;

    private Transform _t;
    private Vector3 _startPos;
    private Vector3 _touchStartPos;
    private Vector2 _fieldMin;
    private Vector2 _fieldMax;

    private void Awake()
    {
        _t = transform;

        if (field)
        {
            var b = field.bounds;
            
            _fieldMin = new Vector2(b.min.x, b.min.y);
            _fieldMax = new Vector2(b.max.x, b.max.y);
        }
    }

    private void Start()
    {
        var p = ClampInField(_t.position);

        _t.position = p;
        
        if (obj)
            obj.position = p;
    }
    
    public void OnStart(Vector3 pos)
    {
        _startPos = _t.position;

        _touchStartPos = pos;
        
        onStart.Invoke();
    }

    public void OnMoving(Vector3 pos)
    {
        var p = _startPos + (pos - _touchStartPos);
        
        p = ClampInField(p);

        _t.position = p;
        
        if (obj)
            obj.position = p;
    }
    
    public void OnStop()
    {
        onStop.Invoke();
    }

    private Vector3 ClampInField(Vector3 pos)
    {
        if (!field)
            return pos;
        
        var p = new Vector3(Mathf.Clamp(pos.x, _fieldMin.x, _fieldMax.x), Mathf.Clamp(pos.y, _fieldMin.y, _fieldMax.y), pos.z);

        return p;
    }
}
