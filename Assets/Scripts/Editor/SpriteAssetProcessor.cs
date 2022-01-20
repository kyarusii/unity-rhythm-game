using UnityEditor;
using UnityEngine;

namespace RGF.Editor
{
	internal class SpriteAssetProcessor : AssetPostprocessor
	{
		private void OnPostprocessTexture(Texture2D texture)
		{
			if (assetPath.StartsWith("Assets/Resources/Sprites/Note"))
			{
				TextureImporter importer = (TextureImporter)assetImporter;

				importer.textureType = TextureImporterType.Sprite;
				importer.spritePivot = new Vector2(0.5f, 0);
				importer.spriteImportMode = SpriteImportMode.Single;
				importer.spritePixelsPerUnit = 160;
			}
		}
	}
}