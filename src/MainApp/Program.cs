using System.Text.RegularExpressions;
using G5;


Strings.currentLanguage = "en";
var appStrings = Strings.Get();
var historyManager = new HistoryManager();

while (true)
{
    var input = Console.ReadLine();
    var cmdParts = Regex.Split(input.Trim(), @"\s+").ToList();

    if (cmdParts[0] == appStrings.Search)
    {
        historyManager.Search(cmdParts[1]);
    }
    else if (cmdParts[0] == appStrings.Back)
    {
        if (!historyManager.Back())
            Console.WriteLine(appStrings.NoBack);
    }
    else if (cmdParts[0] == appStrings.Forward)
    {
        if (!historyManager.Forward())
            Console.WriteLine(appStrings.NoForward);
    }
    else if (cmdParts[0] == appStrings.Stats)
    {
        var sortedEntries = historyManager.Stats(3);
        for (int i = 0; i < sortedEntries.Count; i++)
        {
            Console.WriteLine(sortedEntries[i].Key + " " + sortedEntries[i].Value);
        }
    }
    else if (cmdParts[0] == appStrings.Unique)
    {
        Console.WriteLine(historyManager.Unique());
    }
    else if (cmdParts[0] == appStrings.Exit)
    {
        return;
    }

    var current = historyManager.CurrentPage();
    if (current == null)
    {
        Console.WriteLine(appStrings.NoCurrent);
        continue;
    }
    else
    {
        Console.WriteLine(appStrings.Current + current);
    }
}

