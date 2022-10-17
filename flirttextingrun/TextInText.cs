using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextInText : MonoBehaviour
{
    [SerializeField] private string startText = "";
    [SerializeField] private string endText = "";
    
    private Text _t;

    public void Set(string text)
    {
        if (!_t) _t = GetComponent<Text>();
        
        _t.text = startText + text + endText;
    }
}
