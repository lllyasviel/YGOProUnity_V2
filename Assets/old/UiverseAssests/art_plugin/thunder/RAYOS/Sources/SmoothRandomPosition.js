// This script is placed in public domain. The author takes no responsibility for any possible harm.

// Moves the object along as far as range units randomly but in a smooth way.
// This script requires the Noise.cs script.
var speed = 1.0;
var range = Vector3 (1.0, 1.0, 1.0);

private var noise = new Perlin();
private var position : Vector3;

function Start()
{
	position = transform.position;
}

function Update () {
	transform.position = position + Vector3.Scale(SmoothRandom.GetVector3(speed), range);
}