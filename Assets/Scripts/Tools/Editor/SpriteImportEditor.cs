using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.U2D.Aseprite;

public class P16Import : AssetPostprocessor {
    void OnPreprocessTexture() {
        TextureImporter importer = assetImporter as TextureImporter;
        String name = importer.assetPath.ToLower(); // перевод название файла в прописные буквы для проверки
        if (name.Substring(name.Length - 4, 4)==".png") { // проверка = пнг ли файл
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.spritePixelsPerUnit = 32f; /// <<===== PPU
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            /* 
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            importer.SetTextureSettings(settings);
            */
            
            //importer.spritePivot = new Vector2(0.5f, 0f);
            //importer.spritePivot = new Vector2(-1f,0);
        }
    }
}