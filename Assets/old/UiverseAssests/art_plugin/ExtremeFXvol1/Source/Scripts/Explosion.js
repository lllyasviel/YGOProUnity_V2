var Force:int;
var Radius:int;
var Sounds:AudioClip[];
function Start () {
	var explosionPos : Vector3 = transform.position;
    var colliders : Collider[] = Physics.OverlapSphere (explosionPos, Radius);
	if(Sounds.length>0){
		AudioSource.PlayClipAtPoint(Sounds[Random.Range(0,Sounds.length)],transform.position);
	}
    for (var hit : Collider in colliders) {

        if (hit.GetComponent.<Rigidbody>())
            hit.GetComponent.<Rigidbody>().AddExplosionForce(Force, explosionPos, Radius, 3.0);
    }  
}

function Update () {

}