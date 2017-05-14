var speed = 10.0;

GetComponent.<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward) * speed;