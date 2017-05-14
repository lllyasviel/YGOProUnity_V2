

using UnityEngine;
using System.Collections;


public class AnimUnit : MonoBehaviour
{
	public float	m_fspeed = 1;
	private MegaWorldPathDeform m_kMWPD = null;
	private MegaShapeArc m_kShapeArc = null;

	private float m_fAccumAnimOffset = 0.0f;
	private float m_fArrowHeadLenth	 = 0.57f;
	//private float m_fRGB			 = 1/255.0f;
	private float m_fHigh			 = 1.0f;		//画线高度定值		--CardGame
	void Start() 
	{
		m_kMWPD = GetComponent<MegaWorldPathDeform> ();
		m_fAccumAnimOffset = m_kMWPD.Offset.z;
		//Debug.Log ("offset val is " + m_kAccumAnimOffset + " when init");

		// this.gameObject.renderer.material = 
		// 	new Material(Shader.Find("Custom/CardSeriesShader"));
		// this.gameObject.renderer.material.mainTexture=(Texture)Resources.Load("Texture/arrow");

		//Color color = new Color(141* m_fRGB, m_fRGB, m_fRGB);
		//this.gameObject.renderer.material.SetColor("_BlendColor",color);
		this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",0);
	}

	void Update()
	{
	
//			float fnowoffset = m_kShapeArc.GetCurveLength (0) - m_fAccumAnimOffset;
//	
//			float linelenth = (m_kShapeArc.GetCurveLength (0) - (m_fArrowHeadLenth*0.04f)/2 + 0.764f)/5;
//			//float linelenth = m_kShapeArc.GetCurveLength (0)/5;
//			//设置alpha
//			if(fnowoffset>0&&fnowoffset<= linelenth)
//			{
//				this.gameObject.renderer.material.SetFloat("_Alpha",fnowoffset/linelenth);
//			}
//			else if(fnowoffset>linelenth && fnowoffset<=linelenth*3)
//			{
//				this.gameObject.renderer.material.SetFloat("_Alpha",1);
//			}
//			else if(fnowoffset>linelenth*3&& fnowoffset<=linelenth*5)
//			{
//				this.gameObject.renderer.material.SetFloat("_Alpha",1-((fnowoffset-linelenth*3.0f)/linelenth));
//			}
//			else 
//			{
//				this.gameObject.renderer.material.SetFloat("_Alpha",0);
//			}
	}


//	void AlphaSet()
//	{
//		m_kShapeArc.GetCurveLength (0);
//	}

	public void SetAllAlphaZero()
	{
		this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",0);
	}


	void FixedUpdate()
	{
		if (m_kShapeArc == null)
		{
			return;
		}

		m_fAccumAnimOffset += -m_fspeed * Time.deltaTime;
		//if (m_fAccumAnimOffset >= m_kShapeArc.GetCurveLength (0)-(m_fArrowHeadLenth*0.02f)/2 - 0.382f)
		if (m_fAccumAnimOffset <= (m_fArrowHeadLenth*0.04f)/2 + 0.764f)
		{
			//m_fAccumAnimOffset = 0.0f;
			this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",0);
			m_kShapeArc.GetComponent<LoopPathAnim>().moveToTail(m_kMWPD);
		}

		m_kMWPD.Offset.z = m_fAccumAnimOffset;

//		float linelenth = (m_kShapeArc.GetCurveLength (0) - (m_fArrowHeadLenth*0.04f)/2 + 0.764f)/5;
//		float lineMax	= m_kShapeArc.GetCurveLength (0);
//		if(m_fAccumAnimOffset<lineMax && m_fAccumAnimOffset>=lineMax - linelenth)
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",m_fAccumAnimOffset - /linelenth);
//		}
//		else if(m_fAccumAnimOffset>linelenth && m_fAccumAnimOffset<=linelenth*3)
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",1);
//		}
//		else if(m_fAccumAnimOffset>linelenth*3&& m_fAccumAnimOffset<=linelenth*5)
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",1-((m_fAccumAnimOffset-linelenth*3.0f)/linelenth));
//		}
//		else 
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",0);
//		}


		float fnowoffset = m_kShapeArc.GetCurveLength (0) - m_fAccumAnimOffset;

		float linelenth = (m_fArrowHeadLenth*0.04f)/2 + 0.764f;


		if(fnowoffset>0&&fnowoffset<= m_fHigh)
		{
			this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",fnowoffset/m_fHigh);
		}
		else if(fnowoffset>m_fHigh && fnowoffset<=m_kShapeArc.GetCurveLength (0) - m_fHigh - linelenth)
		{
			this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",0.7f);
		}
		else if(fnowoffset>m_kShapeArc.GetCurveLength (0) - m_fHigh - linelenth && fnowoffset<=m_kShapeArc.GetCurveLength (0))
		{
            float f = (1 - ((fnowoffset - m_kShapeArc.GetCurveLength(0) + m_fHigh + linelenth) / m_fHigh)) * 0.7f;
            //if (f > 0.05f)
            //{
            //    f = 1;
            //}
            this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",f);
		}
		else 
		{
			this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha",0);
		}




		//float linelenth = m_kShapeArc.GetCurveLength (0)/5;
		//设置alpha
//		if(fnowoffset>0&&fnowoffset<= linelenth)
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",fnowoffset/linelenth);
//		}
//		else if(fnowoffset>linelenth && fnowoffset<=linelenth*3)
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",1);
//		}
//		else if(fnowoffset>linelenth*3&& fnowoffset<=linelenth*4)
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",1-((fnowoffset-linelenth*3.0f)/linelenth));
//		}
//		else 
//		{
//			this.gameObject.renderer.material.SetFloat("_Alpha",0);
//		}
	}
	
	public void setMegaShape(MegaShapeArc msa)
	{
		m_kShapeArc = msa;
	}

	public float getAccumOffset()
	{
		return m_fAccumAnimOffset;
	}

	public void setAccumOffset(float val)
	{
		m_fAccumAnimOffset = val; 
	}
}

