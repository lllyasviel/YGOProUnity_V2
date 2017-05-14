var mouseOverColor = Color.blue;
private var originalColor : Color;
function Start () {
	originalColor = GetComponent.<Renderer>().sharedMaterial.color;
}
function OnMouseEnter () {
	GetComponent.<Renderer>().material.color = mouseOverColor;
}

function OnMouseExit () {
	GetComponent.<Renderer>().material.color = originalColor;
}

function OnMouseDown () {
	var screenSpace = Camera.main.WorldToScreenPoint(transform.position);
	var offset = transform.position - Camera.main.ScreenToWorldPoint(Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
	while (Input.GetMouseButton(0))
	{
		var curScreenSpace = Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
		var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
		transform.position = curPosition;
		yield;
	}
}