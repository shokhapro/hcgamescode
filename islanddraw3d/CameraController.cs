using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 0, 3);
    [SerializeField] private Vector2 moveFactor = new Vector2(1f, 1f);
    [SerializeField] private float moveSmooth = 0.9f;

    private Transform _t;
    private Vector3 _point;
    private Vector3 _position;

    private void Awake()
    {
        _t = transform;
        
        _point = _t.position;
        _position = _point;
    }
    
    private void Update()
    {
        MoveUpdate();
        
        _t.position = Vector3.Lerp(_position, _t.position, moveSmooth);
    }

    private void MoveUpdate()
    {
        if (!target) return;
        
        _position = _point + new Vector3(
            (target.position.x + targetOffset.x - _point.x) * moveFactor.x, 0,
            (target.position.z + targetOffset.z - _point.z) * moveFactor.y);
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
    
    public void ResetTarget()
    {
        target = null;
    }

    public void ResetPosition(bool hard = false)
    {
        _position = _point;
        if (hard) _t.position = _position;
    }
}
