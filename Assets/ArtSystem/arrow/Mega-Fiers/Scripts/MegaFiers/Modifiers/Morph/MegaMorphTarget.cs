
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class MegaMorphTarget	//Base
{
	public string		name = "Empty";
	public float		percent;
	public bool			showparams = true;
	public Vector3[]	points;
	public MOMVert[]	mompoints;

	//public int[]		ids;
	//public float[]		weights;
	//public Vector3[]	delta;

	public MOPoint[]	loadpoints;
}

//[System.Serializable]
//public class MegaMorphTarget : MegaMorphTargetBase
//{
//	public Vector3[]	points;
//}

[System.Serializable]
public class MOPoint
{
	public int		id;
	public Vector3	p;
	public float	w;
}

#if false
[System.Serializable]
public class MegaMOMTarget : MegaMorphTarget
{
	// For mom
	public MOMVert[]	mompoints;

	//public int[]		ids;
	//public float[]		weights;
	//public Vector3[]	delta;

	public MOPoint[]	loadpoints;
}
#endif