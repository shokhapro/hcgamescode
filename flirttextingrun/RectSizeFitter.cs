using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectSizeFitter : MonoBehaviour
{
    [SerializeField] private RectTransform fitObject = null;
    [SerializeField] private bool horizontalFit = false;
    [SerializeField] private float horizontalMin = 10f;
    [SerializeField] private float horizontalAdd = 0f;
    [SerializeField] private bool verticalFit = false;
    [SerializeField] private float verticalMin = 10f;
    [SerializeField] private float verticalAdd = 0f;

    private RectTransform _rt;
    private Vector2 _size;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _size = _rt.sizeDelta;
    }

    private void OnGUI()
    {
        if (fitObject != null && fitObject.sizeDelta != _size)
        {
            _size = fitObject.sizeDelta;
            if (_size.x < horizontalMin) _size.x = horizontalMin;
            if (_size.y < verticalMin) _size.y = verticalMin;

            Fit();
        }
    }

    private void Fit()
    {
        if (_rt == null) return;

        if (horizontalFit) _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size.x + horizontalAdd);
        if (verticalFit) _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size.y + verticalAdd);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Fit();
    }
#endif
}
