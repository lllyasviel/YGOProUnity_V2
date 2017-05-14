
using UnityEngine;
using System.IO;

// DONE: return bool to say if mod is valid
// DONE: actually do out of bounds in interp function (spline should know start end tangents)
// DONE: Fix path deform to work for imported models, ie no x rot 270
// DONE: Make path be correct for orientation as in Max
// DONE: Make twist work
// DONE: Make rotate work
// DONE: Pass ends need to work properly, ie extrapolate last tangents
// DONE: Draw gizmo for mods
// DONE: Rotate and twist not working
// TODO: Option to use path in its own space
// TODO: Horrible bug in unity, shape hasnt been created when ongui is called
// TODO: how does scale effect length
// DONE: If closed then need repeat instead of clamp on alpha
[AddComponentMenu("Modifiers/Path Deform")]
public class MegaPathDeform : MegaModifier
{
	public float			percent		= 0.0f;
	public float			stretch		= 1.0f;
	public float			twist		= 0.0f;
	public float			rotate		= 0.0f;
	public MegaAxis			axis		= MegaAxis.X;
	public bool				flip		= false;
	public MegaShape		path		= null;
	public bool				animate		= false;
	public float			speed		= 1.0f;
	public bool				drawpath	= false;
	public float			tangent		= 1.0f;
	[HideInInspector]
	public Matrix4x4		mat			= new Matrix4x4();

	public bool				UseTwistCurve	= false;
	public AnimationCurve	twistCurve		= new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public bool				UseStretchCurve	= false;
	public AnimationCurve	stretchCurve	= new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public override string ModName()	{ return "PathDeform"; }
	public override string GetHelpURL() { return "?page_id=273"; }

	Matrix4x4		wtm = new Matrix4x4();
	Vector3			start;
	Quaternion		tw = Quaternion.identity;

	float usepercent;
	float usetan;
	float ovlen;

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);	// Dont need either, so saving 3 vector mat mults but gaining a mat mult

		float alpha;

		if ( UseStretchCurve )
		{
			float str = stretchCurve.Evaluate(Mathf.Repeat(p.z * ovlen + usepercent, 1.0f)) * stretch;
			alpha = (p.z * ovlen * str) + usepercent;	//(percent / 100.0f);	// can precalc this
		}
		else
		{
			alpha = (p.z * ovlen * stretch) + usepercent;	//(percent / 100.0f);	// can precalc this
		}
		//float alpha = ((p.z * stretch) / path.splines[0].length) + usepercent;	//(percent / 100.0f);	// can precalc this
		//alpha = (p.z * ovlen * stretch) + usepercent;	//(percent / 100.0f);	// can precalc this

		Vector3 ps	= path.InterpCurve3D(0, alpha, path.normalizedInterp) - start;
		Vector3 ps1	= path.InterpCurve3D(0, alpha + usetan, path.normalizedInterp) - start;

		if ( path.splines[0].closed )
			alpha = Mathf.Repeat(alpha, 1.0f);
		else
			alpha = Mathf.Clamp01(alpha);

		if ( UseTwistCurve )
		{
			float twst = twistCurve.Evaluate(alpha) * twist;
			tw = Quaternion.AngleAxis(twst, Vector3.forward);
		}
		else
			tw = Quaternion.AngleAxis(twist * alpha, Vector3.forward);

		Vector3 relativePos = ps1 - ps;
		Quaternion rotation = Quaternion.LookRotation(relativePos) * tw;
		wtm.SetTRS(ps, rotation, Vector3.one);

		wtm = mat * wtm;
		p.z = 0.0f;
		return wtm.MultiplyPoint3x4(p);
		//return invtm.MultiplyPoint3x4(p);
	}

	public override void ModStart(MegaModifiers mc)
	{
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
			percent += speed * Time.deltaTime;

		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		if ( path != null )
		{
			usepercent = percent / 100.0f;
			ovlen = (1.0f / path.splines[0].length);	// * stretch;
			usetan = (tangent * 0.01f);
			//Debug.Log("PathLength " + path.splines[0].length);
			//path.CalcLength(0, 5);
			//Debug.Log("CalcPathLength " + path.splines[0].length);

			//return false;
			//if ( path.beendrawn == false )
				//return false;

			mat = Matrix4x4.identity;
			switch ( axis )
			{
				case MegaAxis.Z: MegaMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); break;
			}
			MegaMatrix.RotateZ(ref mat, Mathf.Deg2Rad * rotate);

			SetAxis(mat);

			start = path.splines[0].knots[0].p;

			Vector3 p1 = path.InterpCurve3D(0, 0.01f, path.normalizedInterp);

			Vector3 up = Vector3.zero;

			switch ( axis )
			{
				case MegaAxis.X: up = Vector3.left; break;
				case MegaAxis.Y: up = Vector3.back; break;
				case MegaAxis.Z: up = Vector3.up; break;
			}

			Quaternion lrot = Quaternion.identity;

			if ( flip )
				up = -up;

			lrot = Quaternion.FromToRotation(p1 - start, up);

			mat.SetTRS(Vector3.zero, lrot, Vector3.one);
			return true;
		}

		return false;
	}

	public void OnDrawGizmos()
	{
		if ( drawpath )
			Display(this);
	}

	// Mmm should be in gizmo code
	void Display(MegaPathDeform pd)
	{
		if ( pd.path != null )
		{
			// Need to do a lookat on first point to get the direction
			pd.mat = Matrix4x4.identity;

			Vector3 p = pd.path.splines[0].knots[0].p;

			Vector3 p1 = pd.path.InterpCurve3D(0, 0.01f, pd.path.normalizedInterp);
			Vector3 up = Vector3.zero;

			switch ( axis )
			{
				case MegaAxis.X: up = Vector3.left; break;
				case MegaAxis.Y: up = Vector3.back; break;
				case MegaAxis.Z: up = Vector3.up; break;
			}

			Quaternion lrot = Quaternion.identity;

			if ( flip )
				up = -up;

			lrot = Quaternion.FromToRotation(p1 - p, up);

			pd.mat.SetTRS(Vector3.zero, lrot, Vector3.one);

			Matrix4x4 mat = pd.transform.localToWorldMatrix * pd.mat;

			for ( int s = 0; s < pd.path.splines.Count; s++ )
			{
				float ldist = pd.path.stepdist;
				if ( ldist < 0.01f )
					ldist = 0.01f;

				float ds = pd.path.splines[s].length / (pd.path.splines[s].length / ldist);

				int c		= 0;
				int k		= -1;
				int lk	= -1;

				Vector3 first = pd.path.splines[s].Interpolate(0.0f, pd.path.normalizedInterp, ref lk) - p;

				for ( float dist = ds; dist < pd.path.splines[s].length; dist += ds )
				{
					float alpha = dist / pd.path.splines[s].length;
					Vector3 pos = pd.path.splines[s].Interpolate(alpha, pd.path.normalizedInterp, ref k) - p;

					if ( (c & 1) == 1 )
						Gizmos.color = pd.path.col1;
					else
						Gizmos.color = pd.path.col2;

					if ( k != lk )
					{
						for ( lk = lk + 1; lk <= k; lk++ )
						{
							Gizmos.DrawLine(mat.MultiplyPoint(first), mat.MultiplyPoint(pd.path.splines[s].knots[lk].p - p));
							first = pd.path.splines[s].knots[lk].p - p;
						}
					}

					lk = k;

					Gizmos.DrawLine(mat.MultiplyPoint(first), mat.MultiplyPoint(pos));

					c++;

					first = pos;
				}

				if ( (c & 1) == 1 )
					Gizmos.color = pd.path.col1;
				else
					Gizmos.color = pd.path.col2;

				if ( pd.path.splines[s].closed )
				{
					Vector3 pos = pd.path.splines[s].Interpolate(0.0f, pd.path.normalizedInterp, ref k) - p;
					Gizmos.DrawLine(mat.MultiplyPoint(first), mat.MultiplyPoint(pos));
				}
			}

			Vector3 p0 = pd.path.InterpCurve3D(0, (percent / 100.0f), pd.path.normalizedInterp) - p;
			p1 = pd.path.InterpCurve3D(0, (percent / 100.0f) + (tangent * 0.01f), pd.path.normalizedInterp) - p;

			Gizmos.color = Color.blue;
			Vector3 sz = new Vector3(pd.path.KnotSize, pd.path.KnotSize, pd.path.KnotSize);
			Gizmos.DrawCube(mat.MultiplyPoint(p0), sz);
			Gizmos.DrawCube(mat.MultiplyPoint(p1), sz);
		}
	}

	public override void DrawGizmo(MegaModContext context)
	{
		//tm = Matrix4x4.identity;
		//Matrix.Translate(ref tm, Offset);
		//invtm = tm.inverse;

		if ( !Prepare(context) )
			return;

		Vector3 min = context.bbox.min;
		Vector3 max = context.bbox.max;

		//Matrix4x4 gtm = Matrix4x4.identity;
		//Vector3 pos = gizmoPos;
		//pos.x = -pos.x;
		//pos.y = -pos.y;
		//pos.z = -pos.z;

		//Vector3 scl = gizmoScale;
		//scl.x = 1.0f - (scl.x - 1.0f);
		//scl.y = 1.0f - (scl.y - 1.0f);
		//gtm.SetTRS(pos, Quaternion.Euler(gizmoRot), scl);

		if ( context.mod.sourceObj != null )
			Gizmos.matrix = context.mod.sourceObj.transform.localToWorldMatrix;	// * gtm;
		else
			Gizmos.matrix = transform.localToWorldMatrix;	// * gtm;

		//Gizmos.color = ModCol();	//Color.yellow;
		corners[0] = new Vector3(min.x, min.y, min.z);
		corners[1] = new Vector3(min.x, max.y, min.z);
		corners[2] = new Vector3(max.x, max.y, min.z);
		corners[3] = new Vector3(max.x, min.y, min.z);

		corners[4] = new Vector3(min.x, min.y, max.z);
		corners[5] = new Vector3(min.x, max.y, max.z);
		corners[6] = new Vector3(max.x, max.y, max.z);
		corners[7] = new Vector3(max.x, min.y, max.z);

		DrawEdge(corners[0], corners[1]);
		DrawEdge(corners[1], corners[2]);
		DrawEdge(corners[2], corners[3]);
		DrawEdge(corners[3], corners[0]);

		DrawEdge(corners[4], corners[5]);
		DrawEdge(corners[5], corners[6]);
		DrawEdge(corners[6], corners[7]);
		DrawEdge(corners[7], corners[4]);

		DrawEdge(corners[0], corners[4]);
		DrawEdge(corners[1], corners[5]);
		DrawEdge(corners[2], corners[6]);
		DrawEdge(corners[3], corners[7]);

		ExtraGizmo(context);
	}

	//public override void DrawGizmo()
	//{
	//Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
	//}
}