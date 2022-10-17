using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class TouchControl : MonoBehaviour
{
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool Enabled = true;
    
    public static List<TimeAction> CurrentInactionListeners = new List<TimeAction>();

    private Camera _cam;
    private int _layerMask;
    private ITouchObject _activeObject;
    private Plane _plane;
    private float _lastActionTime;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        
        _layerMask = LayerMask.GetMask("Touch");
    }
    
    private void Update()
    {
        InactionUpdate();
        
        if (!Enabled) return;
        
        if (Input.touchCount <= 0) return;
        
        Touch t = Input.GetTouch(0);
            
        Ray r = _cam.ScreenPointToRay(t.position);

        switch (t.phase)
        {
            case TouchPhase.Began:
            {
                if (IsPointerOverUIObject()) return;
                
                if (Physics.Raycast(r, out var hit, 50f, _layerMask))
                    if (hit.collider.TryGetComponent(typeof(ITouchObject), out var c))
                    {
                        _activeObject = c.GetComponent<ITouchObject>();
                            
                        _plane = new Plane(Vector3.forward, hit.transform.position);
                    }
                    
                if (_activeObject == null) break;

                if (_plane.Raycast(r, out var d))
                {
                    Vector3 pos = r.GetPoint(d);
                                
                    _activeObject.OnStart(pos);
                }

                break;
            }
            case TouchPhase.Moved:
            {
                if (_activeObject == null) break;
                    
                if (_plane.Raycast(r, out var d))
                {
                    Vector3 pos = r.GetPoint(d);
                                
                    _activeObject.OnMoving(pos);
                }

                break;
            }
            case TouchPhase.Ended:
            {
                if (_activeObject == null) break;
                
                _activeObject.OnStop();
                    
                _activeObject = null;
                    
                break;
            }
        }
    }
    
    public bool HasActiveObject => _activeObject != null;

    private void InactionUpdate()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            _lastActionTime = Time.time;

            if (CurrentInactionListeners.Count > 0)
                CurrentInactionListeners.Clear();
        }

        if (CurrentInactionListeners.Count == 0) return;

        float t = Time.time - _lastActionTime;

        for (var i = 0; i < CurrentInactionListeners.Count; i++)
            if (t >= CurrentInactionListeners[i].Time)
            {
                CurrentInactionListeners[i].Action.Invoke();

                CurrentInactionListeners.RemoveAt(i);

                i--;
            }
    }
}

public class TimeAction
{
    public TimeAction(float time, UnityAction action)
    {
        Time = time;
        Action = action;
    }

    public float Time;
    public UnityAction Action;
}
