using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace CustomJsonConverters
{
    public class MultiObjectDuplicateKeyConverter<T> : JsonConverter where T : class, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<T>();
            var currentObject = new T();
            reader.SupportMultipleContent = true; 

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    result.Add(currentObject);
                    currentObject = new T();
                }
                else if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = (string)reader.Value;
                    reader.Read();
                    var value = reader.Value;

                    var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (property != null && property.PropertyType.IsGenericType && 
                        property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var listType = property.PropertyType.GetGenericArguments()[0];
                        var list = property.GetValue(currentObject) as IList;

                        if (list == null)
                        {
                            list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
                            property.SetValue(currentObject, list);
                        }

                        list.Add(Convert.ChangeType(value, listType));
                    }
                }
            }
            result.Add(currentObject);

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("This converter is only for reading JSON, not writing.");
        }
    }
}