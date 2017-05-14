
using UnityEngine;
using System.Collections.Generic;

// Have a pagemesh for each stack then scale to fit pages * thicknessperpage on each side

[ExecuteInEditMode]
public class MegaBook : MonoBehaviour
{
	public GameObject		front;
	public GameObject		back;
	public GameObject		page1;
	public GameObject		page2;
	public GameObject		page3;
	public List<Texture>	pages = new List<Texture>();
	public float			bookalpha;
	public float			covergap = 0.0f;
	public float			pagespace = 0.01f;

	MegaPageFlip			pf1;
	MegaPageFlip			pf2;
	MegaPageFlip			pf3;
	MeshRenderer			mrpg1;
	MeshRenderer			mrpg2;
	MeshRenderer			mrpg3;

	void SetPageTexture(MeshRenderer mr, int i, Texture t)
	{
		if ( mr.sharedMaterials[i].mainTexture != t )
			mr.sharedMaterials[i].mainTexture = t;
	}

	void Update()
	{
		if ( page1 == null || page2 == null || page3 == null )
			return;

		if ( page1 != null && pf1 == null )
			pf1 = page1.GetComponent<MegaPageFlip>();

		if ( mrpg1 == null )
			mrpg1 = page1.GetComponent<MeshRenderer>();

		if ( page2 != null && pf2 == null )
			pf2 = page2.GetComponent<MegaPageFlip>();

		if ( mrpg2 == null )
			mrpg2 = page2.GetComponent<MeshRenderer>();

		if ( page3 != null && pf3 == null )
			pf3 = page3.GetComponent<MegaPageFlip>();

		if ( mrpg3 == null )
			mrpg3 = page3.GetComponent<MeshRenderer>();

		if ( pf1 == null || pf2 == null || pf3 == null || front == null || back == null )
			return;

		int pagecount = (pages.Count / 2) + 2;

		if ( bookalpha < 0.0f )
			bookalpha = 0.0f;

		if ( bookalpha > 100.0f )
			bookalpha = 100.0f;

		if ( front.transform.GetChildCount() > 0 )
		{
			Transform child = front.transform.GetChild(0);
			if ( child != null )
			{
				Vector3 off = Vector3.zero;
				off.y = covergap * 0.5f;
				child.localPosition = off;
			}
		}

		if ( back.transform.GetChildCount() > 0 )
		{
			Transform child = back.transform.GetChild(0);
			if ( child != null )
			{
				Vector3 off = Vector3.zero;
				off.y = -covergap * 0.5f;
				child.localPosition = off;
			}
		}

		float alpha = bookalpha / 100.0f;

		int page = (int)((float)pagecount * alpha);

		float step = 1.0f / (float)pagecount;

		float turn = (alpha % step) / step;

		Vector3 ang = Vector3.zero;

		// Front cover
		if ( page == 0 )
			ang.z = 180.0f * turn;
		else
			ang.z = 180.0f;

		front.transform.localRotation = Quaternion.Euler(ang);

		// Back cover
		if ( page >= pagecount - 1 )
			ang.z = 180.0f * turn;
		else
			ang.z = 0.0f;

		back.transform.localRotation = Quaternion.Euler(ang);

		if ( pagecount < 3 )
			return;

		// Set PageFlip values
		if ( page == 1 )
		{
			pf1.turn = turn * 100.0f;
			pf2.turn = 0.0f;
			pf3.turn = 0.0f;
		}
		else
		{
			if ( page == pagecount - 2 )
			{
				pf1.turn = 100.0f;
				pf2.turn = 100.0f;
				pf3.turn = turn * 100.0f;
			}
			else
			{
				if ( page == 0 )
				{
					pf1.turn = 0.0f;
					pf2.turn = 0.0f;
					pf3.turn = 0.0f;
				}
				else
				{
					if ( page >= pagecount - 1 )
					{
						pf1.turn = 100.0f;
						pf2.turn = 100.0f;
						pf3.turn = 100.0f;
					}
					else
					{
						pf1.turn = 100.0f;
						pf2.turn = turn * 100.0f;
						pf3.turn = 0.0f;
					}
				}
			}
		}

		// Page offsets
		Vector3 poff = Vector3.zero;
		//float po = pagespace;	// * 2.0f;
		poff.y = Mathf.Lerp(pagespace, -pagespace, pf1.turn * 0.01f);
		page1.transform.localPosition = poff;

		poff.y = Mathf.Lerp(0.0f, 0.0f, pf2.turn * 0.01f);
		page2.transform.localPosition = poff;

		poff.y = Mathf.Lerp(-pagespace, pagespace, pf3.turn * 0.01f);
		page3.transform.localPosition = poff;

		// Page textures
		int pg = page - 2;
		if ( pg < 0 )
			pg = 0;

		pg *= 2;

		if ( pg < pages.Count - 1 )
		{
			SetPageTexture(mrpg1, 0, pages[pg]);
			SetPageTexture(mrpg1, 1, pages[pg + 1]);
		}

		if ( pg < pages.Count - 3 )
		{
			if ( pg < pages.Count - 5 )
			{
				SetPageTexture(mrpg2, 0, pages[pg + 2]);
				SetPageTexture(mrpg2, 1, pages[pg + 3]);
			}
		}

		if ( pg < pages.Count - 5 )
		{
			SetPageTexture(mrpg3, 0, pages[pg + 4]);
			SetPageTexture(mrpg3, 1, pages[pg + 5]);
		}
	}
}