
using UnityEngine;

public class ToggleMultiCore : MonoBehaviour
{
	bool Enabled = false;	//true;

	void Start()
	{
		//Application.targetFrameRate = 60;
		MegaModifiers.ThreadingOn = Enabled;
	}

	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.T) )
		{
			Enabled = !Enabled;
			MegaModifiers.ThreadingOn = Enabled;
		}
	}

	//void OnGUI()
	//{
	//	float fps = 1.0f / Time.smoothDeltaTime;
	//	GUI.Label(new Rect(0, 0, 100, 32), fps.ToString("0.0"));
	//}
}
