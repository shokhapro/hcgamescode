using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

internal static class CallDelayedAction
{
    public static Coroutine Delay(this MonoBehaviour script, float time, UnityAction action)
    {
        return script.StartCoroutine(DelayCoroutine(time, action));
    }
	
    private static IEnumerator DelayCoroutine(float time, UnityAction action)
    {
        if (time > 0) yield return new WaitForSeconds(time);
        else yield return new WaitForEndOfFrame();

        action.Invoke();
    }
}

public class DelayedAction : MonoBehaviour
{
    [Serializable] public class delayAction
    {
        public float delay = 1f;
        public UnityEvent action = new UnityEvent();
    }

    [SerializeField] private delayAction[] array = new delayAction[1];

    public void Invoke()
    {
        for (int i = 0; i < array.Length; i++)
        {
            float d = array[i].delay;
            if (d < 0) d = 0f;

            this.Delay(d, array[i].action.Invoke);
        }
    }
}
