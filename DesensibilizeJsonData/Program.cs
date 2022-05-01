using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

var files = Directory.EnumerateFiles(@"C:\Users\jacqu\source\repos\ErabliereApi\ErabliereApi.Integration.Test\StripeWebhookJson\StripeWebhookTest\NominalFlow");

foreach (var file in files)
{
    var json = JObject.Parse(File.ReadAllText(file));

    DesensibilizeData(json);

    Console.WriteLine(json.ToString());
    File.WriteAllText(file, json.ToString());
}

void DesensibilizeData(JObject json)
{
    foreach (var prop in json)
    {
        if (prop.Value is null)
            continue;

        var propValue = prop.Value;

        if (propValue.Type == JTokenType.String && prop.Key != "type" &&
                                                   prop.Key != "usage" &&
                                                   prop.Key != "object")
        {
            var element = propValue.ToString();

            if (element.Contains('_') || prop.Key == "fingerprint")
            {
                var elements = element.Split('_');

                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Length > 6)
                    {
                        elements[i] = Randomize(elements[i]);
                    }
                }

                element = string.Join("_", elements);
            }

            json[prop.Key] = element;
        }
        else if (propValue.Type == JTokenType.Object)
        {
            DesensibilizeData((JObject)propValue);
        }
    }
}

string Randomize(string str)
{
    var array = new List<char>(str);

    int n = array.Count;
    while (n > 1)
    {
        byte[] randomInt = RandomNumberGenerator.GetBytes(4);
        int k = Convert.ToInt32(randomInt[0]) % n;
        n--;
        char value = array[n];
        array[n] = array[k];
        array[k] = value;
    }

    return string.Join("", array);
}