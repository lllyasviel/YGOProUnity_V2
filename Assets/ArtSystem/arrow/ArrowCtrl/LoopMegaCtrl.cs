using UnityEngine;
using System.Collections;

public class LoopMegaCtrl : MonoBehaviour {

	// Use this for initialization
	private float 		m_offsetZ;
	private float 		m_offsetpercent;
	private float		m_pathLenth;
	public GameObject	Path = null;
	float				m_ObjectLenth;
	float				m_fBeforeLenth = 0.0f;
	public 	int			m_iID;
	public 	GameObject  m_kFirstOb = null;
	bool	m_bneedtoFollow = true;
	public  int			m_iCount = 0;
	void Start () {
		//物体长度  	--CardGame
		m_ObjectLenth = 0.9f*0.02f;
		//this.GetComponent<MegaWorldPathDeform>().animate = false;
		//this.GetComponent<MegaWorldPathDeform>().percent = 0;
	}
	
	// Update is called once per frame
	void Update () {


	}

	void LateUpdate()
	{
		LoopMegaAntmation();
	}

	void LoopMegaAntmation()
	{

		//if(m_offsetZ>0)
		//{
			//获得线段长度		--CardGame
		if(m_bneedtoFollow && m_kFirstOb!=null )
		{
			this.GetComponent<MegaWorldPathDeform>().percent = m_kFirstOb.GetComponent<MegaWorldPathDeform>().percent;
			m_bneedtoFollow = false;
		}

//		if(m_iCount>=5)
//		{
//			m_bneedtoFollow = false;
//			m_iCount = 0;
//		}



		m_pathLenth=Path.GetComponent<MegaShapeArc>().GetCurveLength(0);

//		if(m_fBeforeLenth>m_pathLenth)
//		{
//
//			//this.GetComponent<MegaWorldPathDeform>().Offset -= new Vector3(0,0,(m_fBeforeLenth - m_pathLenth)/5*m_iID);
//			this.GetComponent<MegaWorldPathDeform>().animate = false;
//
//		}
//		else if(m_fBeforeLenth<m_pathLenth)
//		{
//			//this.GetComponent<MegaWorldPathDeform>().Offset -= new Vector3(0,0,(m_fBeforeLenth - m_pathLenth)/5*m_iID);
//			this.GetComponent<MegaWorldPathDeform>().animate = false; 
//		}
//		else
//		{
//				this.GetComponent<MegaWorldPathDeform>().animate = true;
//		}
		m_fBeforeLenth = m_pathLenth;

		m_offsetZ = -this.GetComponent<MegaWorldPathDeform>().Offset.z;
		m_offsetpercent = ((m_ObjectLenth +  m_offsetZ)/m_pathLenth)*100;

		//Debug.Log(this.name+":" + m_offsetpercent);


		this.GetComponent<MegaWorldPathDeform>().path = Path.GetComponent<MegaShapeArc>();

		if(this.GetComponent<MegaWorldPathDeform>().percent >= 100+m_offsetpercent)
		{



			//调整位置方法	--CardGame
			if(m_kFirstOb!=null)
			{
				Debug.Log ("else follow first");
			
				this.GetComponent<MegaWorldPathDeform>().percent = m_kFirstOb.GetComponent<MegaWorldPathDeform>().percent;
				m_bneedtoFollow = true;

			}
			else
			{
				Debug.Log ("first to 0");
				this.GetComponent<MegaWorldPathDeform>().percent = 0;


			}
		}
	}
}
