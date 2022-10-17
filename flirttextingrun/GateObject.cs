using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GateObject : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private TextMeshPro textmesh;
    public UnityEvent onHit;
    
    public void Set(string txt)
    {
        text = txt;
        
        textmesh.text = text;
        
        //random color
    }

    public void OnHit()
    {
        ChatManager.Instance.AddMessage(false, text);
        
        //podsvetkax
        
        onHit.Invoke();
    }
}
