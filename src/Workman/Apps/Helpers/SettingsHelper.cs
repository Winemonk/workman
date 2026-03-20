using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Workman.Apps.Configs;

namespace Workman.Apps.Helpers
{
    internal static class SettingsHelper
    {
        private static JsonStringEnumConverter _JSON_STRING_ENUM_CONVERTER = new JsonStringEnumConverter();
        private static JsonSerializerOptions _JSON_SERIALIZER_OPTIONS = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters = { _JSON_STRING_ENUM_CONVERTER },
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static bool Save(this AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, _JSON_SERIALIZER_OPTIONS);
                JsonNode jsonNode = JsonNode.Parse(File.ReadAllText("appsettings.json")) ?? new JsonObject();
                jsonNode[nameof(AppSettings)] = JsonNode.Parse(json);
                File.WriteAllText("appsettings.json", jsonNode.ToJsonString(_JSON_SERIALIZER_OPTIONS));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
