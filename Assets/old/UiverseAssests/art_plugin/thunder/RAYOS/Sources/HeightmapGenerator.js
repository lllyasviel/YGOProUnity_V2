// This script is placed in public domain. The author takes no responsibility for any possible harm.

var heightMap : Texture2D;
var material : Material;
var size = Vector3(200, 30, 200);

function Start ()
{
	GenerateHeightmap();
}

function GenerateHeightmap ()
{
	// Create the game object containing the renderer
	gameObject.AddComponent(MeshFilter);
	gameObject.AddComponent.<MeshRenderer>();
	if (material)
		GetComponent.<Renderer>().material = material;
	else
		GetComponent.<Renderer>().material.color = Color.white;

	// Retrieve a mesh instance
	var mesh : Mesh = GetComponent(MeshFilter).mesh;

	var width : int = Mathf.Min(heightMap.width, 255);
	var height : int = Mathf.Min(heightMap.height, 255);
	var y = 0;
	var x = 0;

	// Build vertices and UVs
	var vertices = new Vector3[height * width];
	var uv = new Vector2[height * width];
	var tangents = new Vector4[height * width];
	
	var uvScale = Vector2 (1.0 / (width - 1), 1.0 / (height - 1));
	var sizeScale = Vector3 (size.x / (width - 1), size.y, size.z / (height - 1));
	
	for (y=0;y<height;y++)
	{
		for (x=0;x<width;x++)
		{
			var pixelHeight = heightMap.GetPixel(x, y).grayscale;
			var vertex = Vector3 (x, pixelHeight, y);
			vertices[y*width + x] = Vector3.Scale(sizeScale, vertex);
			uv[y*width + x] = Vector2.Scale(Vector2 (x, y), uvScale);

			// Calculate tangent vector: a vector that goes from previous vertex
			// to next along X direction. We need tangents if we intend to
			// use bumpmap shaders on the mesh.
			var vertexL = Vector3( x-1, heightMap.GetPixel(x-1, y).grayscale, y );
			var vertexR = Vector3( x+1, heightMap.GetPixel(x+1, y).grayscale, y );
			var tan = Vector3.Scale( sizeScale, vertexR - vertexL ).normalized;
			tangents[y*width + x] = Vector4( tan.x, tan.y, tan.z, -1.0 );
		}
	}
	
	// Assign them to the mesh
	mesh.vertices = vertices;
	mesh.uv = uv;

	// Build triangle indices: 3 indices into vertex array for each triangle
	var triangles = new int[(height - 1) * (width - 1) * 6];
	var index = 0;
	for (y=0;y<height-1;y++)
	{
		for (x=0;x<width-1;x++)
		{
			// For each grid cell output two triangles
			triangles[index++] = (y     * width) + x;
			triangles[index++] = ((y+1) * width) + x;
			triangles[index++] = (y     * width) + x + 1;

			triangles[index++] = ((y+1) * width) + x;
			triangles[index++] = ((y+1) * width) + x + 1;
			triangles[index++] = (y     * width) + x + 1;
		}
	}
	// And assign them to the mesh
	mesh.triangles = triangles;
		
	// Auto-calculate vertex normals from the mesh
	mesh.RecalculateNormals();
	
	// Assign tangents after recalculating normals
	mesh.tangents = tangents;
}
