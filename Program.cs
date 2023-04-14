using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using DataStructureProject.classes;

namespace DataStructureProject;

abstract class Application
{
    private static async Task Main(string[] args)
    {
        // Fake epoch time based on the current time
        long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // TODO for the BONUS
        // create the priority queues
        // PQueueTrending trendingNews = new PQueueTrending(1000);
        // PQueueRecent recentNews = new PQueueRecent(1000);

        // create the list of news
        List<NewsItem> news = new List<NewsItem>();

        // create a stack to keep track of the read news items
        Stack<NewsItem> recents = new Stack<NewsItem>();

        // read json file as a stream
        try
        {
            const string filePath = "/Users/sergio/RiderProjects/DataStructureProject/data/MOCK_DATA.json";
            var fileStream = File.OpenRead(filePath);
            await using var file = fileStream;
            using var streamReader = new StreamReader(file);
            await using var jsonTextReader = new JsonTextReader(streamReader);
            while (await jsonTextReader.ReadAsync())
            {
                if (jsonTextReader.TokenType != JsonToken.StartObject) continue;
                var obj = await JObject.LoadAsync(jsonTextReader);
                NewsItem? newsItem = obj.ToObject<NewsItem>();
                if (newsItem != null) news.Add(newsItem);
            }
        }
        catch{
            Console.WriteLine("Error reading the file.\nQuitting the application...");
            Environment.Exit(0);
        }

        // keep showing the menu until the user exits
        Console.Clear();
        while (true)
        {
            // show the menu
            Console.WriteLine("\nEnter an option:\n");
            Console.WriteLine("SHOW [--keywords <keyword> | --time <epoch time>]");
            Console.WriteLine("SELECT <ID>");
            Console.WriteLine("BACK");
            Console.WriteLine("SET");
            Console.WriteLine("EXIT \n");

            // read the user input
            string? option = Console.ReadLine();
            option = String.IsNullOrWhiteSpace(option) ? "" : option;

            // TODO SHOW: show recent or trending news
            if (Regex.Match(option, @"^SHOW\s(recent|trending)(\s--keywords\s\w+)*(\s--time\s\d{10})?$",
                    RegexOptions.IgnoreCase).Success)
            {
                Console.Clear();
                if (Regex.IsMatch(option, @"^SHOW recent.*"))
                {
                    if (Regex.IsMatch(option, @"--time"))
                    {
                        // TODO filter by time
                        if (Regex.IsMatch(option, @"--keywords"))
                        {
                            // TODO filter by time and keywords
                        }
                        else
                        {
                            // TODO filter by time only
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(option, @"--keywords"))
                        {
                            // TODO filter by keywords only
                        }
                        else
                        {
                            // TODO show recent without any filter
                        }
                    }
                }
                else if (Regex.IsMatch(option, @"^SHOW trending", RegexOptions.IgnoreCase))
                {
                    // TODO show trending without filters
                }
                else if (Regex.IsMatch(option, @"^SHOW recent", RegexOptions.IgnoreCase))
                {
                    // TODO show recent without filters
                }
            }

            // TODO SELECT: show selected news item by id
            else if (Regex.Match(option, @"^SELECT\s\d+$", RegexOptions.IgnoreCase).Success)
            {
                Console.Clear();
                int id = Convert.ToInt32(option.Split(" ")[1]);
                var result = news.FirstOrDefault(n => n.Id == id);
                if (result != null)
                {
                    // TODO increase the hits counter for the selected news item
                    recents.Push(result);
                    Console.WriteLine(result);
                }
                else
                    Console.WriteLine("News item not found");
            }

            // TODO BACK: show previous read news item in the stack
            else if (Regex.Match(option, @"^BACK$", RegexOptions.IgnoreCase).Success)
            {
                Console.Clear();
                try
                {
                    recents.TryPop(out NewsItem? last);
                    NewsItem lastRead = recents.Peek();
                    // TODO decrease the hits counter for the last read news item
                    Console.WriteLine(lastRead.ToString());
                }
                catch{
                    Console.WriteLine("No more news to show");
                }
            }

            // SET: set the time to an arbitrary one for debugging purposes
            else if (Regex.Match(option, @"^SET\s\d{10}$", RegexOptions.IgnoreCase).Success)
            {
                Console.Clear();
                string? input = option.Substring(option.Length - 10);
                Console.WriteLine($"Setting time from {epoch} to {input}.\n");
                epoch = long.Parse(input);
            }

            // EXIT: exit the program
            else if (Regex.Match(option, @"^EXIT", RegexOptions.IgnoreCase).Success)
            {
                Console.Clear();
                Console.WriteLine("Bye!");
                break;
            }

            // invalid option
            else 
                Console.WriteLine("Invalid option");
        }
    }
}