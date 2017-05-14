var SpeedMin:Vector3;
var SpeedMax:Vector3;
var velosityRandom:boolean;
var zigzax:boolean;
private var power:Vector3;
private var powerlast:Vector3;
function Update () {
	if(velosityRandom){
	if(this.gameObject.GetComponent.<Rigidbody>()){
		this.gameObject.GetComponent.<Rigidbody>().velocity.x += Random.Range(SpeedMin.x,SpeedMax.y) * Time.deltaTime;
		this.gameObject.GetComponent.<Rigidbody>().velocity.y += Random.Range(SpeedMin.y,SpeedMax.y) * Time.deltaTime;
		this.gameObject.GetComponent.<Rigidbody>().velocity.z += Random.Range(SpeedMin.z,SpeedMax.z) * Time.deltaTime;
	}
	}else{
		if(zigzax){
		power.x = Random.Range(0,SpeedMax.x);
		power.y = Random.Range(0,SpeedMax.y);
		power.z = Random.Range(0,SpeedMax.z);
		
		if(powerlast.x > power.x){
			power.x*= -1;
		}

		if(powerlast.x > power.z){
			power.z*= -1;
		}
		
		this.transform.position.x += power.x * Time.deltaTime;
		this.transform.position.y += power.y * Time.deltaTime;
		this.transform.position.z += power.z * Time.deltaTime;
		powerlast = power;
		}else{
			this.transform.position.x += Random.Range(SpeedMin.x,SpeedMax.y) * Time.deltaTime;
			this.transform.position.y += Random.Range(SpeedMin.y,SpeedMax.y) * Time.deltaTime;
			this.transform.position.z += Random.Range(SpeedMin.z,SpeedMax.z) * Time.deltaTime;
		
		}
	}
}