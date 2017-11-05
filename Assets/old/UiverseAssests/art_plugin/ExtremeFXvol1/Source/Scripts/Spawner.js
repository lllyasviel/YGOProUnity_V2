var ObjectSpawn:GameObject;
var SpawnRate:float;
var LifeTimeObject:float = 1;
var LimitObject:int = 3;
private var timetemp:float;
private var objcount:int;
var PositionRandomSize:Vector3;
var PositionOffset:Vector3;
function Start(){
	timetemp = Time.time;
}
function Update () {
	if(Time.time > timetemp + SpawnRate){
		if(ObjectSpawn){
			if(objcount < LimitObject){
				var positionoffset:Vector3 = new Vector3(Random.Range(-PositionRandomSize.x,PositionRandomSize.x),Random.Range(-PositionRandomSize.y,PositionRandomSize.y),Random.Range(-PositionRandomSize.z,PositionRandomSize.z));
				var obj = GameObject.Instantiate(ObjectSpawn,this.transform.position+positionoffset+PositionOffset,this.transform.rotation);
				objcount++;
				Destroy(obj,LifeTimeObject);
			}
		}
		timetemp = Time.time;
	}
}