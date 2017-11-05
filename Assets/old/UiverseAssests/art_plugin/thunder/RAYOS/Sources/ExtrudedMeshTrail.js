// Generates an extrusion trail from the attached mesh
// Uses the MeshExtrusion algorithm in MeshExtrusion.cs to generate and preprocess the mesh.
var time = 2.0;
var autoCalculateOrientation = true;
var minDistance = 0.1;
var invertFaces = false;
private var srcMesh : Mesh;
private var precomputedEdges : MeshExtrusion.Edge[];

class ExtrudedTrailSection
{
	var point : Vector3;
	var matrix : Matrix4x4;
	var time : float;
}

function Start ()
{
	srcMesh = GetComponent(MeshFilter).sharedMesh;
	precomputedEdges = MeshExtrusion.BuildManifoldEdges(srcMesh);
}

private var sections = new Array();

function LateUpdate () {
	var position = transform.position;
	var now = Time.time;
	var tmp : ExtrudedTrailSection = sections[sections.length - 1];
	// Remove old sections
	while (sections.length > 0 && now > (tmp.time + time)) {
		sections.Pop();
		tmp = sections[sections.length - 1];
	}
	tmp = sections[0];
	// Add a new trail section to beginning of array
	if (sections.length == 0 || (tmp.point - position).sqrMagnitude > minDistance * minDistance)
	{
		var section = ExtrudedTrailSection ();
		section.point = position;
		section.matrix = transform.localToWorldMatrix;
		section.time = now;
		sections.Unshift(section);
	}
	
	// We need at least 2 sections to create the line
	if (sections.length < 2)
		return;

	var worldToLocal = transform.worldToLocalMatrix;
	var finalSections = new Matrix4x4[sections.length];
	var previousRotation : Quaternion;

	var fsection : ExtrudedTrailSection = sections[0];
	var ssection : ExtrudedTrailSection = sections[1];
	for (var i=0;i<sections.length;i++)
	{
		if (autoCalculateOrientation)
		{
			if (i == 0)
			{
				var direction = fsection.point - ssection.point;
				var rotation = Quaternion.LookRotation(direction, Vector3.up);
				previousRotation = rotation;
				finalSections[i] = worldToLocal * Matrix4x4.TRS(position, rotation, Vector3.one);	
			}
			// all elements get the direction by looking up the next section
			else if (i != sections.length - 1)
			{	
				var ftmp : ExtrudedTrailSection = sections[i];
				var stmp : ExtrudedTrailSection = sections[i+1];
				direction = ftmp.point - stmp.point;
				rotation = Quaternion.LookRotation(direction, Vector3.up);
				
				// When the angle of the rotation compared to the last segment is too high
				// smooth the rotation a little bit. Optimally we would smooth the entire sections array.
				if (Quaternion.Angle (previousRotation, rotation) > 20)
					rotation = Quaternion.Slerp(previousRotation, rotation, 0.5);
					
				previousRotation = rotation;
				finalSections[i] = worldToLocal * Matrix4x4.TRS(ftmp.point, rotation, Vector3.one);
			}
			// except the last one, which just copies the previous one
			else
			{
				finalSections[i] = finalSections[i-1];
			}
		}
		else
		{
			if (i == 0)
			{
				finalSections[i] = Matrix4x4.identity;
			}
			else
			{
				tmp = sections[i];
				finalSections[i] = worldToLocal * tmp.matrix;
			}
		}
	}
	
	// Rebuild the extrusion mesh	
	MeshExtrusion.ExtrudeMesh (srcMesh, GetComponent(MeshFilter).mesh, finalSections, precomputedEdges, invertFaces);
}

@script RequireComponent (MeshFilter)
