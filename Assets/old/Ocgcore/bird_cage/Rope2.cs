/*=============={KEEP INFO INTACT}==================
===   Name: Rope Script Version 2.1              ===
===   Company: Reverie Interactive               ===
===   ----------------------------------------   ===
===   Written By: Jacob Fletcher                 ===
===   Release: September, 26, 2010               ===
===   ----------------------------------------   ===
===   Copyright: Reverie Interactive             ===
===   License: Free Use if this box is left alone  =
==================================================*/

using UnityEngine;
using System.Collections.Generic;

public class Rope2 : MonoBehaviour
{
    #region Helper Enumerators and Classes

    // Custom Interfaces
    [System.Serializable]
    public class ChainLink
    {
        public GameObject linkPrefab;
        public bool alternateChain = true;
        public LongAxis longestAxis = LongAxis.X;
        public Vector3 linkScale = Vector3.one;
        public Vector3 rotation = Vector3.zero;
        public bool makeAllWeak = false;
        public List<int> weakLinkIndex;
        public float weakLinkBreakingForce = Mathf.Infinity;
    }
    [System.Serializable]
    public class AttachPoint
    {
        public GameObject attachedObject;
        public int jointIndex = 0;
    }
    [System.Serializable]
    public class JointPhysicsSettings
    {
        //Collider Variables
        public PhysicMaterial physicsMaterial = null;

        //RigidBody Variables
        public bool JointsUseGravity = true;
        public RigidbodyInterpolation interpolation = RigidbodyInterpolation.None;
        public float JointMass = 0.1f;
        public float JointDrag = 0.25f;
        public float JointAngularDrag = 0.05f;
        public CollisionDetectMode collisionMode = CollisionDetectMode.Discrete;
    }
    [System.Serializable]
    public class JointSettings
    {
        //Joint Variables
        public Vector3 CJoint_axis = new Vector3(1, 1, 1);
        public Vector3 CJoint_swingAxis = new Vector3(1, 1, 1);
        public float CJoint_lowTwist_Limit = 0;
        public float CJoint_lowTwist_Spring = 0;
        public float CJoint_lowTwist_Dampen = 0;
        public float CJoint_lowTwist_Bounce = 0;
        public float CJoint_highTwist_Limit = 0;
        public float CJoint_highTwist_Spring = 0;
        public float CJoint_highTwist_Dampen = 0;
        public float CJoint_highTwist_Bounce = 0;
        public float CJoint_swing1_Limit = 180;
        public float CJoint_swing1_Spring = 0;
        public float CJoint_swing1_Dampen = 0;
        public float CJoint_swing1_Bounce = 0;
        public float CJoint_swing2_Limit = 180;
        public float CJoint_swing2_Spring = 0;
        public float CJoint_swing2_Dampen = 0;
        public float CJoint_swing2_Bounce = 0;
    }
    [System.Serializable]
    public class ProceduralSettings
    {
        public int meshDetail = 3;
        public bool castShadow = true;
        public bool recieveShadow = true;
        public bool updateWhenOffScreen = false;
        public bool useSkinNormals = true;
    }

    // Custom Enumorators
    public enum RopeMeshType
    {
        //Procedural,
        Prefab,
        LineRender,
        NoSkin
    }
    public enum CollisionDetectMode
    {
        Discrete,
        Continuous,
        ContinuousDynamic
    }
    public enum Attach
    {
        ToEnd,
        ToStart,
        ToJointIndex
    }
    public enum LongAxis
    {
        X,
        Y,
        Z
    }
    public enum RopePresets
    {
        Cable,
        String,
        Rope,
        Custom
    }
    public enum ColliderType
    {
        Capsule,
        Sphere,
        Cube,
        None
    }
    public enum RopeEnd
    {
        PointA,
        PointB
    }
    #endregion

    #region Public Variables
    // General Rope Properties
    public GameObject PointA;
    public GameObject PointB;
    public bool isPointARestrained = true;
    public bool isPointBRestrained = true;
    public int ropeDetail = 5;
    public float ropeWidth = 0.5f;
    public Material material;
    public ColliderType colliderType = ColliderType.Capsule;
    public Vector3 boxColliderSize = Vector3.one;
    public Vector3 altColliderSize = Vector3.one;
    public RopeMeshType ropeMeshType = RopeMeshType.LineRender;
    public List<AttachPoint> attachedObjects = new List<AttachPoint>();
    public bool ropeControlEnabled = false;
    public KeyCode ropeControlPull = KeyCode.Alpha1;
    public KeyCode ropeControlPush = KeyCode.Alpha2;
    public bool windRopeAtStart = true;
    public float ropeRaiseLowerSpeed = 2.5f;
    public float collectionRadius = 0.25f;

    // Chain Link Prefab Settings
    public ChainLink chainLinkSettings;
    //private List<GameObject> demoSet = new List<GameObject>();

    // Procedural Mesh Settings
    public ProceduralSettings proceduralMeshSettings;
    public JointPhysicsSettings jointPhysicsSettings;
    public JointSettings jointSettings;
    #endregion

    #region Private Variables
    Vector3[] crossPoints;
    Transform paStart;
    Transform pbStart;
    Mesh mesh;

    GameObject ropeObject;
    List<GameObject> joints = new List<GameObject>();
    int jointCount = 0;
    float jointSeperation = 0;
    LineRenderer line;
    int prevDetail = 0;
    int movingLink = 0;
    float distanceMoved = 0;
    Vector3 heading;
    float CJoint_breakForce = Mathf.Infinity;
    float CJoint_breakTorque = Mathf.Infinity;
    #endregion

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (PointB)
            {
                // If user changes a setting, update the preview
                if (prevDetail != ropeDetail)
                {
                    heading = (PointB.transform.position - PointA.transform.position) / ropeDetail;
                }

                #region Show rope gizmo preview

                if (isPointARestrained)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawWireSphere(PointA.transform.position, ropeWidth);
                Gizmos.color = Color.black;
                if (showJointGizmos)
                {
                    for (int i = 1; i < ropeDetail; i++)
                    {
                        Gizmos.DrawWireSphere(PointA.transform.position + (heading * i), ropeWidth);
                    }
                }
                if (isPointBRestrained)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(PointB.transform.position, ropeWidth);
                #endregion
            }
        }
        else
        {
            DestroyRopePreview();
            if (PointA && PointB && ropeMeshType == RopeMeshType.NoSkin)
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    Gizmos.color = Color.grey;
                    Gizmos.DrawWireSphere(joints[i].transform.position, ropeWidth);

                    if (i != 0)
                        Gizmos.DrawLine(joints[i].transform.position, joints[i - 1].transform.position);
                }
            }

            //foreach (GameObject dds in demoSet)
                //Destroy(dds);

            //demoSet.Clear();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && allowPreview && (PointA && PointB))
            CreateTempRopeJoints();
    }

    void Awake()
    {
        DestroyRopePreview();
        if (gameObject.GetComponent<LineRenderer>())
            DestroyImmediate(gameObject.GetComponent<LineRenderer>());
        tLine = null;

        if (PointA && PointB)
        {
            paStart = PointA.transform;
            pbStart = PointB.transform;

            BuildRope();
        }

        if (windRopeAtStart && ropeControlEnabled)
        {
            WindUpRope(ropeDetail);
        }
    }

    void FixedUpdate()
    {
        if (ropeMeshType == RopeMeshType.LineRender)
        {
            for (int i = 0; i < joints.Count; i++)
            {
                line.SetPosition(i, joints[i].transform.position);
            }
        }

        if(ropeControlEnabled)
            GetRopeActivity();
    }

    void GetRopeActivity()
    {
        if (ropeControlPull != KeyCode.None)
        {
            if(Input.GetKey(ropeControlPull))
                PullRope(ropeRaiseLowerSpeed);
        }
        if (ropeControlPush != KeyCode.None)
        {
            if(Input.GetKey(ropeControlPush))
                PushRope(ropeRaiseLowerSpeed);
        }
    }

    void Destroy()
    {
        Destroy(ropeObject);
        joints.Clear();
    }

    void BuildRope()
    {
        if (!PointA.GetComponent<Rigidbody>())
            AddRigidbody(PointA, 0);

        if (!PointB.GetComponent<Rigidbody>())
            AddRigidbody(PointB, 2);

        if (ropeMeshType == RopeMeshType.Prefab && chainLinkSettings.linkPrefab == null)
        {
           // Debug.LogWarning("You need to assign a prefab in the \"Chain Link Settings\" dropdown: Changing to LineRenderer mesh style...");
            ropeMeshType = RopeMeshType.LineRender;
        }

        jointCount = ropeDetail + 1;
        if (ropeMeshType == RopeMeshType.LineRender)
        {
            line = PointA.AddComponent<LineRenderer>();
            line.SetVertexCount(jointCount);
            line.SetWidth(ropeWidth * 2, ropeWidth * 2);
            line.material = material;
            line.castShadows = false;
            line.receiveShadows = false;
        }
        jointSeperation = Vector3.Distance(PointB.transform.position, PointA.transform.position) / ropeDetail;
        heading = (PointB.transform.position - PointA.transform.position) / ropeDetail;

        ropeObject = new GameObject(transform.name + "s_Rope");
        ropeObject.transform.position = PointA.transform.position;

        for (int j = 0; j < jointCount; j++)
        {
            joints.Add(new GameObject("Joint_" + j));
            joints[j].transform.position = PointA.transform.position + (heading * j);
            joints[j].transform.parent = ropeObject.transform;

            AddRigidbody(joints[j], 1);

            if (j < jointCount - 1)
            {
                if (ropeMeshType == RopeMeshType.Prefab)
                    AddLink(joints[j], j);

                AddCollider(joints[j],j);
            }

            joints[j].transform.LookAt(PointB.transform);
        }



        //Add Character Joints
        for (int i = 0; i < jointCount; i++)
        {
            CharacterJoint newJoint = AddJoint(joints[i], i);

            if (i > 0)
                newJoint.connectedBody = joints[i - 1].GetComponent<Rigidbody>();
            else
                newJoint.connectedBody = PointA.GetComponent<Rigidbody>();

            if (i == jointCount - 1)
                AddJoint(PointB, -1).connectedBody = joints[i].GetComponent<Rigidbody>();
        }
        
        AttachObjects();
    }
    void AttachObjects()
    {
        foreach (AttachPoint ap in attachedObjects)
        {
            if (ap.attachedObject != null)
            {
                if (!ap.attachedObject.GetComponent<Rigidbody>())
                    AddRigidbody(ap.attachedObject, 1);
                
                ap.attachedObject.AddComponent<FixedJoint>().connectedBody = joints[ap.jointIndex].GetComponent<Rigidbody>();
            }
        }
    }
    void AddCollider(GameObject joint, int indRef)
    {
        switch (colliderType)
        {
            case ColliderType.Capsule:
                CapsuleCollider cCollide = joint.AddComponent<CapsuleCollider>();
                cCollide.radius = ropeWidth;
                cCollide.height = jointSeperation*1.25f;
                cCollide.direction = 2;
                cCollide.center = new Vector3(0, 0, (jointSeperation / 2));
                break;
            case ColliderType.Cube:
                BoxCollider bCollide = joint.AddComponent<BoxCollider>();
                if (indRef % 2 == 1)
                {
                    bCollide.size = boxColliderSize;
                }
                else
                {
                    bCollide.size = altColliderSize;
                }
                bCollide.center = new Vector3(0, 0, (jointSeperation / 2));
                break;
            case ColliderType.Sphere:
                SphereCollider sCollider = joint.AddComponent<SphereCollider>();
                sCollider.radius = ropeWidth;
                break;
            default:
                break;
        }
    }
    CharacterJoint AddJoint(GameObject joint, int jointIndex)
    {
        CharacterJoint cJoint = joint.AddComponent<CharacterJoint>();
        cJoint.axis = jointSettings.CJoint_axis;
        cJoint.swingAxis = jointSettings.CJoint_swingAxis;
#if UNITY_2_6
        SoftJointLimit sj = new SoftJointLimit();
        sj.limit = jointSettings.CJoint_lowTwist_Limit;
        sj.bouncyness = jointSettings.CJoint_lowTwist_Bounce;
        sj.spring = jointSettings.CJoint_lowTwist_Spring;
        sj.damper = jointSettings.CJoint_lowTwist_Dampen;
        cJoint.lowTwistLimit = sj;
        sj.limit = jointSettings.CJoint_highTwist_Limit;
        sj.bouncyness = jointSettings.CJoint_highTwist_Bounce;
        sj.spring = jointSettings.CJoint_highTwist_Spring;
        sj.damper = jointSettings.CJoint_highTwist_Dampen;
        cJoint.highTwistLimit = sj;
        sj.limit = jointSettings.CJoint_swing1_Limit;
        sj.bouncyness = jointSettings.CJoint_swing1_Bounce;
        sj.spring = jointSettings.CJoint_swing1_Spring;
        sj.damper = jointSettings.CJoint_swing1_Dampen;
        cJoint.swing1Limit = sj;
        sj.limit = jointSettings.CJoint_swing2_Limit;
        sj.bouncyness = jointSettings.CJoint_swing2_Bounce;
        sj.spring = jointSettings.CJoint_swing2_Spring;
        sj.damper = jointSettings.CJoint_swing2_Dampen;
        cJoint.swing2Limit = sj;
#endif
#if UNITY_3_0
        SoftJointLimit sj = new SoftJointLimit();
        sj.limit = jointSettings.CJoint_lowTwist_Limit;
        sj.bounciness = jointSettings.CJoint_lowTwist_Bounce;
        sj.spring = jointSettings.CJoint_lowTwist_Spring;
        sj.damper = jointSettings.CJoint_lowTwist_Dampen;
        cJoint.lowTwistLimit = sj;
        sj.limit = jointSettings.CJoint_highTwist_Limit;
        sj.bounciness = jointSettings.CJoint_highTwist_Bounce;
        sj.spring = jointSettings.CJoint_highTwist_Spring;
        sj.damper = jointSettings.CJoint_highTwist_Dampen;
        cJoint.highTwistLimit = sj;
        sj.limit = jointSettings.CJoint_swing1_Limit;
        sj.bounciness = jointSettings.CJoint_swing1_Bounce;
        sj.spring = jointSettings.CJoint_swing1_Spring;
        sj.damper = jointSettings.CJoint_swing1_Dampen;
        cJoint.swing1Limit = sj;
        sj.limit = jointSettings.CJoint_swing2_Limit;
        sj.bounciness = jointSettings.CJoint_swing2_Bounce;
        sj.spring = jointSettings.CJoint_swing2_Spring;
        sj.damper = jointSettings.CJoint_swing2_Dampen;
        cJoint.swing2Limit = sj;
#endif

        cJoint.breakForce = CJoint_breakForce;
        cJoint.breakTorque = CJoint_breakTorque;

        if (jointIndex != -1)
        {
            if (!chainLinkSettings.makeAllWeak)
            {
                if (chainLinkSettings.weakLinkBreakingForce != Mathf.Infinity)
                {
                    for (int i = 0; i < chainLinkSettings.weakLinkIndex.Count; i++)
                    {
                        if (jointIndex == chainLinkSettings.weakLinkIndex[i])
                        {
                            cJoint.breakForce = chainLinkSettings.weakLinkBreakingForce;
                            joint.AddComponent<JointBreaker>().SetParentControl(this);
                        }
                    }
                }
            }
            else
            {
                if (chainLinkSettings.weakLinkBreakingForce != Mathf.Infinity)
                {
                    cJoint.breakForce = chainLinkSettings.weakLinkBreakingForce;
                    joint.AddComponent<JointBreaker>().SetParentControl(this);
                }
            }
        }

        return cJoint;
    }
    public int GetJointCount()
    {
        return ropeDetail;
    }
    public GameObject[] GetJoints()
    {
        return joints.ToArray();
    }
    void AddRigidbody(GameObject joint, int partId)
    {
        Rigidbody rigid = joint.AddComponent<Rigidbody>();

        switch (partId)
        {
            case 0:
                rigid.isKinematic = isPointARestrained;
                break;
            case 1:
                break;
            case 2:
                rigid.isKinematic = isPointBRestrained;
                break;
        }

        rigid.mass = jointPhysicsSettings.JointMass;
        rigid.centerOfMass += new Vector3(0, 0, 0.1f);
        rigid.angularDrag = jointPhysicsSettings.JointAngularDrag;
        rigid.drag = jointPhysicsSettings.JointDrag;
#if UNITY_3_0
        rigid.collisionDetectionMode = (CollisionDetectionMode)jointPhysicsSettings.collisionMode;
#endif
        rigid.useGravity = jointPhysicsSettings.JointsUseGravity;
        rigid.interpolation = jointPhysicsSettings.interpolation;
    }
    void AddLink(GameObject joint, int linkRef)
    {
        GameObject link = (GameObject)Instantiate((Object)chainLinkSettings.linkPrefab, joint.transform.position + new Vector3(0, 0, jointSeperation / 2), Quaternion.identity);
        
        switch (chainLinkSettings.longestAxis)
        {
            case LongAxis.X:
                link.transform.localScale = new Vector3(chainLinkSettings.linkScale.x, chainLinkSettings.linkScale.y, chainLinkSettings.linkScale.z);
                link.transform.rotation = Quaternion.Euler(link.transform.rotation.eulerAngles.x + 90, link.transform.rotation.eulerAngles.y, link.transform.rotation.eulerAngles.z);
                break;
            case LongAxis.Y:
                link.transform.localScale = new Vector3(chainLinkSettings.linkScale.x, chainLinkSettings.linkScale.y, chainLinkSettings.linkScale.z);
                link.transform.rotation = Quaternion.Euler(link.transform.rotation.eulerAngles.x, link.transform.rotation.eulerAngles.y + 90, link.transform.rotation.eulerAngles.z);
                break;
            case LongAxis.Z:
                link.transform.localScale = new Vector3(chainLinkSettings.linkScale.x, chainLinkSettings.linkScale.y, chainLinkSettings.linkScale.z);
                link.transform.rotation = Quaternion.Euler(link.transform.rotation.eulerAngles.x, link.transform.rotation.eulerAngles.y, link.transform.rotation.eulerAngles.z + 90);
                break;
        }

        if (chainLinkSettings.alternateChain)
        {
            if (linkRef % 2 == 0)
            {
                link.transform.rotation = Quaternion.Euler(link.transform.rotation.eulerAngles.x + chainLinkSettings.rotation.x, link.transform.rotation.eulerAngles.y + chainLinkSettings.rotation.y, link.transform.rotation.eulerAngles.z + chainLinkSettings.rotation.z);
            }
        }

        link.transform.parent = joint.transform;
    }

    public void RebuildRope()
    {
        PointA.transform.position = paStart.position;
        PointB.transform.position = pbStart.position;
        PointA.transform.rotation = paStart.rotation;
        PointB.transform.rotation = pbStart.rotation;
        Destroy();
        BuildRope();
    }

    public void PullRope(float speed)
    {
        if(movingLink == 0)
            joints[movingLink].transform.position = PointA.transform.position;

        if (movingLink < joints.Count-1)
        {
            joints[movingLink].GetComponent<Rigidbody>().isKinematic = true;
            joints[movingLink].transform.Translate(-(joints[movingLink].transform.position - PointA.transform.position).normalized * speed * Time.deltaTime, PointA.transform);
            distanceMoved = Vector3.Distance(PointA.transform.position,joints[movingLink].transform.position);
            if (distanceMoved <= collectionRadius)
            {
                if(joints[movingLink].GetComponent<Collider>())
                    joints[movingLink].GetComponent<Collider>().isTrigger = true;
                if(ropeMeshType == RopeMeshType.Prefab)
                    joints[movingLink].GetComponentInChildren<MeshRenderer>().enabled = false;
                joints[movingLink].transform.parent = PointA.transform;
                movingLink++;
                joints[movingLink].transform.parent = PointA.transform;
            }
        }
    }
    public void PushRope(float speed)
    {
        if (movingLink > 0)
        {
            joints[movingLink].GetComponent<Rigidbody>().isKinematic = true;
            joints[movingLink].transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0), PointA.transform);
            distanceMoved = Vector3.Distance(PointA.transform.position, joints[movingLink].transform.position);
            if (distanceMoved >= jointSeperation)
            {
                if (joints[movingLink].GetComponent<Collider>())
                    joints[movingLink].GetComponent<Collider>().isTrigger = false;
                joints[movingLink].GetComponent<Rigidbody>().isKinematic = false;
                joints[movingLink].transform.parent = ropeObject.transform;
                movingLink--;
                if (ropeMeshType == RopeMeshType.Prefab)
                    joints[movingLink].GetComponentInChildren<MeshRenderer>().enabled = true;
            }
        }

        if (movingLink == 0)
        {
            //joints[movingLink].transform.parent = ropeObject.transform;
        }
    }
    public void WindUpRope(int linksToWind)
    {
        linksToWind = Mathf.Clamp(linksToWind, 0, joints.Count - 1);
        for (int i = 0; i < linksToWind; i++)
        {
            joints[i].transform.position = PointA.transform.position;
            joints[i].GetComponent<Rigidbody>().isKinematic = true;
            if (ropeMeshType == RopeMeshType.Prefab)
                joints[i].GetComponentInChildren<MeshRenderer>().enabled = false;
            if (joints[i].GetComponent<Collider>())
                joints[i].GetComponent<Collider>().isTrigger = true;
            joints[i].transform.parent = PointA.transform;

            movingLink = linksToWind;
        }
    }

    #region Temporary Rope Display
    public bool allowPreview = true;
    public bool showJointGizmos = true;
    public bool showColliders = false;
    public List<GameObject> tempRopeJoints = new List<GameObject>();
    private LineRenderer tLine = null;
    private GameObject tempLinks;
    public void CreateTempRopeJoints()
    {
        tempLinks = new GameObject("TempLinks");
        tempLinks.transform.position = PointA.transform.position;

        foreach (GameObject go in tempRopeJoints)
        {
            DestroyImmediate(go);
        }
        tempRopeJoints.Clear();

        Vector3 heading = (PointB.transform.position - PointA.transform.position) / ropeDetail;
        for (int i = 1; i < ropeDetail+1; i++)
        {
            //Create joint foundation
            GameObject j = null;

            #region Create Rope Joints Based On Mesh Type
            switch (ropeMeshType)
            {
                case RopeMeshType.Prefab:
                    if (chainLinkSettings.linkPrefab)
                    {
                        float dist = Vector3.Distance(PointB.transform.position, PointA.transform.position) / ropeDetail;

                        j = (GameObject)Instantiate((Object)chainLinkSettings.linkPrefab, new Vector3(0, 0, dist * i - (dist / 2)) + PointA.transform.position, Quaternion.identity);

                        switch (chainLinkSettings.longestAxis)
                        {
                            case LongAxis.X:
                                j.transform.rotation = Quaternion.Euler(j.transform.rotation.eulerAngles.x + 90, j.transform.rotation.eulerAngles.y, j.transform.rotation.eulerAngles.z);
                                break;
                            case LongAxis.Y:
                                j.transform.rotation = Quaternion.Euler(j.transform.rotation.eulerAngles.x, j.transform.rotation.eulerAngles.y + 90, j.transform.rotation.eulerAngles.z);
                                break;
                            case LongAxis.Z:
                                j.transform.rotation = Quaternion.Euler(j.transform.rotation.eulerAngles.x, j.transform.rotation.eulerAngles.y, j.transform.rotation.eulerAngles.z + 90);
                                break;
                        }

                        if (chainLinkSettings.alternateChain)
                        {
                            if (i % 2 == 1)
                            {
                                j.transform.rotation = Quaternion.Euler(j.transform.rotation.eulerAngles.x + chainLinkSettings.rotation.x, j.transform.rotation.eulerAngles.y + chainLinkSettings.rotation.y, j.transform.rotation.eulerAngles.z + chainLinkSettings.rotation.z);
                            }
                        }

                        //Set name, scale and parent the object
                        j.name = "j_" + i.ToString();
                        j.transform.localScale = new Vector3(chainLinkSettings.linkScale.x, chainLinkSettings.linkScale.y, chainLinkSettings.linkScale.z);
                        j.transform.parent = tempLinks.transform;
                    }
                    else
                    {
                        j = new GameObject("j_" + i.ToString());
                        j.transform.position = heading * i + PointA.transform.position;
                        j.transform.parent = gameObject.transform;
                    }
                    if(gameObject.GetComponent<LineRenderer>())
                        DestroyImmediate(gameObject.GetComponent<LineRenderer>());
                    break;

                case RopeMeshType.LineRender:
                    j = new GameObject("j_" + i.ToString());
                    j.transform.position = heading * i + PointA.transform.position;
                    j.transform.parent = gameObject.transform;

                    if (!tLine)
                    {
                        DestroyImmediate(gameObject.GetComponent<LineRenderer>());
                        tLine = gameObject.AddComponent<LineRenderer>();
                    }

                    tLine.SetPosition(0, PointA.transform.position);
                    tLine.SetPosition(1, PointB.transform.position);
                    tLine.SetWidth(ropeWidth * 2, ropeWidth * 2);
                    tLine.castShadows = false;

                    if (material && (tLine.sharedMaterial != material))
                        tLine.GetComponent<Renderer>().sharedMaterial = material;

                    break;

                default:
                    j = new GameObject("j_" + i.ToString());
                    j.transform.position = heading * i + PointA.transform.position;
                    j.transform.parent = gameObject.transform;
                    break;
            }
            #endregion

            if (showColliders)
            {
                switch (colliderType)
                {
                    case ColliderType.Capsule:
                        GameObject col1 = new GameObject("col");
                        col1.transform.position = j.transform.position;
                        CapsuleCollider cCollide = col1.AddComponent<CapsuleCollider>();
                        cCollide.radius = ropeWidth;
                        cCollide.height = (Vector3.Distance(PointB.transform.position, PointA.transform.position) / ropeDetail) * 1.25f;
                        cCollide.direction = 2;
                        col1.transform.parent = j.transform;
                        break;
                    case ColliderType.Sphere:
                        SphereCollider sc = j.AddComponent<SphereCollider>();
                        sc.radius = ropeWidth / 2;
                        break;
                    case ColliderType.Cube:
                        GameObject col2 = new GameObject("col");
                        col2.transform.position = j.transform.position;
                        BoxCollider bCollide = col2.AddComponent<BoxCollider>();
                        if (i % 2 == 0)
                        {
                            bCollide.size = boxColliderSize;
                        }
                        else
                        {
                            bCollide.size = altColliderSize;
                        }
                        bCollide.center = new Vector3(0, 0, (jointSeperation / 2));
                        col2.transform.parent = j.transform;
                        break;
                }
            }

            tempRopeJoints.Add(j);
        }

        tempLinks.transform.LookAt(PointB.transform);

        foreach (GameObject go in tempRopeJoints)
        {
            go.transform.parent = transform;
        }
        DestroyImmediate(tempLinks);
    }

    public void DestroyRopePreview()
    {
        #region Destroy Temporary Rope
        foreach (GameObject go in tempRopeJoints)
        {
            DestroyImmediate(go);
        }
        tempRopeJoints.Clear();

        #endregion
    }
    #endregion
}