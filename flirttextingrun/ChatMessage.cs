using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private RectTransform frame;

    public void Set(string txt)
    {
        text.text = txt;
    }

    public float GetFrameHeight()
    {
        return frame.sizeDelta.y;
    }
}
