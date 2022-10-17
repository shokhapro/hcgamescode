using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

internal static class Delays
{
    public static Coroutine DelayedAction(this MonoBehaviour script, float time, UnityAction action)
    {
        IEnumerator DelayedActionCoroutine()
        {
            if (time > 0) yield return new WaitForSeconds(time);
            else yield return new WaitForEndOfFrame();

            action.Invoke();
        }
        
        return script.StartCoroutine(DelayedActionCoroutine());
    }
    
    [Serializable] public class DelayedEvent
    {
        public float delay = 1f;
        public UnityEvent action = new UnityEvent();

        public IEnumerator InvokeCoroutine()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            else if (delay == 0) yield return new WaitForEndOfFrame();

            action.Invoke();
        }
    }
    
    [Serializable] public class DelayedEvents
    {
        public DelayedEvent[] array = new DelayedEvent[1];
        
        public IEnumerator InvokeCoroutine()
        {
            for (int i = 0; i < array.Length; i++)
                yield return array[i].InvokeCoroutine();
        }
    }
}