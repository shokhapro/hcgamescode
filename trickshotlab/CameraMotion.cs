using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMotion : MonoBehaviour
{
    [SerializeField] private Vector2 field = new Vector2(0f, 10f);
    [Space]
    [SerializeField] private float movingSpeed = 1f;
    [SerializeField] private float fastMovingSpeed = 4f;
    [Space]
    [SerializeField] private bool followingActive;
    [SerializeField] private Transform followingTarget;
    [SerializeField] private float followingDelayDistance = 1f;
    [SerializeField] [Range(0f, 1f)] private float followingSmoothing = 0.8f;
    [Space]
    [SerializeField] private float slidingFactor = 1f;
    [SerializeField] [Range(0f, 1f)] private float slidingDecelerationSmoothing = 0.9f;
    
    private Transform _t;
    private Vector3 _p;
    private Coroutine _moving = null;
    private TouchControl _tc;
    private bool _slidingBegan = false;
    private float _slidingLastSpeed;
    private float _slidingLastSpeedTime;

    private void Awake()
    {
        _t = transform;
        
        _p = _t.position;

        _tc = FindObjectOfType<TouchControl>();
    }

    private void Update()
    {
        if (_moving != null)
            ;
        else if (followingActive)
            Following();
        else if (!(_tc && _tc.HasActiveObject))
            Sliding();
    }
    
    public void SetField(Vector2 value)
    {
        field = value;
    }
    
    public Vector3 GetPosition()
    {
        return _t.position;
    }

    public void SetPosition(float pos)
    {
        if (_moving != null)
        {
            StopCoroutine(_moving);
            _moving = null;
        }

        _p.x = pos;
        
        _p.x = Mathf.Clamp(_p.x, field.x, field.y);
        
        _t.position = _p;
    }

    public float MoveToPosition(float pos, bool fast = false)
    {
        if (_moving != null)
            StopCoroutine(_moving);

        float t = 1 / (fast ? fastMovingSpeed : movingSpeed) * Mathf.Abs(pos - _t.position.x);

        _moving = StartCoroutine(Moving(pos, t));

        return t;
    }

    private IEnumerator Moving(float toPos, float time)
    {
        float p0 = _t.position.x;
        float p1 = toPos;
        
        float t1 = time;
        float t = 0f;

        while (t < t1)
        {
            t += Time.deltaTime;

            float linear = t / t1;
            if (linear > 1f) linear = 1f;
            
            float curve = Easing.Cubic.InOut(linear);

            _p.x = Mathf.Lerp(p0, p1, curve);
            
            _p.x = Mathf.Clamp(_p.x, field.x, field.y);

            _t.position = _p;

            yield return null;
        }

        _moving = null;
    }
    
    private void Following()
    {
        if (!followingTarget) return;
        
        var d = Mathf.Clamp(followingTarget.position.x - _p.x, -followingDelayDistance, followingDelayDistance);
        _p.x = followingTarget.position.x - d;
        
        _p.x = Mathf.Clamp(_p.x, field.x, field.y);

        _t.position = Vector3.Lerp(_p, _t.position, followingSmoothing);
    }
    
    public void SetFollowingTarget(Transform value)
    {
        followingTarget = value;
    }

    public void SetFollowingActive(bool value)
    {
        followingActive = value;
    }

    private void Sliding()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                {
                    _slidingBegan = !TouchControl.IsPointerOverUIObject();
                
                    if (!_slidingBegan) break;
                
                    _slidingLastSpeed = 0f;
                
                    _p = _t.position;

                    break;
                }
                case TouchPhase.Moved: case TouchPhase.Stationary:
                {
                    if (!_slidingBegan) break;
                
                    float d = t.deltaPosition.x / Screen.height * slidingFactor;
                
                    if (!(Mathf.Abs(d) <= Mathf.Abs(_slidingLastSpeed) && Time.time - _slidingLastSpeedTime < 0.1f))
                    {
                        _slidingLastSpeed = d;
                        _slidingLastSpeedTime = Time.time;
                    }

                    _p.x -= d;
                
                    _p.x = Mathf.Clamp(_p.x, field.x, field.y);

                    _t.position = _p;
                    
                    break;
                }
                case TouchPhase.Ended: case TouchPhase.Canceled:
                {
                    _slidingBegan = false;
                    
                    if (Mathf.Abs(_slidingLastSpeed) < 0.01f) _slidingLastSpeed = 0f;

                    break;
                }
            }
        }

        if (!_slidingBegan && _slidingLastSpeed != 0f)
        {
            _slidingLastSpeed = _slidingLastSpeed * slidingDecelerationSmoothing;

            if (Mathf.Abs(_slidingLastSpeed) < 0.001f) _slidingLastSpeed = 0f;

            _p.x -= _slidingLastSpeed;
                
            _p.x = Mathf.Clamp(_p.x, field.x, field.y);

            _t.position = _p;
        }
    }
}
