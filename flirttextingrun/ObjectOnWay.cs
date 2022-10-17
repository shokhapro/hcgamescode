using UnityEngine;

public class ObjectOnWay : MonoBehaviour
{
    private const float movingSpeed = -5;
    private const float destroyLine = -10f;

    private Transform _t;

    private void Awake()
    {
        _t = transform;
    }

    private void FixedUpdate()
    {
        _t.position += new Vector3(0, 0, movingSpeed * Time.fixedDeltaTime);
        
        if (_t.position.z <= destroyLine)
            Destroy(gameObject);
    }
}