using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    [SerializeField] private Vector2 speed = new Vector2(0.1f, 0.1f);
    [SerializeField] private Vector2 secondaryTextureSpeed = Vector2.zero;

    private Renderer _rend;
    
    void Start()
    {
        _rend = GetComponent<Renderer>();
    }
    
    void Update()
    {
        _rend.material.SetTextureOffset("_MainTex", speed * Time.time);
        if (secondaryTextureSpeed != Vector2.zero)
            _rend.material.SetTextureOffset("_DetailAlbedoMap", secondaryTextureSpeed * Time.time);
    }
}