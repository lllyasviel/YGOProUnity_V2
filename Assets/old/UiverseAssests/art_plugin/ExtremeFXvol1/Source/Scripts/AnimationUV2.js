var speed:float = 0.5f;
function Start () {

}

function Update () {
	GetComponent.<Renderer>().material.mainTextureOffset.x+=speed * Time.deltaTime;
	if(GetComponent.<Renderer>().material.mainTextureOffset.x>1){
		GetComponent.<Renderer>().material.mainTextureOffset.x = 0;
	}
	if(GetComponent.<Renderer>().material.mainTextureOffset.x<0){
		GetComponent.<Renderer>().material.mainTextureOffset.x = 1;
	}
}