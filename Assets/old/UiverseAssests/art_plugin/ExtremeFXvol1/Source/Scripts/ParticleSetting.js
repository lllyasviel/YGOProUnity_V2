var LightIntensityMult:float = -0.5f;
var LifeTime:float = 1;
var RandomRotation:boolean = false;
var PositionOffset:Vector3;
var SpawnEnd:GameObject;
private var timetemp;
function Start(){
	timetemp = Time.time;
	if(RandomRotation){
		this.gameObject.transform.rotation.x = Random.rotation.x;
		this.gameObject.transform.rotation.y = Random.rotation.y;
		this.gameObject.transform.rotation.z = Random.rotation.z;
	}
	
}
function Update () {
	if(Time.time > timetemp + LifeTime){
		if(SpawnEnd){
			var obj = GameObject.Instantiate(SpawnEnd,this.transform.position,this.transform.rotation);
		}
		GameObject.Destroy(this.gameObject);
	}
	if(this.gameObject.GetComponent.<Light>()){
		this.GetComponent.<Light>().intensity += LightIntensityMult * Time.deltaTime;
	}
}