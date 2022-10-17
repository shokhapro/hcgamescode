using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationPlay : MonoBehaviour
{
    private Animation _animation;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    public void Play()
    {
        if (!_animation) return;
        
        _animation.Play();
    }
}
