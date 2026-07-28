using System.Text.RegularExpressions;
class Program
{
    static void Main(string[] args)
    {
        Stack<string> backHistory = new Stack<string>();
        Stack<string> forwardHistory = new Stack<string>();

        var searches = new Dictionary<string, int>();
    

        var tokens = new List<string>();
        
        while (true)
        {
            string input = Console.ReadLine();
            tokens = Regex.Split(input.Trim(), @"\s+").ToList();
            switch (tokens[0])
            {
                case "SEARCH": {
                    backHistory.Push(tokens[1]);
                    searches[tokens[1]] = searches.GetValueOrDefault(tokens[1], 0) + 1;
                    break;
                }

                case "BACK":
                {   
                    if (backHistory.Count == 1)
                    {
                        Console.WriteLine("No back history");
                        break;
                    }
                    var temp = backHistory.Pop();
                    forwardHistory.Push(temp);
                    break;
                    
                }

                case "FORWARD":
                {
                    if (forwardHistory.Count == 0)
                    {
                        Console.WriteLine("No forward history");
                        break;
                    }
                    var temp = forwardHistory.Pop();
                    backHistory.Push(temp);
                    break;
                    
                }
                
                case "STATS":
                {
                    var sortedSearches = searches.OrderBy(x => -x.Value).ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine(sortedSearches[i].Key + " " + sortedSearches[i].Value);
                    }
                    break;
                }

                case "UNIQUE":
                {
                    var uniqueSearches = searches.Keys.Count;
                    Console.WriteLine(uniqueSearches);
                    break;
                }
                case "EXIT":
                {
                    return;
                }
            }

            if (backHistory.Count == 0)
            {
                Console.WriteLine("No current page");
                continue;
            } else
            {
                string current = backHistory.Peek();
                Console.WriteLine("current: " + current);

            }

        }
    }
}
