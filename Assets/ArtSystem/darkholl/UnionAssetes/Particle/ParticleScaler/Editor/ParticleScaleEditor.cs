using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(ParticleScale))]
public class ParticleScaleEditor : Editor {



	void Awake() {

		foreach(Transform tr in tgt.transform) {
			if(tr.gameObject.GetComponent<ParticleScale>() == null) {
				tr.gameObject.AddComponent<ParticleScale>(); 
			}
		}

	}






	public override void OnInspectorGUI() {

		tgt.scaleStep = EditorGUILayout.FloatField("Scale Step", tgt.scaleStep);


		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		


		if(GUILayout.Button(new GUIContent("Scale Up"),   GUILayout.Width(80))) {
			UpdateScale(tgt.scaleStep);
		}
		
		
		if(GUILayout.Button(new GUIContent("Scale Down"),   GUILayout.Width(80))) {
			ReduceScale(tgt.scaleStep);
		}
		

		
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();

	}


	private void UpdateScale(float mod) {
		tgt.UpdateScale(mod);
		foreach(Transform tr in tgt.transform) {
			if(tr.gameObject.GetComponent<ParticleScale>() == null) {
				tr.gameObject.AddComponent<ParticleScale>(); 
			}

			tr.gameObject.GetComponent<ParticleScale>().UpdateScale(mod);
		}
	}

	private void ReduceScale(float mod) {
		tgt.ReduceScale(mod);
		foreach(Transform tr in tgt.transform) {
			if(tr.gameObject.GetComponent<ParticleScale>() == null) {
				tr.gameObject.AddComponent<ParticleScale>(); 
			}
			
			tr.gameObject.GetComponent<ParticleScale>().ReduceScale(mod);
		}
	}


	public ParticleScale tgt {
		get {
			return target as ParticleScale;
		}
	}



}
