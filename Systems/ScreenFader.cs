using UnityEngine;
using UnityEngine.UI;

// A simple black screen fader 

[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    float _duration = 1;
    private float _lerpTime = 0f;
    private bool _isLerping = false;
    private Color _startColor;
    private Color _targetColor;
    Image _image;
    
    void Awake()
    {
        _image = GetComponent<Image>();
        _startColor = _image.color;
    }

    public void FadeToColor(Color color, float duration)
    {
        _duration = duration;
        _startColor = _image.color;
        _targetColor = color;
        _isLerping = true;
    }

    public void Update()
    {
        if (_isLerping)
        {
            _lerpTime += Time.deltaTime;
            float t = Mathf.Clamp01(_lerpTime / _duration);
            _image.color = Color.Lerp(_startColor, _targetColor, t);
            
            if (_lerpTime >= _duration)
            {
                _isLerping = false;
                _lerpTime = 0;
            }
        }
    }
}