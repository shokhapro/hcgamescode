using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    [SerializeField] private bool touchActive = true;
    [SerializeField] private Vector2 xField = new Vector2(-2.5f, 2.5f);
    [SerializeField] private float smooth = 0.85f;
    
    private Transform _t;
    private Plane _raycastPlane;
    private Camera _cam;

    private void Awake()
    {
        Instance = this;
        
        _t = transform;
        
        _raycastPlane = new Plane(Vector3.up, -_t.position.y);
        
        _cam = Camera.main;
    }
    
    private void Update()
    {
        if (!touchActive) return;
        
        if (Input.touchCount == 0) return;

        var t = Input.GetTouch(0);
        
        Ray r = _cam.ScreenPointToRay(t.position);
        
        switch (t.phase)
        {
            case TouchPhase.Began: case TouchPhase.Moved: case TouchPhase.Stationary:
                if (IsPointerOverUIObject()) return;
                
                if (_raycastPlane.Raycast(r, out var d))
                {
                    var p = r.GetPoint(d);

                    var x = Mathf.Clamp(p.x, xField.x, xField.y);

                    _t.position = new Vector3(Mathf.Lerp(x, _t.position.x, smooth), _t.position.y, _t.position.z);
                }
                
                break;
        }
    }
    
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void SetTouchActive(bool value)
    {
        touchActive = value;
    }

    private void FixedUpdate()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(_t.position, Vector3.forward, out hit, 0.5f))
        {
            GateObject gate;
            if (hit.collider.TryGetComponent(out gate))
            {
                gate.OnHit();
            }
        }
    }

    public void ResetPosition()
    {
        _t.position = new Vector3(0, _t.position.y, _t.position.z);
    }
}