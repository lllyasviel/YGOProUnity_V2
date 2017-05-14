public var uvAnimationTileX = 24;
public var uvAnimationTileY = 1;
public var framesPerSecond = 10.0;
public var loop:boolean;
public var play:boolean = true;
private var index:int; 
private var offsettime;
public var Hidewhenstopplaying:boolean;
function Start(){
	offsettime = Time.time;
}
function Update () {
	index = (Time.time - offsettime) * framesPerSecond;
	if(play){
	index = index % (uvAnimationTileX * uvAnimationTileY);
	var size = Vector2 (1.0 / uvAnimationTileX, 1.0 / uvAnimationTileY);
	var uIndex = index % uvAnimationTileX;
	var vIndex = index / uvAnimationTileX;
	var offset = Vector2 (uIndex * size.x, 1.0 - size.y - vIndex * size.y);
	
	GetComponent.<Renderer>().material.SetTextureOffset ("_MainTex", offset);
	GetComponent.<Renderer>().material.SetTextureScale ("_MainTex", size);
	}
	if(!loop){
		if(index >= (uvAnimationTileX * uvAnimationTileY)-1){
			play = false;
			if(Hidewhenstopplaying){
				GetComponent.<Renderer>().active = false;
			}
		}
	}

}