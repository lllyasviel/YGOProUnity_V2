

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class LoopPathAnim : MonoBehaviour 
{
//	public GameObject m_kStartObject = null;
//	public GameObject m_kEndObject = null;

	private MegaShapeArc m_kShapeArc = null;

	public List<MegaWorldPathDeform> m_kMWPD_List = 
		new List<MegaWorldPathDeform>();

	public float 	m_fOffsetValueStep = 1.0f;

	void Start ()
	{
		m_kShapeArc = GetComponent<MegaShapeArc> ();

//		m_kStartObject.transform.position = m_kShapeArc.splines [0].knots [0].p;
//		m_kEndObject.transform.position =
//			m_kShapeArc.splines [0].knots[ m_kShapeArc.splines [0].knots.Count-1 ].p;

		foreach(MegaWorldPathDeform iter in m_kMWPD_List)
		{
			if( iter.GetComponent<AnimUnit>() )
			{
				iter.GetComponent<AnimUnit>().setMegaShape(m_kShapeArc);
				//Debug.Log("set shaps...");
			}
		}
	}
	

	void Update ()
	{

	}

	void LateUpdate()
	{
//		m_kShapeArc.splines [0].knots [0].p = m_kStartObject.transform.position;
//
//		m_kShapeArc.splines [0].knots [m_kShapeArc.splines [0].knots.Count - 1].p =
//			m_kEndObject.transform.position;
	}

	//得到曲线路径长度  --CardGame
	public float getShapeLen()
	{
		return m_kShapeArc.GetCurveLength(0);
	}

	//添加到箭头组成尾部  --CardGame
	public void moveToTail(MegaWorldPathDeform mwpd)
	{
		if( m_kMWPD_List.Contains(mwpd) )
		{
			float val = 
				m_kMWPD_List[m_kMWPD_List.Count-1].
					GetComponent<AnimUnit>().getAccumOffset() + m_fOffsetValueStep;
			mwpd.GetComponent<AnimUnit>().setAccumOffset(val);

		    m_kMWPD_List.Remove(mwpd);
			m_kMWPD_List.Add(mwpd);
		}
	}


}

