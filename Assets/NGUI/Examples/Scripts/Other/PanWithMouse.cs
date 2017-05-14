//using UnityEngine;

///// <summary>
///// Placing this script on the game object will make that game object pan with mouse movement.
///// </summary>

//[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
//public class PanWithMouse : MonoBehaviour
//{
//	public Vector2 degrees = new Vector2(5f, 3f);
//	public float range = 1f;

//	Transform mTrans;
//	Quaternion mStart;
//	Vector2 mRot = Vector2.zero;

//	void Start ()
//	{
//		mTrans = transform;
//		mStart = mTrans.localRotation;
//	}
//    Vector3 last_pos = Vector3.zero;
//    Vector3 realpos = Vector3.zero;
//    Vector3 realrrrr = Vector3.zero;
//    void Update ()
//	{
//        float halfWidth = Screen.width * 0.5f;
//        float halfHeight = Screen.height * 0.5f;
//        float delta = RealTime.deltaTime;
//        Vector3 pos = UICamera.lastEventPosition;
//        if (Program.InputGetMouseButtonDown_0)  
//        {
//            last_pos = pos;
//        }
//        if (last_pos != pos && Program.InputGetMouseButton_0)
//        {
//            realpos = (pos - last_pos) / delta;
//        }
//        else
//        {
//            realpos = Vector3.zero;
//        }
//        realrrrr += (realpos - realrrrr) * delta * 1000f;
//        if (range < 0.1f) range = 0.1f;
//        float x = Mathf.Clamp((realrrrr.x) / halfWidth / (range * 10f), -1f, 1f);
//        float y = Mathf.Clamp((realrrrr.y) / halfHeight / (range * 10f), -1f, 1f);
//        mRot = Vector2.Lerp(mRot, new Vector2(x, y), delta);

//        mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * degrees.y, mRot.x * degrees.x, 0f);
//    }
//}
using UnityEngine;

/// <summary>
/// Placing this script on the game object will make that game object pan with mouse movement.
/// </summary>

[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
public class PanWithMouse : MonoBehaviour
{

    Transform mTrans;
    Quaternion mStart;
    Vector2 mRot = Vector2.zero;

    void Start()
    {
        mTrans = transform;
        mStart = mTrans.localRotation;
    }
    Vector2 lastPos = Vector3.zero;
    void Update()
    {
        float delta = RealTime.deltaTime;
        Vector3 pos;
        float halfWidth = Screen.width * 0.5f;
        float halfHeight = Screen.height * 0.5f;
        if (Program.InputGetMouseButtonDown_0)
        {
            lastPos = UICamera.lastEventPosition;
        }
        if (Program.InputGetMouseButton_0)
        {
            pos = -UICamera.lastEventPosition + lastPos;
        }
        else
        {
            pos = Vector3.zero;
        }
        float x = Mathf.Clamp((pos.x) / halfWidth * 1f, -1f, 1f);
        float y = Mathf.Clamp((pos.y) / halfHeight * 1f, -1f, 1f);
        mRot = Vector2.Lerp(mRot, new Vector2(x, y), delta * 5f);

        mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * 6f, mRot.x * 8f, 0f);
    }
}

