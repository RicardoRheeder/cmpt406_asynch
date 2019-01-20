using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

public static class JsonConversion {

    public static T CreateFromJson<T>(string json, System.Type type) {
        using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json))) {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(type);
            return (T) Convert.ChangeType(deserializer.ReadObject(stream), type);
        }
    }

    //Note that the "thingToConvert" object must actually match the type of the "type"
    public static string ConvertObjectToJson(System.Type type, System.Object thingToConvert) {
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
}
