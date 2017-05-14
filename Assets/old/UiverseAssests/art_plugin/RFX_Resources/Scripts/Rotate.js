
#pragma strict

var Speed : float = 3.0;  

var Rot : float = 80.0;  

private var bottom : float;

function Awake () {
	bottom = transform.position.y;
}

function Update () {
	transform.Rotate(Vector3(0, Rot, 0) * Time.deltaTime, Space.World);
}