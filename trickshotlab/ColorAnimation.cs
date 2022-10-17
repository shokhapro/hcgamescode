using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorAnimation : MonoBehaviour
{
    [SerializeField] Color toColor = Color.red;
    [SerializeField] float duration = 1f;

    private Image _image;
    private SpriteRenderer _sprite;

    private Color _fromColor;
    private Coroutine _coroutine;

    private void Awake()
    {
        TryGetComponent<Image>(out _image);
        TryGetComponent<SpriteRenderer>(out _sprite);

        _fromColor = GetColor();
    }

    private Color GetColor()
    {
        if (_image)
            return _image.color;
        else if (_sprite)
            return _sprite.color;

        return Color.clear;
    }
    
    private void SetColor(Color value)
    {
        if (_image)
            _image.color = value;
        else if (_sprite)
            _sprite.color = value;
    }

    public void Animate()
    {
        _coroutine = StartCoroutine(anim());
    }

    IEnumerator anim()
    {
        var t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            if (t > duration) t = duration;

            var l = t / duration;

            var c = Color.Lerp(_fromColor, toColor, l);
            SetColor(c);
            
            yield return null;
        }
    }

    public void Reset()
    {
        StopCoroutine(_coroutine);

        SetColor(_fromColor);
    }
}
