using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBody : MonoBehaviour {

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private List<SpriteRenderer> _renderers;

    [SerializeField]
    private bool _moving;

    [SerializeField]
    private bool _action;

    [SerializeField]
    private Color _defaultColour;

    [SerializeField]
    private Color _flashColour;

    [SerializeField]
    private float _flashStart;

    [SerializeField]
    private float _flashEnd;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _animator.SetBool("Moving", _moving);
        _animator.SetBool("Action", _action);

        FlashFade();
    }

    public bool Moving {
        get {
            return _moving;
        }
        set {
            _moving = value;
        }
    }

    public bool Action {
        get {
            return _action;
        }
        set {
            _action = value;
        }
    }

    [ContextMenu("Flash")]
    public void Flash() {
        FlashColor(_flashColour, 2.0f);
    }

    public void FlashColor(Color colour, float fadeTime) {
        _flashStart = Time.time;
        _flashEnd = _flashStart + fadeTime;
        _flashColour = colour;
    }

    private void FlashFade() {
        float currentTime = Time.time;
        if (currentTime < _flashEnd) {
            float weight = (currentTime - _flashStart) / (_flashEnd - _flashStart);
            Color flashColour = new Color(Mathf.Lerp(_flashColour.r, _defaultColour.r, weight),
                Mathf.Lerp(_flashColour.g, _defaultColour.g, weight),
                Mathf.Lerp(_flashColour.b, _defaultColour.b, weight),
                1.0f);
            foreach (SpriteRenderer renderer in _renderers) {
                renderer.color = flashColour;
            }
        }
    }
}
