using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Decorator : MonoBehaviour
{
    public static Decorator Instance;
    
    private IslandMesh _island;
    private Camera _camera;
    private Transform _t;
    [SerializeField] private Collider bounds;
    [SerializeField] private Collider viewBounds;
    [SerializeField] private Vector2 viewMoveFactor = new Vector2(5f, 5f);
    [SerializeField] private Transform objectsParent;
    [SerializeField] private Vector2 objectPositionOffset = new Vector2(0f, 1f);
    [SerializeField] private Vector2 objectPositionRandom = new Vector2(1f, 1f);
    [SerializeField] private float objectFallingHeight = 7.5f;
    [SerializeField] private Delays.DelayedEvents onFirstObject;
    private Plane _raycastPlane;
    private int _touchLayerMask;
    private float _clickDeltaTime = 0.2f;
    
    private bool _touchActive = true;
    private bool _touchBegan = false;
    private Transform _touchObject;
    private Vector3 _touchOffset;
    private float[] _touchChanges = new float[4];
    private bool _firstObject = true;
    private Vector3 _initPos;

    void Awake()
    {
        Instance = this;
        
        _island = FindObjectOfType<IslandMesh>();
        
        _camera = Camera.main;
        
        _t = transform;

        _initPos = _t.position;
        
        _touchLayerMask = LayerMask.GetMask("Object");
        
        _raycastPlane = new Plane(Vector3.up, -_t.position.y);
    }

    private void Update()
    {
        TouchUpdate();
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

                Physics.Raycast(r, out var hit, 25f, _touchLayerMask);
                _touchObject = hit.collider ? hit.collider.transform : null;

                ObjectRemoveUpdate();
                
                Move(true);
                break;
            case TouchPhase.Moved: case TouchPhase.Stationary:
                if (!_touchBegan) return;
                
                Move();
                break;
            case TouchPhase.Ended:
                if (!_touchBegan) return;
                
                ObjectRemoveUpdate();
                
                _touchBegan = false;
                break;
        }

        void Move(bool began = false)
        {
            if (_touchObject)
            {
                if (_raycastPlane.Raycast(r, out var d))
                {
                    var p = r.GetPoint(d);

                    if (began) _touchOffset = _touchObject.position - p;
                    p += _touchOffset;
                
                    var b = bounds.bounds;
                    p.x = Mathf.Clamp(p.x, b.min.x, b.max.x);
                    p.z = Mathf.Clamp(p.z, b.min.z, b.max.z);

                    p.y = _island.GetHeightOnPoint(p);

                    _touchObject.position = p;
                }
            }
            else
            {
                var dx = t.deltaPosition.x / Screen.width;
                var dy = t.deltaPosition.y / Screen.width;

                var p = _t.position - new Vector3(dx * viewMoveFactor.x, 0, dy * viewMoveFactor.y);

                var b = viewBounds.bounds;
                p.x = Mathf.Clamp(p.x, b.min.x, b.max.x);
                p.z = Mathf.Clamp(p.z, b.min.z, b.max.z);

                _t.position = p;
            }
        }

        void ObjectRemoveUpdate()
        {
            if (!_touchObject) return;
                
            _touchChanges[3] = _touchChanges[2];
            _touchChanges[2] = _touchChanges[1];
            _touchChanges[1] = _touchChanges[0];
            _touchChanges[0] = Time.time;

            if (_touchChanges[1] == 0f || _touchChanges[0] - _touchChanges[1] > _clickDeltaTime) return;
            if (_touchChanges[2] == 0f || _touchChanges[1] - _touchChanges[2] > _clickDeltaTime) return;
            if (_touchChanges[3] == 0f || _touchChanges[2] - _touchChanges[3] > _clickDeltaTime) return;
            
            Destroy(_touchObject.gameObject);
        }
    }
    
    public void TouchActive(bool value)
    {
        _touchActive = value;

        if (!value)
        {
            _touchBegan = false;
            
            _firstObject = true;
        }
    }

    public GameObject AddObject(GameObject prefab, float randrot)
    {
        var obj = Instantiate(prefab, objectsParent);

        obj.transform.position = _t.position +
                                 new Vector3(objectPositionOffset.x + objectPositionRandom.x * Random.Range(-0.5f, 0.5f), 0,
                                     objectPositionOffset.y + objectPositionRandom.y * Random.Range(-0.5f, 0.5f));
        
        obj.transform.eulerAngles = new Vector3(0, randrot * Random.Range(-0.5f, 0.5f), 0);

        IEnumerator ObjectFadeIn(GameObject o)
        {
            var ot = o.transform;
            var p1 = ot.position;
            p1.y = _island.GetHeightOnPoint(p1);
            var p0 = new Vector3(p1.x, objectFallingHeight, p1.z);
            var t1 = 0.5f;
            var t = 0f;
            var oc = o.GetComponent<Collider>();

            if (oc) oc.enabled = false;

            while (t < t1)
            {
                t += Time.deltaTime;

                var l = Mathf.Clamp01(t / t1);
                
                ot.position = Vector3.Lerp(p0, p1, l);

                yield return null;
            }
            
            if (oc) oc.enabled = true;
        }

        StartCoroutine(ObjectFadeIn(obj));
        
        if (_firstObject)
        {
            _firstObject = false;

            StartCoroutine(onFirstObject.InvokeCoroutine());
        }

        return obj;
    }

    public void Clear()
    {
        for (var i = 0; i < objectsParent.childCount; i++)
            Destroy(objectsParent.GetChild(i).gameObject);
    }
    
    public void ResetPosition()
    {
        _t.position = _initPos;
    }
}
