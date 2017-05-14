private var mesh :  Mesh;
var showFPS = false;

function Start ()
{
	GetComponent.<GUIText>().material.color = Color.black;
}

function LateUpdate () {
	if (!mesh)
		mesh = FindObjectOfType (Mesh);
	
	if (Time.frameCount % 5) {
		var vps : int = (mesh.vertexCount / Time.smoothDeltaTime) / 1000;
		GetComponent.<GUIText>().text = "Vertices per second:\n" + vps + "k";
		
		var fps : int = (1.0 / Time.smoothDeltaTime);
		if (showFPS)
			GetComponent.<GUIText>().text += "\nFrames per second:\n" + fps;
	}
}