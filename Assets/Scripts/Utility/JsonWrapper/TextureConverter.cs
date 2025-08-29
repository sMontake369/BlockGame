using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TextureConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if(value is Texture texture)
        {
            writer.WriteValue(texture.name); // GameObjectの名前を出力
        }
        else 
        {
            serializer.Serialize(writer, value); // デフォルトのシリアライズ
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        Texture texture;
        try { texture = Addressables.LoadAssetAsync<Texture>(reader.Value).WaitForCompletion(); }
        catch (Exception) {texture = null;}
        
        if(texture == null)
        {
            Debug.LogWarning("存在しないキー: " + reader.Value);
            return Addressables.LoadAssetAsync<Texture>("defaultTexture").WaitForCompletion();
        }
        return texture; 
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Texture);
    }
}
