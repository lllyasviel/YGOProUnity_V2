var target : Transform;
var distanceMin = 10.0;
var distanceMax = 15.0;
var distanceInitial = 12.5;
var scrollSpeed = 1.0;

var xSpeed = 250.0;
var ySpeed = 120.0;

var yMinLimit = -20;
var yMaxLimit = 80;

private var x = 0.0;
private var y = 0.0;
private var distanceCurrent = 0.0;

@script AddComponentMenu ("Camera-Control/Key Mouse Orbit")

function Start () {
    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

	distanceCurrent = distanceInitial;

	// Make the rigid body not change rotation
   	if (GetComponent.<Rigidbody>())
		GetComponent.<Rigidbody>().freezeRotation = true;
}

function LateUpdate () {
    if (target) {
        x += Input.GetAxis("Horizontal") * xSpeed * 0.02;
        y -= Input.GetAxis("Vertical") * ySpeed * 0.02;
 		distanceCurrent -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
		
		distanceCurrent = Mathf.Clamp(distanceCurrent, distanceMin, distanceMax);
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
 		       
        var rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * Vector3(0.0, 0.0, -distanceCurrent) + target.position;
        
        transform.rotation = rotation;
        transform.position = position;
    }
}

static function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}