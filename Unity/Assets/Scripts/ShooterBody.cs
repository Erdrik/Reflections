using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBody : MonoBehaviour {

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private bool _moving;

    [SerializeField]
    private bool _action;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _animator.SetBool("Moving", _moving);
        _animator.SetBool("Action", _action);
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
}
