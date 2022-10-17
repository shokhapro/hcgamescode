using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private IslandMesh _island;
    private Transform _body;
    private Vector3 _bodyPoint;
    [SerializeField] private float bodyMoveSmooth = 0.8f;
    private int _touchLayerMask;
    [SerializeField] private Collider bounds;
    [SerializeField] private Transform sprayTarget;
    [SerializeField] private float sprayPower = 0.3f;
    [SerializeField] private Delays.DelayedEvents onSprayStart;
    [SerializeField] private Delays.DelayedEvents onSprayStop;
    private Camera _camera;
    private Plane _raycastPlane;
    [SerializeField] private Delays.DelayedEvents onFirstTouch;
    
    private bool _touchActive = true;
    private bool _touchBegan = false;
    private bool _touchCollider = false;
    private Vector3 _touchOffset;
    private Vector3 _bodyPosition;
    private Vector3 _cameraPosition;
    private Coroutine _sprayStartCoroutine;
    private Coroutine _sprayStopCoroutine;
    private bool _sand = false;
    private bool _firstTouch = true;
    
    private void Awake()
    {
        _island = FindObjectOfType<IslandMesh>();

        _body = transform;
        _bodyPoint = _body.position;
        
        _camera = Camera.main;

        _touchLayerMask = LayerMask.GetMask("Player");

        _raycastPlane = new Plane(Vector3.up, -_body.position.y);
        
        _bodyPosition = _body.transform.position;
    }

    private void Update()
    {
        TouchUpdate();

        _body.position = Vector3.Lerp(_bodyPosition, _body.position, bodyMoveSmooth);
    }

    private void OnEnable()
    {
        StartCoroutine(SandCoroutine());
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void TouchUpdate()
    {
        if (!_touchActive) return;
        
        if (Input.touchCount <= 0) return;

        Touch t = Input.GetTouch(0);
        
        Ray r = _camera.ScreenPointToRay(t.position);

        switch (t.phase)
        {
            case TouchPhase.Began:
                if (IsPointerOverUIObject()) return;
                
                _touchBegan = true;
                
                if (Physics.Raycast(r, 25f, _touchLayerMask)) _touchCollider = true;
                
                SprayActive(true);
                Move(true);

                if (_firstTouch)
                {
                    _firstTouch = false;

                    StartCoroutine(onFirstTouch.InvokeCoroutine());
                }
                break;
            case TouchPhase.Moved: case TouchPhase.Stationary:
                if (!_touchBegan) return;
                
                Move();
                break;
            case TouchPhase.Ended:
                if (!_touchBegan) return;

                SprayActive(false);
                
                _touchBegan = false;

                _touchCollider = false;
                break;
        }

        void Move(bool began = false)
        {
            if (_raycastPlane.Raycast(r, out var d))
            {
                var p = r.GetPoint(d);
                
                if (began) _touchOffset = _touchCollider ? _body.position - p : Vector3.zero;
                p += _touchOffset;

                var b = bounds.bounds;
                p.x = Mathf.Clamp(p.x, b.min.x, b.max.x);
                p.z = Mathf.Clamp(p.z, b.min.z, b.max.z);

                _bodyPosition = p;
            }
        }
    }

    public void TouchActive(bool value)
    {
        _touchActive = value;

        if (!value)
        {
            if (_sprayStartCoroutine != null)
                StopCoroutine(_sprayStartCoroutine);
            if (_sprayStopCoroutine != null)
                StopCoroutine(_sprayStopCoroutine);

            _touchBegan = false;

            _touchCollider = false;
            
            _sand = false;

            _firstTouch = true;
        }
    }
    
    private void SprayActive(bool value)
    {
        if (value)
        {
            if (_sprayStopCoroutine != null)
                StopCoroutine(_sprayStopCoroutine);
            
            _sprayStartCoroutine = StartCoroutine(onSprayStart.InvokeCoroutine());
        }
        else
        {
            if (_sprayStartCoroutine != null)
                StopCoroutine(_sprayStartCoroutine);
                
            _sprayStopCoroutine = StartCoroutine(onSprayStop.InvokeCoroutine());
        }
    }

    public void Sand(bool value)
    {
        _sand = value;
    }

    private IEnumerator SandCoroutine()
    {
        const float dt = 0.05f;
        var w = new WaitForSeconds(dt);
        
        while (true)
        {
            if (_sand) _island.Sand(sprayTarget.position, sprayPower * dt);
            
            yield return w;
        }
    }

    public void FadeIn(float z)
    {
        IEnumerator FadeInCoroutine()
        {
            var t = 1f;
            var t0 = 0f;
            var to = _bodyPosition;
            var from = to + new Vector3(0, 0, z);
        
            while (t0 < t)
            {
                t0 += Time.deltaTime;

                var l = Mathf.Clamp01(t0 / t);
                
                _bodyPosition = Vector3.Lerp(from, to, l);
                
                yield return null;
            }
        }
        
        StartCoroutine(FadeInCoroutine());
    }
    
    public void FadeOut(float z)
    {
        IEnumerator FadeOutCoroutine()
        {
            var t = 2f;
            var t0 = 0f;
            var from = _bodyPosition;
            var to = from + new Vector3(0, 0, z);
        
            while (t0 < t)
            {
                t0 += Time.deltaTime;

                var l = Mathf.Clamp01(t0 / t);
                
                _bodyPosition = Vector3.Lerp(from, to, l);
                
                yield return null;
            }
        }
        
        StartCoroutine(FadeOutCoroutine());
    }

    public void ResetPosition()
    {
        _bodyPosition = _bodyPoint;
        _body.position = _bodyPosition;
    }
}
