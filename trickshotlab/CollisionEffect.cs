using UnityEngine;

public class CollisionEffect : MonoBehaviour
{
    [SerializeField] private float impulseThreshold = 3f;
    [Space]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float sizeMin = 0.2f;
    [SerializeField] private float sizeFactor = 0.1f;
    [SerializeField] private float duration = 0.1f;
    [Space]
    [SerializeField] private AudioSource sound;
    
    private Transform _sTransform;
    private Coroutine _sCoroutine;

    private static CollisionEffect _instance;

    private void Awake()
    {
        _instance = this;
        
        if (sprite)
        {
            sprite.enabled = false;
            
            _sTransform = sprite.transform;
        }
    }

    public static void Spawn(ContactPoint2D contact)
    {
        if (!_instance) return;
        
        _instance.spawn(contact);
    }
    
    private void spawn(ContactPoint2D contact)
    {
        if (contact.normalImpulse < impulseThreshold) return;
        
        if (sprite)
        {
            _sTransform.SetPositionAndRotation(contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
            _sTransform.localScale = Vector3.one * sizeMin * (1f + contact.normalImpulse * sizeFactor);

            if (_sCoroutine != null)
                StopCoroutine(_sCoroutine);
            sprite.enabled = true;
            _sCoroutine = this.Delay(duration, () =>
            {
                sprite.enabled = false;

                _sCoroutine = null;
            });
        }

        if (sound)
            sound.Play();
    }
}
