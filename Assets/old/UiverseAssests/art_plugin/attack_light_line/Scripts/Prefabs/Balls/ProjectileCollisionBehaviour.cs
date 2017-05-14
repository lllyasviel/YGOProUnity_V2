using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;


public enum RandomMoveCoordinates
{
  None,
  XY,
  XZ,
  YZ,
  XYZ
}

public class ProjectileCollisionBehaviour : MonoBehaviour
{
  public float RandomMoveRadius;
  public float RandomMoveSpeed;
  public float RandomRange;
  public RandomMoveCoordinates RandomMoveCoordinates = RandomMoveCoordinates.None;
  public GameObject EffectOnHitObject;
  public GameObject GoLight;
  public bool IsCenterLightPosition;
  public bool IsLookAt;
  public bool AttachAfterCollision;
  public bool IsRootMove = true;
  public bool IsLocalSpaceRandomMove;
  public bool IsDeviation;
  public bool SendCollisionMessage = true;
  public bool ResetParentPositionOnDisable;

  private EffectSettings effectSettings;
  private Transform tRoot, tTarget, t, tLight;
  private Vector3 forwardDirection, startPosition, startParentPosition;
  private RaycastHit hit;
  private Vector3 smootRandomPos, oldSmootRandomPos;
  private float deltaSpeed;
  private float startTime;
  private float randomSpeed, randomRadiusX, randomRadiusY;
  private int randomDirection1, randomDirection2, randomDirection3;
  private bool onCollision;
  private bool isInitializedOnStart;
  private Vector3 randomTargetOffsetXZVector;

  void GetEffectSettingsComponent(Transform tr)
  {
    var parent = tr.parent;
    if (parent!=null) {
      effectSettings = parent.GetComponentInChildren<EffectSettings>();
      if (effectSettings == null)
      GetEffectSettingsComponent(parent.transform);
    }
  }

  // Use this for initialization
  private void Start()
  { 
    t = transform;
    GetEffectSettingsComponent(t);
    if (effectSettings == null)
      Debug.Log("Prefab root or children have not script \"PrefabSettings\"");
    if (!IsRootMove) startParentPosition = transform.parent.position;
    if (GoLight != null) tLight = GoLight.transform;
    InitializeDefault();
    isInitializedOnStart = true;
  }

  void OnEnable()
  {
    if(isInitializedOnStart) InitializeDefault();
  }

  void OnDisable()
  {
    if (ResetParentPositionOnDisable && isInitializedOnStart && !IsRootMove) transform.parent.position = startParentPosition;
  }

  private void InitializeDefault()
  {
    hit = new RaycastHit();
    onCollision = false;
    smootRandomPos = new Vector3();
    oldSmootRandomPos = new Vector3();
    deltaSpeed = 0;
    startTime = 0;
    randomSpeed = 0;
    randomRadiusX = 0;
    randomRadiusY = 0;
    randomDirection1 = 0;
    randomDirection2 = 0;
    randomDirection3 = 0;
    
    tRoot = IsRootMove ? effectSettings.transform : transform.parent;
    startPosition = tRoot.position;

    tTarget = effectSettings.Target.transform;
    if (effectSettings.EffectRadius > 0.001)
    {
      var rand = Random.insideUnitCircle * effectSettings.EffectRadius;
      randomTargetOffsetXZVector = new Vector3(rand.x, 0, rand.y);
    }
    else
      randomTargetOffsetXZVector = Vector3.zero;

    if (IsLookAt)
      tRoot.LookAt(tTarget);
    forwardDirection = tRoot.position + (tTarget.position + randomTargetOffsetXZVector - tRoot.position).normalized * effectSettings.MoveDistance;
    GetTargetHit();
    InitRandomVariables();
  }

  private void Update()
  {
    if (tTarget == null || onCollision)
      return;
    
    var endPoint = effectSettings.IsHomingMove ? tTarget.position : forwardDirection;
    //GetDistance
    var distance = Vector3.Distance(tRoot.position, endPoint);
    var distanceNextFrame = effectSettings.MoveSpeed * Time.deltaTime;
    if (distanceNextFrame > distance)
      distanceNextFrame = distance;
    if (distance <= effectSettings.ColliderRadius)
    {
      hit = new RaycastHit();
      CollisionEnter();
    }

    var direction = (endPoint - tRoot.position).normalized;
    RaycastHit raycastHit;
    if (Physics.Raycast(tRoot.position, direction, out raycastHit, distanceNextFrame + effectSettings.ColliderRadius, effectSettings.LayerMask))
    {
      endPoint = raycastHit.point - direction * effectSettings.ColliderRadius;
      CollisionEnter();
    }
    if (raycastHit.transform != null)
      hit = raycastHit;

    if (IsCenterLightPosition && GoLight!=null)
      tLight.position = (startPosition + tRoot.position)/2;

    //GetRandomDistance
    var delta = new Vector3();
    if (RandomMoveCoordinates != RandomMoveCoordinates.None)
    {
      UpdateSmootRandomhPos();
      delta = smootRandomPos - oldSmootRandomPos;
    }
    var moveDistance = Vector3.MoveTowards(tRoot.position, endPoint, effectSettings.MoveSpeed * Time.deltaTime);
    var moveDistanceRandom = moveDistance + delta;
    if (IsLookAt && effectSettings.IsHomingMove)
      tRoot.LookAt(moveDistanceRandom);
    if (IsLocalSpaceRandomMove)
    {
      if (IsRootMove) {
        tRoot.position = moveDistance;
        t.localPosition += delta;
      }
      else {
        tRoot.position = moveDistanceRandom;
      }
    }
    else if (IsRootMove)
    {
      tRoot.position = moveDistanceRandom;
    }
    oldSmootRandomPos = smootRandomPos; 
  }

  private void CollisionEnter()
  {
    if (EffectOnHitObject!=null && hit.transform!=null) {
      var hitEffect = Instantiate(EffectOnHitObject, tRoot.position, new Quaternion()) as GameObject;
      var hitGo = hit.transform.gameObject;
      var mesh = hitGo.GetComponent<MeshFilter>().sharedMesh;
      hitEffect.GetComponent<MeshFilter>().sharedMesh = mesh;
      hitEffect.transform.parent = hitGo.transform;
      hitEffect.transform.localPosition = Vector3.zero;
      var rand = 1 + Random.Range(0, 2000) / 100000f;
      hitEffect.transform.localScale = new Vector3(rand, rand, rand);
    }

    if (AttachAfterCollision)
      tRoot.parent = hit.transform;

    if(SendCollisionMessage) effectSettings.OnCollisionHandler(new CollisionInfo {Hit = hit});
    onCollision = true;
  }

  private void InitRandomVariables()
  {
    deltaSpeed = RandomMoveSpeed * Random.Range(1, 1000 * RandomRange + 1) / 1000 - 1;
    startTime = Time.time;
    randomRadiusX = Random.Range(RandomMoveRadius / 20, RandomMoveRadius * 100) / 100;
    randomRadiusY = Random.Range(RandomMoveRadius / 20, RandomMoveRadius * 100) / 100;
    randomSpeed = Random.Range(RandomMoveSpeed / 20, RandomMoveSpeed * 100) / 100;
    randomDirection1 = Random.Range(0, 2) > 0 ? 1 : -1;
    randomDirection2 = Random.Range(0, 2) > 0 ? 1 : -1;
    randomDirection3 = Random.Range(0, 2) > 0 ? 1 : -1;
  }

  private void GetTargetHit()
  {
    RaycastHit raycastHit; 
    var ray = new Ray(tRoot.position, Vector3.Normalize(tTarget.position + randomTargetOffsetXZVector - tRoot.position));
    var coll = tTarget.GetComponent<Collider>();
    if (coll != null && tTarget.GetComponent<Collider>().Raycast(ray, out raycastHit, effectSettings.MoveDistance)) {
      hit = raycastHit;
    }
  }

  private void UpdateSmootRandomhPos()
  {
    float coord1, coord2;
    var time = (Time.time - startTime);

    var timeDegree = time * randomSpeed;
    var delta = time * deltaSpeed;
    if (IsDeviation) {
      var deviation = Vector3.Distance(tRoot.position, hit.point) / effectSettings.MoveDistance;

      coord1 = randomDirection2 * Mathf.Sin(timeDegree) * randomRadiusX * deviation;
      coord2 = randomDirection3 * Mathf.Sin(timeDegree + (randomDirection1 * Mathf.PI / 2) * time + Mathf.Sin(delta)) * randomRadiusY * deviation;
    }
    else {
      coord1 = randomDirection2 * Mathf.Sin(timeDegree) * randomRadiusX;
      coord2 = randomDirection3 * Mathf.Sin(timeDegree + (randomDirection1 * Mathf.PI / 2) * time + Mathf.Sin(delta)) * randomRadiusY;
    }

    if (RandomMoveCoordinates==RandomMoveCoordinates.XY)
      smootRandomPos = new Vector3(coord1, coord2, 0);
    if (RandomMoveCoordinates==RandomMoveCoordinates.XZ)
      smootRandomPos = new Vector3(coord1, 0, coord2);
    if (RandomMoveCoordinates==RandomMoveCoordinates.YZ)
      smootRandomPos = new Vector3(0, coord1, coord2);
    if (RandomMoveCoordinates==RandomMoveCoordinates.XYZ)
      smootRandomPos = new Vector3(coord1, coord2, (coord1 + coord2) / 2 * randomDirection1);
  }
}

  