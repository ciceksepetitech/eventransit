using System.Text.Json;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface ICustomObjectMapper
{
    Dictionary<string, object> Map(JsonElement body, Dictionary<string, string> source);
}