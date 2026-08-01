public class AppStrings
{
    public string Search {get; init; }
    public string Back {get; init; }
    public string Forward {get; init; }
    public string Stats {get; init; }
    public string Unique {get; init; }
    public string Exit {get; init; }

    public string NoForward {get; init; }
    public string NoBack {get; init; }
    public string NoCurrent {get; init; }
    public string Current {get; init; }

}


class Strings
{
    public static string currentLanguage = "en";

    private static Dictionary<string, AppStrings> translations = new()
    {
        ["en"] = new()
        {
            // Commands
            Search = "SEARCH",
            Back = "BACK",
            Forward = "FORWARD",
            Stats = "STATS",
            Unique = "UNIQUE",
            Exit = "EXIT",

            // Outputs
            NoForward = "No forward history",
            NoBack = "No back history",
            NoCurrent = "No current page",
            Current = "current: "
        },
        ["fa"] = new()
        {
            // Commands
            Search = "search",
            Back = "bazgasht",
            Forward = "jelo",
            Stats = "vaziat",
            Unique = "yegane",
            Exit = "khoroj",

            // Outputs
            NoForward = "jeloye bishtar vojod nadarad",
            NoBack = "bazgasht vojod nadarad",
            NoCurrent = "safe jari vojod nadarad",
            Current = "safe jari: "
        }
    };

    public static AppStrings Get()
    {
        return translations.ContainsKey(currentLanguage) ? translations[currentLanguage] : translations["en"];
    }

}