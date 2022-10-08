using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json.Linq;

var appSettingsFilePath = Directory.GetCurrentDirectory() + "//appSettings.json";
var appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(appSettingsFilePath));

var files = Directory.GetFiles(appSettings.WorkingDirectory, "*.*", SearchOption.AllDirectories)
    .Where(s => Path.GetExtension(s).ToLowerInvariant() == ".json");

foreach (var filePath in files)
{
    var originalText = File.ReadAllText(filePath);

    // Minifying JSON
    var obj = JsonConvert.DeserializeObject(originalText);
    var minifyJson = JsonConvert.SerializeObject(obj, Formatting.None);

    // Skip if no dataverse legacy
    if (!originalText.Contains("\"shared_commondataservice\"")) continue;
    minifyJson = minifyJson.Replace("\"shared_commondataservice\":{\"runtimeSource\":\"invoker\",", "\"shared_commondataservice\":{\"runtimeSource\":\"embedded\",");
    minifyJson = minifyJson.Replace("shared_commondataservice", "shared_commondataserviceforapps");
    minifyJson =
        minifyJson.Replace($"\"connectionReferenceLogicalName\":\"{appSettings.DataverseLegacyConnectionName}\"",
            $"\"connectionReferenceLogicalName\":\"{appSettings.DataverseConnectionName}\"");
    minifyJson = ReplaceDatasetToId(minifyJson);

    minifyJson = minifyJson.Replace("\"operationId\":\"PostItem_V2\"", "\"operationId\":\"CreateRecord\"");
    minifyJson = minifyJson.Replace("\"operationId\":\"PatchItem_V2\"", "\"operationId\":\"UpdateRecord\"");
    minifyJson = minifyJson.Replace("\"operationId\":\"DeleteItem\"", "\"operationId\":\"DeleteRecord\"");
    minifyJson = minifyJson.Replace("\"operationId\":\"GetItem_V2\"", "\"operationId\":\"GetItem\"");
    minifyJson = minifyJson.Replace("\"operationId\":\"GetItems_V2\"", "\"operationId\":\"ListRecords\"");
    obj = JsonConvert.DeserializeObject(minifyJson);

    var modifiedText = JsonConvert.SerializeObject(obj, Formatting.Indented);

    File.WriteAllText(filePath, modifiedText);
    Console.WriteLine("Replaced..");
}

string RemoveEmptyValues(string text)
{
    var splitParameters = text.Split("\"item/").ToArray();
    var result = new List<string>();
    foreach (var parameter in splitParameters)
    {
        var splitValues = parameter.Split(":");
        if (splitValues[0].StartsWith('"'))
        {
            result.Add(parameter);
            continue;
        }
        if (splitValues.Length == 2 && splitValues[1].Contains("\"\"")) continue;
        if (splitValues.Length > 2 && splitValues[1].Contains("\"\""))
        {
            splitValues[1] = splitValues[1].Replace("\"\"", "");
            var tempText = string.Join(":", splitValues.Skip(1));
            result.Add(tempText);
            continue;
        }

        result.Add("\"item/" + parameter);
    }
    var temp = string.Join("", result);
    return temp;
}

string ReplaceDatasetToId(string text)
{
    var splitParameters = text.Split("\"parameters\":{").ToArray();
    if (!splitParameters.Any(e => e.Contains("\"dataset\"") &&
                                 e.Contains("\"table\"") && e.Contains("\"id\""))) return text;

    var copySplitParameters = splitParameters.ToArray();
    for (var i = 0; i < splitParameters.Length; i++)
    {
        var parameter = splitParameters[i];
        var valid = parameter.Contains("\"dataset\"") &&
           parameter.Contains("\"table\"");
        if (!valid) continue;
        var containsId = parameter.Contains("\"id\"");
        var find = containsId ? "\"id\"" : "\"table\"";
        var ended = parameter.IndexOf(find, StringComparison.CurrentCulture);
        var findText = parameter.Substring(0, ended) + find;
        var findTable = findText.IndexOf("\"table\"", StringComparison.CurrentCulture);
        var findTableText = findText.Substring(0, findTable);

        var result = parameter.Replace(findTableText, "")
            .Replace("\"table\"", "\"entityName\"")
            .Replace("\"id\"", "\"recordId\"");
        copySplitParameters[i] = RemoveEmptyValues(result);
    }

    return string.Join("\"parameters\":{", copySplitParameters);
}


Console.ReadKey();
public class AppSettings
{
    public string DataverseLegacyConnectionName { get; set; } = null!;
    public string DataverseConnectionName { get; set; } = null!;
    public string WorkingDirectory { get; set; } = null!;
}