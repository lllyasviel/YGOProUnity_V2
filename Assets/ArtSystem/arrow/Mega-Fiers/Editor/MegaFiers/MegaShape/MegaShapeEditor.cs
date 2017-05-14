
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

// TODO: SVG import
// TODO: Button to recalc lengths
// TEST: Build a simple scene in max then have a road, barrier, fence etc

[CustomEditor(typeof(MegaShape))]
public class MegaShapeEditor : Editor
{
	int		selected = -1;
	Vector3 pm = new Vector3();
	Vector3 delta = new Vector3();

	bool showsplines = false;
	bool showknots = false;

	float ImportScale = 1.0f;
	static public Vector3 CursorPos = Vector3.zero;	//0.0f;
	static public Vector3 CursorSpline = Vector3.zero;	//0.0f;
	static public Vector3 CursorTangent = Vector3.zero;	//0.0f;

	public virtual bool Params()	{ return false; }

	public bool showcommon = true;

	public override void OnInspectorGUI()
	{
		bool buildmesh = false;
		MegaShape shape = (MegaShape)target;

		EditorGUILayout.BeginHorizontal();

		if ( GUILayout.Button("Add Knot") )
		{
			if ( shape.splines == null || shape.splines.Count == 0 )
			{
				MegaSpline spline = new MegaSpline();	// Have methods for these
				shape.splines.Add(spline);
			}

			//Undo.RegisterUndo(target, "Add Knot");

			MegaKnot knot = new MegaKnot();
#if true
			// Add a point at CursorPos

			//sp = selected + 1;
			//Debug.Log("CursorPos " + CursorPos + " CursorKnot " + CursorKnot);
			float per = CursorPercent * 0.01f;

			CursorTangent = shape.splines[0].Interpolate(per + 0.01f, true, ref CursorKnot);	//this.GetPositionOnSpline(i) - p;
			CursorPos = shape.splines[0].Interpolate(per, true, ref CursorKnot);	//this.GetPositionOnSpline(i) - p;

			knot.p = CursorPos;
			//CursorTangent = 
			//Vector3 t = shape.splines[0].knots[selected].Interpolate(0.51f, shape.splines[0].knots[0]);
			knot.outvec = (CursorTangent - knot.p);
			knot.outvec.Normalize();
			knot.outvec *= shape.splines[0].knots[CursorKnot].seglength * 0.25f;
			knot.invec = -knot.outvec;
			knot.invec += knot.p;
			knot.outvec += knot.p;

			shape.splines[0].knots.Insert(CursorKnot + 1, knot);
#else
			int sp = 0;

			if ( selected == -1 || shape.splines[0].knots.Count == 1 )
			{
				shape.splines[0].knots.Add(knot);
				selected = shape.splines[0].knots.Count - 1;
			}
			else
			{
				if ( selected < shape.splines[0].knots.Count - 1 )
				{
					sp = selected + 1;
					knot.p = shape.splines[0].knots[selected].Interpolate(0.5f, shape.splines[0].knots[selected + 1]);
					Vector3 t = shape.splines[0].knots[selected].Interpolate(0.51f, shape.splines[0].knots[selected + 1]);
					knot.outvec = (t - knot.p);	//.Normalize();
					knot.outvec.Normalize();
					knot.outvec *= shape.splines[0].knots[selected].seglength * 0.25f;
					knot.invec = -knot.outvec;
					knot.invec += knot.p;
					knot.outvec += knot.p;
				}
				else
				{
					if ( shape.splines[0].closed )
					{
						sp = selected + 1;
						knot.p = shape.splines[0].knots[selected].Interpolate(0.5f, shape.splines[0].knots[0]);
						Vector3 t = shape.splines[0].knots[selected].Interpolate(0.51f, shape.splines[0].knots[0]);
						knot.outvec = (t - knot.p);	//.Normalize();
						knot.outvec.Normalize();
						knot.outvec *= shape.splines[0].knots[selected].seglength * 0.25f;
						knot.invec = -knot.outvec;
						knot.invec += knot.p;
						knot.outvec += knot.p;
					}
					else
					{
						sp = selected - 1;

						//Debug.Log("selected " + selected + " count " + shape.splines[0].knots.Count + " sp " + sp);
						knot.p = shape.splines[0].knots[sp].Interpolate(0.5f, shape.splines[0].knots[sp + 1]);
						Vector3 t = shape.splines[0].knots[sp].Interpolate(0.51f, shape.splines[0].knots[sp + 1]);
						knot.outvec = (t - knot.p);	//.Normalize();
						knot.outvec.Normalize();
						knot.outvec *= shape.splines[0].knots[sp].seglength * 0.25f;
						knot.invec = -knot.outvec;
						knot.invec += knot.p;
						knot.outvec += knot.p;
						sp++;
					}
				}

				shape.splines[0].knots.Insert(sp, knot);
				selected = sp;	//++;
			}
#endif
			shape.CalcLength(10);
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}

		if ( GUILayout.Button("Delete Knot") )
		{
			if ( selected != -1 )
			{
				//Undo.RegisterUndo(target, "Delete Knot");
				shape.splines[0].knots.RemoveAt(selected);
				selected--;
				shape.CalcLength(10);
			}
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();

		if ( GUILayout.Button("Match Handles") )
		{
			if ( selected != -1 )
			{
				//Undo.RegisterUndo(target, "Match Handles");

				Vector3 p = shape.splines[0].knots[selected].p;
				Vector3 d = shape.splines[0].knots[selected].outvec - p;
				shape.splines[0].knots[selected].invec = p - d;
				shape.CalcLength(10);
			}
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}

		if ( GUILayout.Button("Load") )
		{
			// Load a spl file from max, so delete everything and replace
			LoadShape(ImportScale);
			buildmesh = true;
		}

		EditorGUILayout.EndHorizontal();

		showcommon = EditorGUILayout.Foldout(showcommon, "Common Params");

		bool rebuild = false;	//Params();

		if ( showcommon )
		{
			//CursorPos = EditorGUILayout.Vector3Field("Cursor", CursorPos);
			CursorPercent = EditorGUILayout.FloatField("Cursor", CursorPercent);
			CursorPercent = Mathf.Repeat(CursorPercent, 100.0f);

			ImportScale = EditorGUILayout.FloatField("Import Scale", ImportScale);

			MegaAxis av = (MegaAxis)EditorGUILayout.EnumPopup("Axis", shape.axis);
			if ( av != shape.axis )
			{
				shape.axis = av;
				rebuild = true;
			}

			shape.col1 = EditorGUILayout.ColorField("Col 1", shape.col1);
			shape.col2 = EditorGUILayout.ColorField("Col 2", shape.col2);

			shape.KnotCol = EditorGUILayout.ColorField("Knot Col", shape.KnotCol);
			shape.HandleCol = EditorGUILayout.ColorField("Handle Col", shape.HandleCol);
			shape.VecCol = EditorGUILayout.ColorField("Vec Col", shape.VecCol);

			shape.KnotSize = EditorGUILayout.FloatField("Knot Size", shape.KnotSize);
			shape.stepdist = EditorGUILayout.FloatField("Step Dist", shape.stepdist);

			if ( shape.stepdist < 0.01f )
				shape.stepdist = 0.01f;

			shape.normalizedInterp = EditorGUILayout.Toggle("Normalized Interp", shape.normalizedInterp);
			shape.drawHandles = EditorGUILayout.Toggle("Draw Handles", shape.drawHandles);
			shape.drawKnots = EditorGUILayout.Toggle("Draw Knots", shape.drawKnots);
			shape.drawspline = EditorGUILayout.Toggle("Draw Spline", shape.drawspline);
			shape.lockhandles = EditorGUILayout.Toggle("Lock Handles", shape.lockhandles);

			shape.animate = EditorGUILayout.Toggle("Animate", shape.animate);
			if ( shape.animate )
			{
				shape.time = EditorGUILayout.FloatField("Time", shape.time);
				shape.MaxTime = EditorGUILayout.FloatField("Loop Time", shape.MaxTime);
				shape.speed = EditorGUILayout.FloatField("Speed", shape.speed);
				shape.LoopMode = (MegaRepeatMode)EditorGUILayout.EnumPopup("Loop Mode", shape.LoopMode);
			}

			// Mesher
			shape.makeMesh = EditorGUILayout.Toggle("Make Mesh", shape.makeMesh);

			if ( shape.makeMesh )
			{
				shape.meshType = (MeshShapeType)EditorGUILayout.EnumPopup("Mesh Type", shape.meshType);

				shape.Pivot = EditorGUILayout.Vector3Field("Pivot", shape.Pivot);

				shape.CalcTangents = EditorGUILayout.Toggle("Calc Tangents", shape.CalcTangents);
				shape.GenUV = EditorGUILayout.Toggle("Gen UV", shape.GenUV);
				shape.PhysUV = EditorGUILayout.Toggle("Physical UV", shape.PhysUV);
				shape.UVOffset = EditorGUILayout.Vector2Field("UV Offset", shape.UVOffset);
				shape.UVRotate = EditorGUILayout.Vector2Field("UV Rotate", shape.UVRotate);
				shape.UVScale = EditorGUILayout.Vector2Field("UV Scale", shape.UVScale);
				shape.UVOffset1 = EditorGUILayout.Vector2Field("UV Offset1", shape.UVOffset1);
				shape.UVRotate1 = EditorGUILayout.Vector2Field("UV Rotate1", shape.UVRotate1);
				shape.UVScale1 = EditorGUILayout.Vector2Field("UV Scale1", shape.UVScale1);

				switch ( shape.meshType )
				{
					case MeshShapeType.Fill:
						shape.DoubleSided = EditorGUILayout.Toggle("Double Sided", shape.DoubleSided);
						shape.Height = EditorGUILayout.FloatField("Height", shape.Height);
						shape.HeightSegs = EditorGUILayout.IntField("HeightSegs", shape.HeightSegs);
						shape.UseHeightCurve = EditorGUILayout.Toggle("Use Height Crv", shape.UseHeightCurve);
						if ( shape.UseHeightCurve )
							shape.heightCrv = EditorGUILayout.CurveField("Height Curve", shape.heightCrv);
						break;

					case MeshShapeType.Line:
						shape.DoubleSided = EditorGUILayout.Toggle("Double Sided", shape.DoubleSided);
						shape.Height = EditorGUILayout.FloatField("Height", shape.Height);
						shape.HeightSegs = EditorGUILayout.IntField("HeightSegs", shape.HeightSegs);
						shape.heightCrv = EditorGUILayout.CurveField("Height Curve", shape.heightCrv);
						shape.Start = EditorGUILayout.FloatField("Start", shape.Start);
						shape.End = EditorGUILayout.FloatField("End", shape.End);
						shape.Rotate = EditorGUILayout.FloatField("Rotate", shape.Rotate);
						break;

					case MeshShapeType.Tube:
						shape.Sides = EditorGUILayout.IntField("Sides", shape.Sides);
						shape.TubeStep = EditorGUILayout.FloatField("TubeStep", shape.TubeStep);
						shape.Start = EditorGUILayout.FloatField("Start", shape.Start);
						shape.End = EditorGUILayout.FloatField("End", shape.End);
						break;
				}
			}

			showsplines = EditorGUILayout.Foldout(showsplines, "Splines");

			if ( showsplines )
			{
				for ( int i = 0; i < shape.splines.Count; i++ )
				{
					DisplaySpline(shape, shape.splines[i]);
				}
			}
		}

		if ( Params() )
		{
			rebuild = true;
		}

		if ( GUI.changed )
		{
			EditorUtility.SetDirty(target);
			//shape.CalcLength(10);
			buildmesh = true;
		}

		if ( rebuild )
		{
			shape.MakeShape();
			EditorUtility.SetDirty(target);
			buildmesh = true;
		}

		if ( buildmesh )
		{
			shape.BuildMesh();
		}
	}

	void DisplayKnot(MegaShape shape, MegaSpline spline, MegaKnot knot)
	{
		bool recalc = false;

		Vector3 p = EditorGUILayout.Vector3Field("Pos", knot.p);
#if false
		Vector3 invec = EditorGUILayout.Vector3Field("In", knot.invec);
		//Vector3 outvec = EditorGUILayout.Vector3Field("Out", knot.outvec);

		if ( invec != knot.invec )
		{
			if ( shape.lockhandles )
			{
				Vector3 d = invec - knot.invec;
				knot.outvec -= d;
			}

			knot.invec = invec;
			recalc = true;
		}

		Vector3 outvec = EditorGUILayout.Vector3Field("Out", knot.outvec);

		if ( outvec != knot.outvec )
		{
			if ( shape.lockhandles )
			{
				Vector3 d = outvec - knot.outvec;
				knot.invec -= d;
			}

			knot.outvec = outvec;
			recalc = true;
		}

		//Vector3 p = EditorGUILayout.Vector3Field("Pos", knot.p);
#endif
		delta = p - knot.p;

		knot.invec += delta;
		knot.outvec += delta;

		if ( knot.p != p )
		{
			recalc = true;
			knot.p = p;
		}

		if ( recalc )
		{
			shape.CalcLength(10);
		}
	}

	void DisplaySpline(MegaShape shape, MegaSpline spline)
	{
		bool closed = EditorGUILayout.Toggle("Closed", spline.closed);

		if ( closed != spline.closed )
		{
			spline.closed = closed;
			shape.CalcLength(10);
			EditorUtility.SetDirty(target);
			//shape.BuildMesh();
		}

		EditorGUILayout.LabelField("Length ", spline.length.ToString("0.000"));

		showknots = EditorGUILayout.Foldout(showknots, "Knots");

		if ( showknots )
		{
			for ( int i = 0; i < spline.knots.Count; i++ )
			{
				DisplayKnot(shape, spline, spline.knots[i]);
				//EditorGUILayout.Separator();
			}
		}
	}

	public void OnSceneGUI()
	{
		Undo.RegisterUndo(target, "Move Shape Points");

		MegaShape shape = (MegaShape)target;
		float ksize = shape.KnotSize * 0.01f;

		float ringratio = 1.3f;
		float borderratio = 1.15f;
		float knotsize = ksize;
		//float ringsize = knotsize1.1f;
		float handlesize = knotsize * 0.75f;
		//float outdiscsize = ksize * 1.0f * 0.5f;

		Handles.matrix = shape.transform.localToWorldMatrix;

		bool recalc = false;

		Vector3 dragplane = Vector3.one;
		Vector3 camfwd = Camera.current.transform.forward;

		if ( Mathf.Abs(camfwd.x) > Mathf.Abs(camfwd.y) )
		{
			if ( Mathf.Abs(camfwd.x) > Mathf.Abs(camfwd.z) )
				dragplane.x = 0.0f;
			else
				dragplane.z = 0.0f;
		}
		else
		{
			if ( Mathf.Abs(camfwd.y) > Mathf.Abs(camfwd.z) )
				dragplane.y = 0.0f;
			else
				dragplane.z = 0.0f;
		}
		//Debug.Log("Dragplane " + dragplane);

		Color nocol = new Color(0, 0, 0, 0);
		for ( int s = 0; s < shape.splines.Count; s++ )
		{
			for ( int p = 0; p < shape.splines[s].knots.Count; p++ )
			{
				Vector3 pp = shape.transform.TransformPoint(shape.splines[s].knots[p].p);

				Vector3 normal = Camera.current.transform.forward;
				if ( shape.drawKnots )	//&& recalc == false )
				{
					pm = shape.splines[s].knots[p].p;

					Handles.color = Color.black;
					Handles.color = shape.VecCol;

					//Vector3 normal = (pp - Camera.current.transform.position).normalized;
					Handles.DrawSolidDisc(pp, normal, knotsize * borderratio);	//ksize * 0.55f);

					if ( p == selected )
					{
						Handles.color = Color.white;
						Handles.Label(pm, " Selected\n" + pm.ToString("0.000"));
					}
					else
					{
						Handles.color = shape.KnotCol;
						Handles.Label(pm, " " + p);
					}

					//if ( p == selected )
					//shape.splines[s].knots[p].p = Handles.PositionHandle(pm, Quaternion.identity);
					//else
					//{

					Handles.color = shape.KnotCol;
					Handles.DrawSolidDisc(pp, normal, knotsize);

					//shape.splines[s].knots[p].p = Handles.FreeMoveHandle(pm, Quaternion.identity, ksize, Vector3.zero, Handles.SphereCap);	//CubeCap);
					Handles.color = nocol;	//shape.VecCol;
					Vector3 newp = Handles.FreeMoveHandle(pm, Quaternion.identity, knotsize * ringratio, Vector3.zero, Handles.CircleCap);	//SphereCap);	//CubeCap);
					shape.splines[s].knots[p].p += Vector3.Scale(newp - pm, dragplane);


					//if ( shape.splines[s].knots[p].p != pm )
					//selected = p;
					//}

					delta = shape.splines[s].knots[p].p - pm;

					shape.splines[s].knots[p].invec += delta;
					shape.splines[s].knots[p].outvec += delta;

					if ( shape.splines[s].knots[p].p != pm )
					{
						selected = p;
						recalc = true;
					}

					pm = shape.transform.TransformPoint(shape.splines[s].knots[p].p);

					//Handles.CubeCap(0, pm, Quaternion.identity, shape.KnotSize);
				}

				if ( shape.drawHandles )	//&& recalc == false )
				{
					Handles.color = shape.VecCol;
					pm = shape.transform.TransformPoint(shape.splines[s].knots[p].p);

					Vector3 ip = shape.transform.TransformPoint(shape.splines[s].knots[p].invec);
					Vector3 op = shape.transform.TransformPoint(shape.splines[s].knots[p].outvec);
					Handles.DrawLine(pm, shape.transform.TransformPoint(shape.splines[s].knots[p].invec));
					Handles.DrawLine(pm, shape.transform.TransformPoint(shape.splines[s].knots[p].outvec));


					//Handles.color = Color.black;
					//Vector3 normal = (op - Camera.current.transform.position).normalized;
					Handles.DrawSolidDisc(op, normal, handlesize * borderratio);	//ksize * 0.55f * ringratio);
					//normal = Camera.current.transform.forward;	//(ip - Camera.current.transform.position).normalized;
					Handles.DrawSolidDisc(ip, normal, handlesize * borderratio);	//ksize * 0.55f * ringratio);

					Handles.color = shape.HandleCol;

					//Handles.DrawSolidDisc(op, normal, ksize * 0.5f * ringratio);
					//Handles.DrawSolidDisc(ip, normal, ksize * 0.5f * ringratio);
					Handles.DrawSolidDisc(op, normal, handlesize);
					Handles.DrawSolidDisc(ip, normal, handlesize);

					//shape.splines[s].knots[p].invec = Handles.PositionHandle(shape.splines[s].knots[p].invec, Quaternion.identity);	//shape.hsize);
					//shape.splines[s].knots[p].outvec = Handles.PositionHandle(shape.splines[s].knots[p].outvec, Quaternion.identity);	//shape.hsize);

					Vector3 invec = shape.splines[s].knots[p].invec;
					//if ( p == selected )
						//invec = Handles.PositionHandle(shape.splines[s].knots[p].invec, Quaternion.identity);	//shape.hsize);
					//else
					Handles.color = nocol;
					Vector3 newinvec = Handles.FreeMoveHandle(shape.splines[s].knots[p].invec, Quaternion.identity, handlesize * ringratio, Vector3.zero, Handles.CircleCap);	//SphereCap);	//CubeCap);

					invec += Vector3.Scale(newinvec - invec, dragplane);
					//Debug.Log("sel " + selected + " new " + invec.ToString("0.0000") + " old " + shape.splines[s].knots[p].invec.ToString("0.0000"));
					if ( invec != shape.splines[s].knots[p].invec )
					{
						if ( shape.lockhandles )
						{
							Vector3 d = invec - shape.splines[s].knots[p].invec;
							shape.splines[s].knots[p].outvec -= d;
						}

						shape.splines[s].knots[p].invec = invec;
						selected = p;
						recalc = true;
					}
					Vector3 outvec = shape.splines[s].knots[p].outvec;	// = Handles.PositionHandle(shape.splines[s].knots[p].outvec, Quaternion.identity);	//shape.hsize);

					//if ( p == selected )
						//outvec = Handles.PositionHandle(shape.splines[s].knots[p].outvec, Quaternion.identity);	//shape.hsize);
					//else
					Vector3 newoutvec = Handles.FreeMoveHandle(shape.splines[s].knots[p].outvec, Quaternion.identity, handlesize * ringratio, Vector3.zero, Handles.CircleCap);	//SphereCap);	//CubeCap);
					outvec += Vector3.Scale(newoutvec - outvec, dragplane);

					if ( outvec != shape.splines[s].knots[p].outvec )
					{
						if ( shape.lockhandles )
						{
							Vector3 d = outvec - shape.splines[s].knots[p].outvec;
							shape.splines[s].knots[p].invec -= d;
						}

						shape.splines[s].knots[p].outvec = outvec;
						selected = p;
						recalc = true;
					}
					Vector3 hp = shape.transform.TransformPoint(shape.splines[s].knots[p].invec);
					//Handles.CubeCap(0, hp, Quaternion.identity, shape.KnotSize);
					if ( selected == p )
						Handles.Label(hp, " " + p);

					hp = shape.transform.TransformPoint(shape.splines[s].knots[p].outvec);
					//Handles.CubeCap(0, hp, Quaternion.identity, shape.KnotSize);
					
					if ( selected == p )
						Handles.Label(hp, " " + p);
				}

				// Draw nearest point (use for adding knot)
				//CursorPos = Handles.PositionHandle(CursorPos, Quaternion.identity);
#if false
				if ( shape.drawKnots )
				{
					pm = shape.splines[s].knots[p].p;

					if ( p == selected )
					{
						Handles.color = Color.white;
						Handles.Label(pm, " Selected\n" + pm.ToString("0.000"));
					}
					else
					{
						Handles.color = shape.KnotCol;
						Handles.Label(pm, " " + p);
					}

					//if ( p == selected )
						shape.splines[s].knots[p].p = Handles.PositionHandle(pm, Quaternion.identity);
					//else
					//{
						//shape.splines[s].knots[p].p = Handles.FreeMoveHandle(pm, Quaternion.identity, shape.KnotSize, Vector3.zero, Handles.CubeCap);

						//if ( shape.splines[s].knots[p].p != pm )
						//selected = p;
					//}

					delta = shape.splines[s].knots[p].p - pm;

					shape.splines[s].knots[p].invec += delta;
					shape.splines[s].knots[p].outvec += delta;

					if ( shape.splines[s].knots[p].p != pm )
					{
						selected = p;
						recalc = true;
					}

					pm = shape.transform.TransformPoint(shape.splines[s].knots[p].p);

					Handles.CubeCap(0, pm, Quaternion.identity, shape.KnotSize);
				}
#endif
			}
		}

		if ( recalc )
		{
			shape.CalcLength(10);
			shape.BuildMesh();
		}

		Handles.matrix = Matrix4x4.identity;
	}

	[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
	static void RenderGizmo(MegaShape shape, GizmoType gizmoType)
	{
		if ( (gizmoType & GizmoType.NotInSelectionHierarchy) != 0 )
		{
			if ( (gizmoType & GizmoType.Active) != 0 )
			{
				DrawGizmos(shape, new Color(1.0f, 1.0f, 1.0f, 1.0f));
				Color col = Color.yellow;
				col.a = 0.5f;
				Gizmos.color = col;	//Color.yellow;
				//CursorPos = shape.FindNearestPoint(CursorPos, 5, ref CursorKnot, ref CursorTangent);
				//CursorSpline = shape.FindNearestPoint(CursorPos, 5, ref CursorKnot, ref CursorTangent);
				//Gizmos.DrawSphere(shape.transform.TransformPoint(CursorPos), shape.KnotSize);
				CursorPos = shape.InterpCurve3D(0, CursorPercent * 0.01f, true);
				Gizmos.DrawSphere(shape.transform.TransformPoint(CursorPos), shape.KnotSize * 0.01f);
				Handles.color = Color.white;
				Handles.Label(shape.transform.TransformPoint(CursorPos), "Cursor " + CursorPercent.ToString("0.00") + "% - " + CursorPos);
			}
			else
				DrawGizmos(shape, new Color(1.0f, 1.0f, 1.0f, 0.25f));
		}
		Gizmos.DrawIcon(shape.transform.position, "MegaSpherify icon.png");
		Handles.Label(shape.transform.position, " " + shape.name);
	}

	static public int CursorKnot = 0;
	static public float CursorPercent = 0.0f;

	// Dont want this in here, want in editor
	// If we go over a knot then should draw to the knot
	static void DrawGizmos(MegaShape shape, Color modcol)
	{
		if ( ((1 << shape.gameObject.layer) & Camera.current.cullingMask) == 0 )
			return;

		if ( !shape.drawspline )
			return;

		for ( int s = 0; s < shape.splines.Count; s++ )
		{
			float ldist = shape.stepdist * 0.1f;
			if ( ldist < 0.01f )
				ldist = 0.01f;

			float ds = shape.splines[s].length / (shape.splines[s].length / ldist);

			if ( ds > shape.splines[s].length )
			{
				ds = shape.splines[s].length;
			}

			int c	= 0;
			int k	= -1;
			int lk	= -1;

			Vector3 first = shape.splines[s].Interpolate(0.0f, shape.normalizedInterp, ref lk);

			for ( float dist = ds; dist < shape.splines[s].length; dist += ds )
			{
				float alpha = dist / shape.splines[s].length;
				Vector3 pos = shape.splines[s].Interpolate(alpha, shape.normalizedInterp, ref k);

				if ( (c & 1) == 1 )
					Gizmos.color = shape.col1 * modcol;
				else
					Gizmos.color = shape.col2 * modcol;

				if ( k != lk )
				{
					for ( lk = lk + 1; lk <= k; lk++ )
					{
						Gizmos.DrawLine(shape.transform.TransformPoint(first), shape.transform.TransformPoint(shape.splines[s].knots[lk].p));
						first = shape.splines[s].knots[lk].p;
					}
				}

				lk = k;

				Gizmos.DrawLine(shape.transform.TransformPoint(first), shape.transform.TransformPoint(pos));

				c++;

				first = pos;
			}

			if ( (c & 1) == 1 )
				Gizmos.color = shape.col1 * modcol;
			else
				Gizmos.color = shape.col2 * modcol;

			Vector3 lastpos;
			if ( shape.splines[s].closed )
				lastpos = shape.splines[s].Interpolate(0.0f, shape.normalizedInterp, ref k);
			else
				lastpos = shape.splines[s].Interpolate(1.0f, shape.normalizedInterp, ref k);

			Gizmos.DrawLine(shape.transform.TransformPoint(first), shape.transform.TransformPoint(lastpos));
		}

		//Gizmos.color = Color.yellow;
		//CursorPos = shape.FindNearestPoint(CursorPos, 5, ref CursorKnot, ref CursorTangent);
		//Gizmos.DrawWireSphere(shape.transform.TransformPoint(CursorPos), shape.KnotSize);
	}

	// Load stuff
	string lastpath = "";

	public delegate bool ParseBinCallbackType(BinaryReader br, string id);
	public delegate void ParseClassCallbackType(string classname, BinaryReader br);

	void LoadShape(float scale)
	{
		MegaShape ms = (MegaShape)target;
		//Modifiers mod = mr.GetComponent<Modifiers>();	// Do this at start and store

		string filename = EditorUtility.OpenFilePanel("Shape File", lastpath, "spl");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		// Clear what we have
		ms.splines.Clear();

		ParseFile(filename, ShapeCallback);

		ms.Scale(scale);

		ms.MaxTime = 0.0f;

		for ( int s = 0; s < ms.splines.Count; s++ )
		{
			if ( ms.splines[s].animations != null )
			{
				for ( int a = 0; a < ms.splines[s].animations.Count; a++ )
				{
					MegaControl con = ms.splines[s].animations[a].con;
					if ( con != null )
					{
						float t = con.Times[con.Times.Length - 1];
						if ( t > ms.MaxTime )
							ms.MaxTime = t;
					}
				}
			}
		}
	}

	public void ShapeCallback(string classname, BinaryReader br)
	{
		switch ( classname )
		{
			case "Shape": LoadShape(br); break;
		}
	}

	public void LoadShape(BinaryReader br)
	{
		//MegaMorphEditor.Parse(br, ParseShape);
		MegaParse.Parse(br, ParseShape);
	}

	public void ParseFile(string assetpath, ParseClassCallbackType cb)
	{
		FileStream fs = new FileStream(assetpath, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read);

		BinaryReader br = new BinaryReader(fs);

		bool processing = true;

		while ( processing )
		{
			string classname = MegaParse.ReadString(br);

			if ( classname == "Done" )
				break;

			int	chunkoff = br.ReadInt32();
			long fpos = fs.Position;

			cb(classname, br);

			fs.Position = fpos + chunkoff;
		}

		br.Close();
	}

	static public Vector3 ReadP3(BinaryReader br)
	{
		Vector3 p = Vector3.zero;

		p.x = br.ReadSingle();
		p.y = br.ReadSingle();
		p.z = br.ReadSingle();

		return p;
	}

	bool SplineParse(BinaryReader br, string cid)
	{
		MegaShape ms = (MegaShape)target;
		MegaSpline ps = ms.splines[ms.splines.Count - 1];

		switch ( cid )
		{
			case "Transform":
				Vector3 pos = ReadP3(br);
				Vector3 rot = ReadP3(br);
				Vector3 scl = ReadP3(br);
				rot.y = -rot.y;
				ms.transform.position = pos;
				ms.transform.rotation = Quaternion.Euler(rot * Mathf.Rad2Deg);
				ms.transform.localScale = scl;
				break;

			case "Flags":
				int count = br.ReadInt32();
				ps.closed = (br.ReadInt32() == 1);
				count = br.ReadInt32();
				ps.knots = new List<MegaKnot>(count);
				ps.length = 0.0f;
				break;

			case "Knots":
				for ( int i = 0; i < ps.knots.Capacity; i++ )
				{
					MegaKnot pk = new MegaKnot();

					pk.p = ReadP3(br);
					pk.invec = ReadP3(br);
					pk.outvec = ReadP3(br);
					pk.seglength = br.ReadSingle();

					ps.length += pk.seglength;
					pk.length = ps.length;
					ps.knots.Add(pk);
				}
				break;
		}
		return true;
	}

	MegaKnotAnim ma;

	bool AnimParse(BinaryReader br, string cid)
	{
		MegaShape ms = (MegaShape)target;

		switch ( cid )
		{
			case "V":
				int v = br.ReadInt32();
				ma = new MegaKnotAnim();
				int s = ms.GetSpline(v, ref ma);	//.s, ref ma.p, ref ma.t);

				if ( ms.splines[s].animations == null )
					ms.splines[s].animations = new List<MegaKnotAnim>();

				ms.splines[s].animations.Add(ma);
				break;

			case "Anim":
				//ma.con = MegaBezVector3KeyControl.LoadBezVector3KeyControl(br);
				ma.con = MegaParseBezVector3Control.LoadBezVector3KeyControl(br);
				break;
		}
		return true;
	}

	bool ParseShape(BinaryReader br, string cid)
	{
		MegaShape ms = (MegaShape)target;

		switch ( cid )
		{
			case "Num":
				int count = br.ReadInt32();
				ms.splines = new List<MegaSpline>(count);
				//id = 0;
				break;

			case "Spline":
				MegaSpline spl = new MegaSpline();
				ms.splines.Add(spl);
				//MegaMorphEditor.Parse(br, SplineParse);
				MegaParse.Parse(br, SplineParse);
				break;

			case "Anim":
				//Debug.Log("Anim info");
				//MegaMorphEditor.Parse(br, AnimParse);
				MegaParse.Parse(br, AnimParse);
				break;
		}

		return true;
	}
}
