using System;
using Newtonsoft.Json;
using UnityEngine;

public class Vector3Converter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Vector3 vector3)
        {
            writer.WriteStartObject();
            writer.Formatting = Formatting.None;
            writer.WritePropertyName(nameof(Vector3.x));
            writer.WriteValue(vector3.x);
            writer.WritePropertyName(nameof(Vector3.y));
            writer.WriteValue(vector3.y);
            writer.WritePropertyName(nameof(Vector3.z));
            writer.WriteValue(vector3.z);
            writer.WriteEndObject();
            writer.Formatting = Formatting.Indented;
        }
        else 
        {
            serializer.Serialize(writer, value); // デフォルトのシリアライズ
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        reader.Read();
        float x = (float)reader.ReadAsDouble();
        reader.Read();
        float y = (float)reader.ReadAsDouble();
        reader.Read();
        float z = (float)reader.ReadAsDouble();
        reader.Read();
        Vector3 vector3 = new Vector3(x, y, z);
        return vector3;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3);
    }
}

public class Vector3IntConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Vector3Int vector3Int)
        {
            writer.WriteStartObject();
            writer.Formatting = Formatting.None;
            writer.WritePropertyName(nameof(Vector3.x));
            writer.WriteValue(vector3Int.x);
            writer.WritePropertyName(nameof(Vector3.y));
            writer.WriteValue(vector3Int.y);
            writer.WritePropertyName(nameof(Vector3.z));
            writer.WriteValue(vector3Int.z);
            writer.WriteEndObject();
            writer.Formatting = Formatting.Indented;
        }
        else 
        {
            serializer.Serialize(writer, value); // デフォルトのシリアライズ
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        reader.Read();
        int x = (int)reader.ReadAsInt32();
        reader.Read();
        int y = (int)reader.ReadAsInt32();
        reader.Read();
        int z = (int)reader.ReadAsInt32();
        reader.Read();
        Vector3Int vector3Int = new Vector3Int(x, y, z);
        return vector3Int;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3Int);
    }
}