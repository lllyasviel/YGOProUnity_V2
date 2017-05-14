
using UnityEditor;

[CustomEditor(typeof(MegaUVTiles))]
public class MegaUVTilesEditor : MegaModifierEditor
{
	public override bool Inspector()
	{
		MegaUVTiles mod = (MegaUVTiles)target;

		EditorGUIUtility.LookLikeControls();

		mod.Frame = EditorGUILayout.IntField("Frame", mod.Frame);
		mod.TileWidth = EditorGUILayout.IntField("Tile Width", mod.TileWidth);
		mod.TileHeight = EditorGUILayout.IntField("Tile Height", mod.TileHeight);

		mod.off = EditorGUILayout.Vector2Field("Off", mod.off);
		mod.scale = EditorGUILayout.Vector2Field("Scale", mod.scale);

		mod.Animate = EditorGUILayout.Toggle("Animate", mod.Animate);

		mod.EndFrame = EditorGUILayout.IntField("End Frame", mod.EndFrame);
		mod.fps = EditorGUILayout.FloatField("Fps", mod.fps);
		mod.AnimTime = EditorGUILayout.FloatField("Anim Time", mod.AnimTime);

		mod.flipx = EditorGUILayout.Toggle("Flip X", mod.flipx);
		mod.flipy = EditorGUILayout.Toggle("Flip Y", mod.flipy);

		mod.loopMode = (MegaRepeatMode)EditorGUILayout.EnumPopup("Loop Mode", mod.loopMode);

		return false;
	}
}
