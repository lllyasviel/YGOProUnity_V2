using UnityEngine;
using System.Collections;

public class HighlightingController : MonoBehaviour
{
	protected HighlightableObject ho;
	
	void Awake()
	{
		ho = gameObject.AddComponent<HighlightableObject>();
	}
	
	void Update()
	{
		AfterUpdate();
	}
	
	protected virtual void AfterUpdate() {}
}