using UnityEngine;

public class TransformRandomizer : MonoBehaviour
{
    [SerializeField] private bool randActive = false;
    [SerializeField] private Vector2 randPosShift = new Vector2(1, 1);
    [SerializeField] private float randRotShift = 360f;

    private Transform _t;
    private Vector3 _pos;
    private Vector3 _rot;

    private void Awake()
    {
        _t = transform;

        _pos = _t.position;
        _rot = _t.eulerAngles;
    }
    
    public void Randomize()
    {
        if (randActive) gameObject.SetActive(Random.Range(0, 2) == 1);
        _t.position = _pos + new Vector3(randPosShift.x * Random.Range(-0.5f, 0.5f), 0, randPosShift.y * Random.Range(-0.5f, 0.5f));
        _t.eulerAngles = _rot + new Vector3(0, randRotShift * Random.Range(-0.5f, 0.5f), 0);
    }
}
