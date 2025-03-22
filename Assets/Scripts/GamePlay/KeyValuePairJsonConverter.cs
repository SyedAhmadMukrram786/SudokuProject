using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class KeyValuePairJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Dictionary<int, int> dictionary)
        {
            writer.WriteStartObject();
            foreach (var item in dictionary)
            {
                writer.WritePropertyName(item.Key.ToString()); // Write key as property name
                writer.WriteValue(item.Value); // Write value
            }
            writer.WriteEndObject();
        }
        else if (value is List<KeyValuePair<int, int>> list)
        {
            writer.WriteStartArray();
            foreach (var item in list)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(item.Key.ToString()); // Write key as property name
                writer.WriteValue(item.Value); // Write value
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
        else
        {
            throw new JsonSerializationException("Unsupported type for KeyValuePairJsonConverter");
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Dictionary<int, int>) || objectType == typeof(List<KeyValuePair<int, int>>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (objectType == typeof(Dictionary<int, int>))
        {
            JObject jsonObject = JObject.Load(reader);
            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            foreach (var property in jsonObject.Properties())
            {
                int key = int.Parse(property.Name);
                int value = property.Value.ToObject<int>();
                dictionary.Add(key, value);
            }
            return dictionary;
        }
        else if (objectType == typeof(List<KeyValuePair<int, int>>))
        {
            JArray jsonArray = JArray.Load(reader);
            List<KeyValuePair<int, int>> keyValueList = new List<KeyValuePair<int, int>>();

            foreach (JObject obj in jsonArray)
            {
                foreach (var property in obj.Properties())
                {
                    int key = int.Parse(property.Name);
                    int value = property.Value.ToObject<int>();
                    keyValueList.Add(new KeyValuePair<int, int>(key, value));
                }
            }
            return keyValueList;
        }

        throw new JsonSerializationException("Unsupported type for KeyValuePairJsonConverter");
    }
}
