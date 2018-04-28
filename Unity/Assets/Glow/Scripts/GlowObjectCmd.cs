using UnityEngine;
using System.Collections.Generic;

public class GlowObjectCmd : MonoBehaviour
{

	public Renderer[] Renderers
	{
		get;
		private set;
	}

	public Color CurrentColor
	{
		get { return _currentColor; }
	}

    [SerializeField]
	private Color _currentColor;

	void Start()
	{
		Renderers = GetComponentsInChildren<Renderer>();
		GlowController.RegisterObject(this);

    }
}
