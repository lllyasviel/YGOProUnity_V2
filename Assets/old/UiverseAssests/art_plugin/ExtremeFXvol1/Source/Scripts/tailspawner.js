
var ObjectSpawn : Transform;
var SpawnRate: float = 0.2f;
var AreaSpawnSize :int = 20;
var LifetimeSpaned:int = 3;
private var timetemp:float;

function Start () {
}

function Update () {
   if(Time.time >= timetemp+SpawnRate){
   	  var objectspawned:Transform = Instantiate(ObjectSpawn, transform.position + new Vector3(Random.Range(-AreaSpawnSize,AreaSpawnSize),0,Random.Range(-AreaSpawnSize,AreaSpawnSize)), Quaternion.identity);
   	  GameObject.Destroy(objectspawned.gameObject,3);
   	  timetemp = Time.time;
   }  
 
}