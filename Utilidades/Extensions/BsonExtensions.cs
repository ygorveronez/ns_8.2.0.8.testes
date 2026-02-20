using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewtonsoftJson = Newtonsoft.Json;

namespace Utilidades.Extensions
{    
    public static class BsonExtensions
    {
        public static BsonDocument ToBsonDocument<T>(this T data)
        {
            if (data == null)
                return null;

            if (data is BsonDocument doc)
                return doc;

            if (data is System.Collections.IEnumerable enumerable && !(data is string))
            {
                var bsonArray = new BsonArray();
                foreach (var item in enumerable)
                {
                    if (item is BsonValue bsonValue)
                    {
                        bsonArray.Add(bsonValue);
                    }
                    else if (item is BsonDocument itemDoc)
                    {
                        bsonArray.Add(itemDoc);
                    }
                    else
                    {
                        string itemJson = NewtonsoftJson.JsonConvert.SerializeObject(item);
                        bsonArray.Add(BsonDocument.Parse(itemJson));
                    }
                }
                return new BsonDocument { { "items", bsonArray } };
            }

            string json = NewtonsoftJson.JsonConvert.SerializeObject(data);
            return BsonDocument.Parse(json);
        }

        public static T FromBsonDocument<T>(this BsonDocument document)
        {
            if (document == null)
                return default(T);

            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                BsonArray array = null;
                
                if (document.Contains("items") && document["items"].IsBsonArray)
                {
                    array = document["items"].AsBsonArray;
                }
                else
                {
                    foreach (var element in document.Elements)
                    {
                        if (element.Value.IsBsonArray)
                        {
                            array = element.Value.AsBsonArray;
                            break;
                        }
                    }
                }
                
                if (array != null)
                {
                    var itemType = typeof(T).GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var list = Activator.CreateInstance(listType) as IList;
                    
                    foreach (var item in array)
                    {
                        object deserialized;
                        
                        if (item is BsonDocument itemDoc)
                        {
                            try
                            {
                                deserialized = BsonSerializer.Deserialize(itemDoc, itemType);
                            }
                            catch
                            {
                                var json = itemDoc.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
                                deserialized = NewtonsoftJson.JsonConvert.DeserializeObject(json, itemType);
                            }
                        }
                        else
                        {
                            try
                            {
                                var wrapperDoc = new BsonDocument("item", item);
                                deserialized = BsonSerializer.Deserialize(wrapperDoc, itemType);
                            }
                            catch
                            {
                                var json = item.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.Strict });
                                deserialized = NewtonsoftJson.JsonConvert.DeserializeObject(json, itemType);
                            }
                        }
                        
                        list.Add(deserialized);
                    }
                    
                    return (T)(object)list;
                }
                
                if (document.ElementCount == 0)
                {
                    return default(T);
                }
            }

            if (document.Contains("items") && document["items"].IsBsonArray)
            {
                var array = document["items"].AsBsonArray;
                var arrayDoc = new BsonDocument("items", array);
                return BsonSerializer.Deserialize<T>(arrayDoc);
            }

            if (document.Contains("value"))
            {
                var value = document["value"];
                if (value is BsonDocument valueDoc)
                {
                    return BsonSerializer.Deserialize<T>(valueDoc);
                }
                else
                {
                    var json = value.ToJson();
                    return NewtonsoftJson.JsonConvert.DeserializeObject<T>(json);
                }
            }

            return BsonSerializer.Deserialize<T>(document);
        }
    }
}

