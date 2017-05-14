var speed:Vector3;
var speedRedirect:Vector3;
function Start(){
	speed += new Vector3(Random.Range(-speedRedirect.x,speedRedirect.x),Random.Range(-speedRedirect.y,speedRedirect.y),Random.Range(-speedRedirect.z,speedRedirect.z));
}
function Update () {
	this.GetComponent.<Rigidbody>().velocity += speed* Time.deltaTime;
}