using UnityEngine;
using System.Collections;

public class FlashingController : HighlightingController
{
	public Color flashingStartColor = Color.blue;
	public Color flashingEndColor = Color.cyan;
	public float flashingDelay = 2.5f;
	public float flashingFrequency = 2f;
	
	//void Start()
	//{
	//	StartCoroutine(DelayFlashing());
	//}

    void OnEnable()
    {
        StartCoroutine(DelayFlashing());
    }

    //public void Act()
    //{
    //    StartCoroutine(DelayFlashing());
    //}
	
	protected IEnumerator DelayFlashing()
	{
		yield return new WaitForSeconds(flashingDelay);
		
		// Start object flashing after delay
		ho.FlashingOn(flashingStartColor, flashingEndColor, flashingFrequency);
	}
}
