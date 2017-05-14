
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MegaRopeDeformEditor : Editor
{
	static MegaRopeDeformEditor()
	{
		//EditorApplication.update += Update;
	}

	// Have a per object flag for editor update
	static void Update1()
	{
		GameObject obj = Selection.activeGameObject;

		if ( obj )
		{
			//MegaRopeDeform mr = (MegaRopeDeform)obj.GetComponent<MegaRopeDeform>();

			//if ( mr )
			{
				MegaModifyObject mod = (MegaModifyObject)obj.GetComponent<MegaModifyObject>();
				if ( mod )
				{
					mod.ModifyObject();
				}
			}
		}
	}

	static void Update()
	{
		MegaModifyObject[] mods = (MegaModifyObject[])FindSceneObjectsOfType(typeof(MegaModifyObject));

		for ( int i = 0; i < mods.Length; i++ )
			mods[i].ModifyObject();
	}
}