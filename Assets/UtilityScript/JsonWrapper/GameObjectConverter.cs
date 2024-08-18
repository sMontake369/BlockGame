using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameObjectConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is GameObject gameObject)
        {
            writer.WriteValue(gameObject.name); // GameObjectの名前を出力
        }
        else 
        {
            serializer.Serialize(writer, value); // デフォルトのシリアライズ
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        GameObject gameObject;
        try {gameObject = Addressables.LoadAssetAsync<GameObject>(reader.Value).WaitForCompletion();}
        catch (Exception) {gameObject = null;}

        if(gameObject == null)
        {
            Debug.LogWarning("存在しないキー: " + reader.Value);
            gameObject = Addressables.LoadAssetAsync<GameObject>("defaultGameObject").WaitForCompletion();
        }
        return gameObject;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(GameObject);
    }
}
