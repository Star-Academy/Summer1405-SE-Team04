using System.Text.RegularExpressions;
using G5;

class Program
{
    static void Main(string[] args)
    {
        var historyManager = new HistoryManager();
        while (true)
        {
            var input = Console.ReadLine();
            var cmdParts = Regex.Split(input.Trim(), @"\s+").ToList();
            switch (cmdParts[0])
            {
                case "SEARCH":
                    {
                        historyManager.Search(cmdParts[1]);
                        break;
                    }

                case "BACK":
                    {
                        if (!historyManager.Back())
                            Console.WriteLine("No back history");
                        break;
                    }

                case "FORWARD":
                    {
                        if (!historyManager.Forward())
                            Console.WriteLine("No forward history");
                        break;
                    }

                case "STATS":
                    {
                        var sortedEntries = historyManager.Stats(3);
                        for (int i = 0; i < sortedEntries.Count; i++)
                        {
                            Console.WriteLine(sortedEntries[i].Key + " " + sortedEntries[i].Value);
                        }
                        break;
                    }

                case "UNIQUE":
                    {
                        Console.WriteLine(historyManager.Unique());
                        break;
                    }

                case "EXIT":
                    {
                        return;
                    }
            }

            var current = historyManager.CurrentPage();
            if (current == null)
            {
                Console.WriteLine("No current page");
                continue;
            }
            else
            {
                Console.WriteLine("current: " + current);
            }
        }
    }
}
