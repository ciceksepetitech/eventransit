using System.Text.Json;
using EvenTransit.Messaging.Core.Abstractions;

namespace EvenTransit.Service.Mappers;

public class CustomObjectMapper : ICustomObjectMapper
{
    public Dictionary<string, object> Map(JsonElement body, Dictionary<string, string> source)
    {
        var obj = new Dictionary<string, object>();
        
        foreach (var kv in source)
        {
            var target = kv.Value;
            var nestedProps = kv.Key.Split(".");
            
            var leaf = body;
            
            foreach (var prop in nestedProps)
            {
                if (!leaf.TryGetProperty(prop, out var element)) continue;
                var nested = false;
                
                switch (element.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                        break;
                    case JsonValueKind.Object:
                        leaf = element;
                        nested = true;
                        break;
                    case JsonValueKind.Array:
                        obj[target] = element;
                        break;
                    case JsonValueKind.String:
                        obj[target] = element.GetString() ?? string.Empty;
                        break;
                    case JsonValueKind.Number:
                        if (element.TryGetInt32(out var i32)) obj[target] = new JsonProperty();
                        if (element.TryGetDecimal(out var d)) obj[target] = d;
                        if (element.TryGetDouble(out var db)) obj[target] = db;
                        if (element.TryGetInt16(out var i16)) obj[target] = i16;
                        if (element.TryGetInt64(out var i64)) obj[target] = i64;
                        if (element.TryGetUInt16(out var ui16)) obj[target] = ui16;
                        if (element.TryGetUInt32(out var ui32)) obj[target] = ui32;
                        if (element.TryGetUInt64(out var ui64)) obj[target] = ui64;
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        obj[target] = leaf.GetBoolean();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(body));
                }
                    
                if (!nested) break;
            }
        }

        return obj;
    }
}