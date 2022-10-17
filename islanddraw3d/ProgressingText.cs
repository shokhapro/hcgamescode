using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressingText : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private string prefix = "";
    [SerializeField] private int fromInt = 0;
    [SerializeField] private int toInt = 100;
    [SerializeField] private string postfix = "";
    [SerializeField] private float time = 1f;

    public void SetValues(int from, int to)
    {
        fromInt = from;
        toInt = to;
        
        text.text = prefix + fromInt + postfix;
    }

    public void Progress()
    {
        IEnumerator Progressing()
        {
            var t = 0f;
            while (t < time)
            {
                t += Time.deltaTime;

                var l = Mathf.Clamp01(t / time);
                var val = Mathf.FloorToInt(Mathf.Lerp(fromInt, toInt, l));

                text.text = prefix + val + postfix;
            
                yield return null;
            }
        }

        StartCoroutine(Progressing());
    }
}
