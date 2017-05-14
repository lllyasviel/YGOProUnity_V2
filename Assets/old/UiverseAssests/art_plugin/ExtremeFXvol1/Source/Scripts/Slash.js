var speed:Vector3 = Vector3.one;
function Update () {
	transform.localScale.x += speed.x * Time.deltaTime;
	transform.localScale.y += speed.y * Time.deltaTime;
	if(transform.localScale.y<0){
		transform.localScale.y = 0;
	}
	
}