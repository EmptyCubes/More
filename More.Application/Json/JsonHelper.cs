using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace More.Application.Json
{
    public static class JsonHelper
    {
        public static Dictionary<string, object> FromJson(string json)
        {
            var jsonReader = new JsonTextReader(new StringReader(json));
            var inputs = new Dictionary<string, object>();
            ReadData(jsonReader, inputs, null);
            return inputs;
        }

        public static void ReadData(JsonTextReader reader, Dictionary<string, object> inputs, List<Dictionary<string, object>> currentList)
        {
            var propertyName = String.Empty;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        if (currentList == null)
                        {
                            if (String.IsNullOrEmpty(propertyName))
                            {
                                ReadData(reader, inputs, null);
                            }
                            else
                            {
                                var objectDictionary = new Dictionary<string, object>();
                                inputs.Add(propertyName, objectDictionary);
                                ReadData(reader, objectDictionary, null);
                            }
                        }
                        else
                        {
                            var objectDictionary = new Dictionary<string, object>();
                            currentList.Add(objectDictionary);
                            ReadData(reader, objectDictionary, currentList);
                        }
                        break;
                    case JsonToken.StartArray:
                        var list = new List<Dictionary<string, object>>();
                        ReadData(reader, inputs, list);
                        inputs.Add(propertyName, list);
                        break;
                    case JsonToken.EndObject:
                        return;
                    case JsonToken.EndArray:
                        return;
                        break;
                    case JsonToken.Boolean:
                        inputs.Add(propertyName, Convert.ToBoolean(reader.Value));
                        break;
                    case JsonToken.Float:
                        inputs.Add(propertyName, Convert.ToDouble(reader.Value));
                        break;
                    case JsonToken.Integer:
                        inputs.Add(propertyName, Convert.ToDouble(reader.Value));
                        break;
                    case JsonToken.Bytes:
                        inputs.Add(propertyName, Convert.ToInt32(reader.Value));
                        break;
                    case JsonToken.Date:
                        DateTime dateValue;
                        Int32 intValue;

                        if (DateTime.TryParse(reader.Value.ToString(), out dateValue))
                        {
                            inputs.Add(propertyName, dateValue);
                        }
                        else if (Int32.TryParse(reader.Value.ToString(), out intValue))
                        {
                            inputs.Add(propertyName, intValue);
                        }
                        else
                        {
                            inputs.Add(propertyName, reader.Value.ToString());
                        }

                        break;
                    case JsonToken.Null:
                        inputs.Add(propertyName, String.Empty);
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

        public static string GetJsonTrace(Dictionary<string, object> trace)
        {
            var sb = new StringBuilder();
            var writer = new JsonTextWriter(new StringWriter(sb));
            writer.WriteStartObject();
            WriteList(writer, trace);
            writer.WriteEndObject();
            return sb.ToString();
        }

        public static void WriteList(JsonTextWriter writer, Dictionary<string, object> items)
        {
            //writer.WriteStartArray();
            foreach (var item in items)
            {
                writer.WritePropertyName(item.Key);
                if (item.Value is IList<Dictionary<string, object>>)
                {
                    var itemList = item.Value as IList<Dictionary<string, object>>;
                    writer.WriteStartArray();
                    foreach (var x in itemList)
                    {
                        WriteList(writer, x);
                    }

                    writer.WriteEndArray();
                }
                else if (item.Value is Dictionary<string, object>)
                {
                    writer.WriteStartObject();
                    WriteList(writer, item.Value as Dictionary<string, object>);
                    writer.WriteEndObject();
                }

                else
                {
                    writer.WriteValue(item.Value);
                }


            }
            // writer.WriteEndArray();
        }
        public static string WriteData( Dictionary<string, object> items)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            var writer = new JsonTextWriter(new StringWriter(sb));
            WriteList(writer,items);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
