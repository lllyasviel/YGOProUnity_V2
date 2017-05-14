using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	public float lifetime = 2.0f;

	void Awake()
	{
		Destroy(gameObject, lifetime);
	}
}
