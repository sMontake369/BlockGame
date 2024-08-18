using System;
using Newtonsoft.Json;
using UnityEngine;


public class ColorConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Color color)
        {
            writer.WriteValue(color.r);
            writer.WriteValue(color.g);
            writer.WriteValue(color.b);
            writer.WriteValue(color.a);
        }
        else 
        {
            serializer.Serialize(writer, value); // デフォルトのシリアライズ
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
    {
        float r = (float)reader.ReadAsDouble();
        float g = (float)reader.ReadAsDouble();
        float b = (float)reader.ReadAsDouble();
        float a = (float)reader.ReadAsDouble();
        Color color = new Color(r, g, b, a);
        return color;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color);
    }
}