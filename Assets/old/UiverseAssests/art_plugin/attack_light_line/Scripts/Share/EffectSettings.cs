using System;
using UnityEngine;

public class EffectSettings : MonoBehaviour
{
	public float ColliderRadius = 0.2f;
	public float EffectRadius = 0;
	public GameObject Target;
	public float MoveSpeed = 1;
	public float MoveDistance = 20;
	public bool IsHomingMove;
	public bool IsVisible = true;
	public bool DeactivateAfterCollision = true;
	public float DeactivateTimeDelay = 4;
	public LayerMask LayerMask = -1;
	
	public event EventHandler<CollisionInfo> CollisionEnter;
	public event EventHandler EffectDeactivated;
	
	private GameObject[] active_key = new GameObject[100];
	private float[] active_value = new float[100];
	private GameObject[] inactive_Key = new GameObject[100];
	private float[] inactive_value = new float[100];
	private int lastActiveIndex;
	private int lastInactiveIndex;
	private int currentActiveGo;
	private int currentInactiveGo;
	private bool deactivatedIsWait;
	
	public void OnCollisionHandler(CollisionInfo e)
	{
		for (int i = 0; i < lastActiveIndex; i++)
		{
			Invoke("SetGoActive", active_value[i]);
		}
		for (int i = 0; i < lastInactiveIndex; i++)
		{
			Invoke("SetGoInactive", inactive_value[i]);
		}
		var handler = CollisionEnter;
		if (handler != null)
			handler(this, e);
		if (DeactivateAfterCollision && !deactivatedIsWait)
		{
			deactivatedIsWait = true;
			Invoke("Deactivate", DeactivateTimeDelay);
		}
	}
	public void OnEffectDeactivatedHandler()
	{
		var handler = EffectDeactivated;
		if (handler != null)
			handler(this, EventArgs.Empty);
	}
	
	public void Deactivate()
	{
		OnEffectDeactivatedHandler();
		gameObject.SetActive(false);
	}
	
	private void SetGoActive()
	{
		active_key[currentActiveGo].SetActive(false);
		++currentActiveGo;
		if (currentActiveGo >= lastActiveIndex) currentActiveGo = 0;
	}
	
	private void SetGoInactive()
	{
		inactive_Key[currentInactiveGo].SetActive(true);
		++currentInactiveGo;
		if (currentInactiveGo >= lastInactiveIndex)
		{
			currentInactiveGo = 0;
		}
	}
	
	public void OnEnable()
	{
		for (int i = 0; i < lastActiveIndex; i++)
		{
			active_key[i].SetActive(true);
		}
		for (int i = 0; i < lastInactiveIndex; i++)
		{
			inactive_Key[i].SetActive(false);
		}
		deactivatedIsWait = false;
	}
	
	public void OnDisable()
	{
		CancelInvoke("SetGoActive");
		CancelInvoke("SetGoInactive");
		CancelInvoke("Deactivate");
		currentActiveGo = 0;
		currentInactiveGo = 0;
	}
	
	public void RegistreActiveElement(GameObject go, float time)
	{
		active_key[lastActiveIndex] = go;
		active_value[lastActiveIndex] = time;
		++lastActiveIndex;
	}
	
	public void RegistreInactiveElement(GameObject go, float time)
	{
		inactive_Key[lastInactiveIndex] = go;
		inactive_value[lastInactiveIndex] = time;
		++lastInactiveIndex;
	}
}

public class CollisionInfo : EventArgs
{
	public RaycastHit Hit;
}