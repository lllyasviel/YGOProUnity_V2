using UnityEngine;

public class MegaMatrix
{
	static public void Set(ref Matrix4x4 mat, float[] vals)
	{
		if ( vals.Length >= 16 )
		{
			for ( int i = 0; i < 16; i++ )
				mat[i] = vals[i];
		}
	}

	static public void Translate(ref Matrix4x4 mat, Vector3 p)
	{
		Translate(ref mat, p.x, p.y, p.z);
	}

	static public void Scale(ref Matrix4x4 mat, Vector3 s, bool trans)
	{
		Matrix4x4 tm = Matrix4x4.identity;

		tm[0, 0] = s.x;
		tm[1, 1] = s.y;
		tm[2, 2] = s.z;

		mat = tm * mat;

		if ( trans )
		{
			mat[0, 3] *= s.x;
			mat[1, 3] *= s.y;
			mat[2, 3] *= s.z;
		}
	}

	static public Vector3 GetTrans(ref Matrix4x4 mat)
	{
		Vector3 p = new Vector3();

		p.x = mat[0, 3];
		p.y = mat[1, 3];
		p.z = mat[2, 3];

		return p;
	}

	static public void SetTrans(ref Matrix4x4 mat, Vector3 p)
	{
		mat[0, 3] = p.x;
		mat[1, 3] = p.y;
		mat[2, 3] = p.z;
	}

	static public void NoTrans(ref Matrix4x4 mat)
	{
		mat[0, 3] = 0.0f;
		mat[1, 3] = 0.0f;
		mat[2, 3] = 0.0f;
	}

	static public void Translate(ref Matrix4x4 mat, float x, float y, float z)
	{
		Matrix4x4 tm = Matrix4x4.identity;

		tm[0, 3] = x;
		tm[1, 3] = y;
		tm[2, 3] = z;

		mat = tm * mat;
	}

	static public void RotateX(ref Matrix4x4 mat, float ang)
	{
		Matrix4x4 tm = Matrix4x4.identity;

		float c = Mathf.Cos(ang);
		float s = Mathf.Sin(ang);

		tm[1, 1] = c;
		tm[1, 2] = s;
		tm[2, 1] = -s;
		tm[2, 2] = c;

		mat = tm * mat;
	}

	static public void RotateY(ref Matrix4x4 mat, float ang)
	{
		Matrix4x4 tm = Matrix4x4.identity;

		float c = Mathf.Cos(ang);
		float s = Mathf.Sin(ang);

		tm[0, 0] = c;
		tm[0, 2] = -s;
		tm[2, 0] = s;
		tm[2, 2] = c;

		mat = tm * mat;
	}

	static public void RotateZ(ref Matrix4x4 mat, float ang)
	{
		Matrix4x4 tm = Matrix4x4.identity;

		float c = Mathf.Cos(ang);
		float s = Mathf.Sin(ang);

		tm[0, 0] = c;
		tm[0, 1] = s;
		tm[1, 0] = -s;
		tm[1, 1] = c;

		mat = tm * mat;
	}

	static public void Rotate(ref Matrix4x4 mat, Vector3 rot)
	{
		RotateX(ref mat, rot.x);
		RotateY(ref mat, rot.y);
		RotateZ(ref mat, rot.z);
	}

	static public void LookAt(ref Matrix4x4 mat, Vector3 source_pos, Vector3 target_pos)
	{
		Vector3 source_target_unit_vector = target_pos - source_pos;

		Vector3 ydir = Vector3.Normalize(target_pos - source_pos);
		Vector3 zdir = Vector3.Normalize(Vector3.Cross(Vector3.up, ydir));

		mat = Matrix4x4.identity;

		mat.SetColumn(1, Vector3.Normalize(Vector3.Cross(ydir, zdir)));
		mat.SetColumn(2, Vector3.Normalize(source_target_unit_vector));
		mat.SetColumn(0, zdir);
	}

	static public void LookAt(ref Matrix4x4 mat, Vector3 source_pos, Vector3 target_pos, Vector3 up)
	{
		Vector3 source_target_unit_vector = target_pos - source_pos;

		Vector3 ydir = Vector3.Normalize(target_pos - source_pos);
		Vector3 zdir = Vector3.Normalize(Vector3.Cross(up, ydir));

		mat = Matrix4x4.identity;
		mat.SetColumn(1, Vector3.Normalize(Vector3.Cross(ydir, zdir)));
		mat.SetColumn(2, Vector3.Normalize(source_target_unit_vector));
		mat.SetColumn(0, zdir);
	}
}