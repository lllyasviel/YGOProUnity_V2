
var Force:Vector3;
var Objcet:GameObject;
var Num:int;
var Scale:int = 20;
var ScaleMin:int = 10;
var Sounds:AudioClip[];
var LifeTimeObject:float = 2;
var postitionoffset:float = 2;
var random:boolean;
function Start () {
	Physics.IgnoreLayerCollision(2,2);
	if(Sounds.length>0){
		AudioSource.PlayClipAtPoint(Sounds[Random.Range(0,Sounds.length)],transform.position);
	}
	if(Objcet){
	if(random){

		Num = Random.Range(1,Num);
	}
	for(var i=0;i<Num;i++){
		var pos = new Vector3(Random.Range(-postitionoffset,postitionoffset),Random.Range(-postitionoffset,postitionoffset),Random.Range(-postitionoffset,postitionoffset)) / 10f;
		var obj:GameObject = Instantiate(Objcet, this.transform.position + pos, Random.rotation);
		var scale = Random.Range(ScaleMin,Scale);
		Destroy(obj,LifeTimeObject);
		if(Scale>0)
		obj.transform.localScale = new Vector3(scale,scale,scale);
		if(obj.GetComponent.<Rigidbody>() ){
   			obj.GetComponent.<Rigidbody>().AddForce(new Vector3(Random.Range(-Force.x,Force.x),Random.Range(-Force.y,Force.y),Random.Range(-Force.z,Force.z)));
   			
   		}
   		
  		}
  	}
}
