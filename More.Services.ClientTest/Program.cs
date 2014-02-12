using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using More.Services.ClientTest.MoreService;
using Newtonsoft.Json;

namespace More.Services.ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string json =
                @"{ age: 25, cars: [ { id: 1 }, { id: 2 }]}";

            var jsonReader = new JsonTextReader(new StringReader(json));
            Dictionary<string, object>
                inputs = new Dictionary<string, object>();

            ReadData(jsonReader, inputs, null);

            Console.ReadLine();
        }

        public static void ReadData(JsonTextReader reader, Dictionary<string, object> inputs, List<Dictionary<string,object>> currentList )
        {
            var propertyName = string.Empty;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        if (currentList == null)
                        {
                            if (string.IsNullOrEmpty(propertyName))
                            {
                                ReadData(reader, inputs, null);
                            }
                            else
                            {
                                var objectDictionary = new Dictionary<string, object>();
                                inputs.Add(propertyName, objectDictionary);
                                ReadData(reader, objectDictionary, null);
                            }
                        } else
                        {
                             var objectDictionary = new Dictionary<string, object>();
                            currentList.Add(objectDictionary);
                            ReadData(reader,objectDictionary,currentList);
                        }
                        break;
                    case JsonToken.StartArray:
                        var list = new List<Dictionary<string, object>>();
                        ReadData(reader,inputs,list);
                        inputs.Add(propertyName,list);
                        break;
                    case JsonToken.EndObject:
                        return;
                    case JsonToken.EndArray:
                        return;
                        break;
                    case JsonToken.Boolean:
                        inputs.Add(propertyName,Convert.ToBoolean(reader.Value));
                        break;
                    case JsonToken.Float:
                        inputs.Add(propertyName, Convert.ToDouble(reader.Value));
                        break;
                    case JsonToken.Integer:
                        inputs.Add(propertyName, Convert.ToDouble(reader.Value));
                        break;
                    case JsonToken.Bytes:
                    case JsonToken.Date:
                        inputs.Add(propertyName, Convert.ToDateTime(reader.Value));
                        break;
                    case JsonToken.Null:
                        inputs.Add(propertyName, string.Empty);
                        break;
                    case JsonToken.String:
                        inputs.Add(propertyName, reader.Value.ToString());
                        break;
                    case JsonToken.PropertyName:
                        propertyName = reader.Value.ToString();
                        break;
                }

            }
        }
    }
}
