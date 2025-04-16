using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GroundVFX : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private float duration = 1f;
    
    private float _elapsedTime;
    private Color _currentColor;
    private bool _isFadeOut;
    private float _inverseDuration;
    private bool isActive = false;

    private void Start()
    {
        _currentColor = _renderer.color;
        _currentColor.a = 0f; // Start fully transparent
        _renderer.color = _currentColor;
        _inverseDuration = 1f / duration;
        StartCoroutine(UpdateColor());
    }

    private IEnumerator UpdateColor()
    {
        while (true)
        {
            if (isActive)
            {
                _elapsedTime += Time.deltaTime;
                
                if (_elapsedTime < duration)
                {
                    float progress = _elapsedTime * _inverseDuration;
                    if (_isFadeOut)
                    {
                        _currentColor.a = Mathf.Lerp(1f, 0f, progress);
                    }
                    else
                    {
                        _currentColor.a = Mathf.Lerp(0f, 1f, progress);
                    }
                    _renderer.color = _currentColor;
                }
                else
                {
                    _elapsedTime = 0f;
                    _isFadeOut = !_isFadeOut;
                }
            }

            yield return null;
        }
    }

    public void StartVFX()
    {
        isActive = true;
        _elapsedTime = 0f;
        _isFadeOut = false;
        _currentColor.a = 0f;
        _renderer.color = _currentColor;
    }
}
