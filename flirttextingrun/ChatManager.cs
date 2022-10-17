using System.Collections;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    [SerializeField] private RectTransform window;
    [SerializeField] private ChatMessage messageMePrefab;
    [SerializeField] private ChatMessage messageOppPrefab;
    [SerializeField] private ChatMessage messageOppWinPrefab;
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private AudioSource messageSound;

    private Transform _t;
    private RectTransform _rt;
    private float _length = 170f;
    private Coroutine _scrollingCoroutine = null;

    private void Awake()
    {
        Instance = this;

        _t = transform;
        _rt = GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateScroll();
    }

    private void UpdateScroll()
    {
        float h = _length;

        if (h < window.rect.height) h = window.rect.height;

        if (_scrollingCoroutine != null) StopCoroutine(_scrollingCoroutine);
        _scrollingCoroutine = StartCoroutine(scrolling());

        IEnumerator scrolling()
        {
            var t1 = 0.5f;
            var t = 0f;
            var h0 = _rt.sizeDelta.y;
            while (t < t1)
            {
                t += Time.deltaTime;
                if (t >= t1) t = t1;
                
                _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, Mathf.Lerp(h0, h, t/t1));

                yield return null;
            }

            _scrollingCoroutine = null;
        }
    }
    
    public void AddMessage(bool fromOpp, string text, bool win = false)
    {
        this.DelayedAction(delay, add);

        void add()
        {
            ChatMessage m = Instantiate(fromOpp ? (win ? messageOppWinPrefab : messageOppPrefab) : messageMePrefab, _t);
            m.Set(text);
            m.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -_length);

            _length += m.GetFrameHeight() + spacing;
        
            UpdateScroll();
            
            messageSound.Play();
        }
    }

    public void Clear()
    {
        StopAllCoroutines();
        
        var ms = GetComponentsInChildren<ChatMessage>();
        for (var i = 0; i < ms.Length; i++)
            Destroy(ms[i].gameObject);
        
        _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, window.rect.height);
        _length = 170f;
    }
}
