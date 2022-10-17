using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Transform _t;
    private Rigidbody2D _rb;
    private bool _isRunning;
    private Vector3 _startPos;

    private void Awake()
    {
        _t = transform;
        
        _rb = GetComponent<Rigidbody2D>();

        _rb.simulated = false;

        _isRunning = false;
    }

    public void Run()
    {
        if (_isRunning) return;
        _isRunning = true;
        
        _startPos = _t.position;

        _rb.simulated = true;
    }
    
    public void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;

        _rb.simulated = false;
        Relax();

        this.Delay(0.05f, () => { _t.position += Vector3.up * 0.0001f; });
        this.Delay(0.1f, () => { _t.position = _startPos; });
    }

    public void Relax()
    {
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;
    }
    
    public void AddForce(Vector2 value)
    {
        _rb.AddForce(value);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CollisionEffect.Spawn(other.GetContact(0));
    }
}
