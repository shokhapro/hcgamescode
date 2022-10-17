using UnityEngine;
using UnityEngine.Events;

public class LevelObject : MonoBehaviour
{
    [SerializeField] private UnityEvent onPreview;
    [SerializeField] private UnityEvent onRun;
    [SerializeField] private UnityEvent onStop;
    
    public void OnPreview()
    {
        onPreview.Invoke();
    }
    
    public void OnRun()
    {
        onRun.Invoke();
    }
    
    public void OnStop()
    {
        onStop.Invoke();
    }
}
