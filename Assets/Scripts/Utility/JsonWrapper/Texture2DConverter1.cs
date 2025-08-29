using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Texture2DConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if(value is Texture2D texture2D)
        {
            writer.WriteValue(texture2D.name); // GameObjectの名前を出力
        }
        else 
        {
            serializer.Serialize(writer, value); // デフォルトのシリアライズ
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        Texture2D Texture2D;
        try { Texture2D = Addressables.LoadAssetAsync<Texture2D>(reader.Value).WaitForCompletion(); }
        catch (Exception) {Texture2D = null;}
        
        if(Texture2D == null)
        {
            Debug.LogWarning("存在しないキー: " + reader.Value);
            return Addressables.LoadAssetAsync<Texture2D>("defaultTexture2D").WaitForCompletion();
        }
        return Texture2D; 
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Texture2D);
    }
}
