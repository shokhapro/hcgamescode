using System.Collections;
using UnityEngine;

public class ProgressingScale : MonoBehaviour
{
    [SerializeField] private Vector3 fromScale = Vector3.zero;
    [SerializeField] private Vector3 toScale = Vector3.one;
    [SerializeField] private float time = 1f;

    public void SetValues(Vector3 from, Vector3 to)
    {
        fromScale = from;
        toScale = to;

        transform.localScale = fromScale;
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
                var val = Vector3.Lerp(fromScale, toScale, l);

                transform.localScale = val;
            
                yield return null;
            }
        }

        StartCoroutine(Progressing());
    }
}
