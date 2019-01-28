using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

//Helper class that is used for server communication
public static class JsonConversion {

    //Takes a json string and a type and returns an object of Type from the JSON string
    //It's expected that the user knows how to use it and Passes in the type they expect
    public static T CreateFromJson<T>(string json, System.Type type) {
        using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json))) {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(type);
            return (T) Convert.ChangeType(deserializer.ReadObject(stream), type);
        }
    }

    //Takes in a type and an object and serializes it into JSON
    //Used for server communication
    public static string ConvertObjectToJson<T>(System.Type type, T thingToConvert) {
        if (thingToConvert != null) {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(type);
            ser.WriteObject(stream, thingToConvert);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
        else {
            throw new InvalidDataException("Cannot convert a null object to JSON");
        }
    }

    //Used when we need to send a single field to the server, easier than creating data contracts for each field
    public static string GetJsonForSingleField(string key, string value) {
        return "{\"" + key + "\":\"" + value + "\"}";
    }
}
