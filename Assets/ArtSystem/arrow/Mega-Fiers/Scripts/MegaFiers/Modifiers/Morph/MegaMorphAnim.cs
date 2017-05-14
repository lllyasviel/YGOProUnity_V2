
using UnityEngine;

[AddComponentMenu("Modifiers/Morph Animate")]
[ExecuteInEditMode]
public class MegaMorphAnim : MonoBehaviour
{
	public string	SrcChannel = "None";
	public float	Percent = 0.0f;
	MegaMorphChan	channel;

	public string	SrcChannel1 = "None";
	public float	Percent1 = 0.0f;
	MegaMorphChan	channel1;

	public string	SrcChannel2 = "None";
	public float	Percent2 = 0.0f;
	MegaMorphChan	channel2;

	public string	SrcChannel3 = "None";
	public float	Percent3 = 0.0f;
	MegaMorphChan	channel3;

	public string	SrcChannel4 = "None";
	public float	Percent4 = 0.0f;
	MegaMorphChan	channel4;

	public string	SrcChannel5 = "None";
	public float	Percent5 = 0.0f;
	MegaMorphChan	channel5;

	public string	SrcChannel6 = "None";
	public float	Percent6 = 0.0f;
	MegaMorphChan	channel6;

	public string	SrcChannel7 = "None";
	public float	Percent7 = 0.0f;
	MegaMorphChan	channel7;

	public string	SrcChannel8 = "None";
	public float	Percent8 = 0.0f;
	MegaMorphChan	channel8;

	public string	SrcChannel9 = "None";
	public float	Percent9 = 0.0f;
	MegaMorphChan	channel9;

	public void SetChannel(MegaMorph mr, int index)
	{
		switch ( index )
		{
			case 0: channel = mr.GetChannel(SrcChannel);	break;
			case 1: channel1 = mr.GetChannel(SrcChannel1); break;
			case 2: channel2 = mr.GetChannel(SrcChannel2); break;
			case 3: channel3 = mr.GetChannel(SrcChannel3); break;
			case 4: channel4 = mr.GetChannel(SrcChannel4); break;
			case 5: channel5 = mr.GetChannel(SrcChannel5); break;
			case 6: channel6 = mr.GetChannel(SrcChannel6); break;
			case 7: channel7 = mr.GetChannel(SrcChannel7); break;
			case 8: channel8 = mr.GetChannel(SrcChannel9); break;
			case 9: channel9 = mr.GetChannel(SrcChannel9); break;
		}
	}

	void Start()
	{
		MegaMorph mr = GetComponent<MegaMorph>();

		if ( mr != null )
		{
			channel = mr.GetChannel(SrcChannel);
			channel1 = mr.GetChannel(SrcChannel1);
			channel2 = mr.GetChannel(SrcChannel2);
			channel3 = mr.GetChannel(SrcChannel3);
			channel4 = mr.GetChannel(SrcChannel4);
			channel5 = mr.GetChannel(SrcChannel5);
			channel6 = mr.GetChannel(SrcChannel6);
			channel7 = mr.GetChannel(SrcChannel7);
			channel8 = mr.GetChannel(SrcChannel9);
			channel9 = mr.GetChannel(SrcChannel9);
		}
	}

	void Update()
	{
		if ( channel != null )	channel.Percent = Percent;
		if ( channel1 != null ) channel1.Percent = Percent1;
		if ( channel2 != null ) channel2.Percent = Percent2;
		if ( channel3 != null ) channel3.Percent = Percent3;
		if ( channel4 != null ) channel4.Percent = Percent4;
		if ( channel5 != null ) channel5.Percent = Percent5;
		if ( channel6 != null ) channel6.Percent = Percent6;
		if ( channel7 != null ) channel7.Percent = Percent7;
		if ( channel8 != null ) channel8.Percent = Percent8;
		if ( channel9 != null ) channel9.Percent = Percent9;
	}
}