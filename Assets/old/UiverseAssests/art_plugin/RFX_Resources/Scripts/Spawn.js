#pragma strict
var enemyPrefabs : GameObject[];

var prefabIndex = -1;

var Texttime : GUIText;

function SpawnSolo ()
{
	var object : GameObject = enemyPrefabs[prefabIndex];
	GameObject.Instantiate(object);
	Texttime.text=prefabIndex.ToString();
}


function SpawnEnemies ()
{
	
	if (prefabIndex >= enemyPrefabs.Length)
	{
		prefabIndex = 0;
	}
	
	var object : GameObject = enemyPrefabs[prefabIndex];
	GameObject.Instantiate(object);
	Texttime.text=prefabIndex.ToString();
}

function Update () {
	
	if (Input.GetButtonDown("Fire2"))
	{
		DestroyAllObjectsWithTag("Fx");
		SpawnSolo();
	}

	if (Input.GetButtonDown("Fire1"))
	{
		DestroyAllObjectsWithTag("Fx");
		prefabIndex++;
		SpawnEnemies();
	}

}

 function DestroyAllObjectsWithTag(tag : String) {
 
        var scan : GameObject;
        for(scan in GameObject.FindGameObjectsWithTag(tag)) {
              GameObject.Destroy(scan);
        }
        }