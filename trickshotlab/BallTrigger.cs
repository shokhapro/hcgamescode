using UnityEngine;
using UnityEngine.Events;

public class BallTrigger : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private UnityEvent onEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        
        if (!other.TryGetComponent(typeof(Ball), out var c)) return;
        
        onEnter.Invoke();
    }

    public void SetActive(bool value)
    {
        active = value;
    }
}
