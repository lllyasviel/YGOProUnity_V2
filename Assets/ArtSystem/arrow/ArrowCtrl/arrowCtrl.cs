using UnityEngine;
using System.Collections;

public class arrowCtrl : MonoBehaviour {

	public GameObject[] m_karrow;
	// Use this for initialization
	void Start () {
	
	
	}
	

	public void AllAlphaZero()
	{
		for(int i = 0;i<m_karrow.Length;i++)
		{
			if(m_karrow[i].GetComponent<AnimUnit>()!=null)
			{
				m_karrow[i].GetComponent<AnimUnit>().SetAllAlphaZero();
			}
		}
	}

}
