using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextCountAnimation : MonoBehaviour
{
    public int value = 0;
    public int plus = 0;

    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }
    
    private void OnEnable()
    {
        _text.text = value.ToString();
        
        StartCoroutine(anim());
    }
    
    IEnumerator anim()
    {
        var v2 = value + plus;
        
        var t = 0f;
        var t2 = 0.3f;

        while (t < t2)
        {
            t += Time.deltaTime;
            if (t > t2) t = t2;

            var l = t / t2;

            float n = Mathf.Lerp(value, v2, l);
            
            _text.text = Mathf.FloorToInt(n).ToString();
            
            yield return null;
        }
    }
}
