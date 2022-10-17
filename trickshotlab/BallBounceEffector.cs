using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BallBounceEffector : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float effectFactor = 0.25f;
    
    private Rigidbody2D _brb;
    private Coroutine _delay;
    private bool _effect = false;

    private void Awake()
    {
        var b = FindObjectOfType<Ball>();

        if (!b)
        {
            gameObject.SetActive(false);

            return;
        }

        _brb = b.GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _brb.gameObject)
            _delay = this.Delay(1f, () => _effect = true);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == _brb.gameObject)
        {
            StopCoroutine(_delay);

            _effect = false;
        }
    }
    
    private void FixedUpdate()
    {
        if (_effect)
        {
            var v = _brb.velocity;
            
            /*if (v.y == 0f)
            {
                _effect = false;

                return;
            }*/
            
            if (v.y > 0f) v.y *= 1f - effectFactor;
            
            _brb.velocity = v;
        }
    }
}
