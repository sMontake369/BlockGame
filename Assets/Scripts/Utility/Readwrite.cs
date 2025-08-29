using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public static class ReadWrite
{
    static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Converters = new List<JsonConverter> 
        {
            new StringEnumConverter(),
            new Vector3Converter(),
            new Vector3IntConverter(),
            new GameObjectConverter(),
            new TextureConverter(),
            new Texture2DConverter(),
            new ColorConverter(),
        },
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public static T Read<T>(string fileName) 
    {
        TextAsset textAsset = Addressables.LoadAssetAsync<TextAsset>(fileName).WaitForCompletion();
        return JsonConvert.DeserializeObject<T>(textAsset.ToString(), settings);
    } 

    public static T Read<T>(TextAsset text) 
    {
        T obj = default(T);
        try {obj = JsonConvert.DeserializeObject<T>(text.ToString(), settings); }
        catch (System.Exception) { Debug.Log("いくつかのエラー"); }
        Debug.Log("Loaded!");
        return obj;
    } 

    public static void Write<StageData>(string fileName, StageData data)
    {
        string json = JsonConvert.SerializeObject(data, settings);
        string path = "Assets/";
        StreamWriter wr = new StreamWriter(path + fileName, false);
        wr.WriteLine(json); 
        wr.Close(); 
        Debug.Log("Saved!");
    }
}