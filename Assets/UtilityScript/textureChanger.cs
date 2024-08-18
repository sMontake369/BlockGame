using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public List<Texture2D> textures;
    int textureSize = 512;

    void Start()
    {
        Texture2D mergedTexture = MergeTextures(textures, textureSize);
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.material.mainTexture = mergedTexture;
    }

    Texture2D MergeTextures(List<Texture2D> textures, int textureSize)
    {
        float startTime = Time.realtimeSinceStartup;
        int count = textures.Count;
        int subTextureSize = textureSize / count;  // 各テクスチャのサイズ

        // 結果のテクスチャを作成
        Texture2D result = new Texture2D(textureSize, textureSize, TextureFormat.RGBA64, false);

        // 各テクスチャをリサイズしてコピー
        for (int i = 0; i < count; i++)
        {
            Texture2D texture = textures[i];
            int x = i * subTextureSize;

            // テクスチャの一部を新しいテクスチャにコピー
            Graphics.CopyTexture(texture, 0, 0, x, 0, subTextureSize, textureSize, result, 0, 0, x, 0);
        }

        result.Apply();
        Debug.Log("Time taken: " + (Time.realtimeSinceStartup - startTime));
        return result;
    }
}