using DataStructureProject;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

// Fake epoch time based on the current time
long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

// create the list of news
List<NewsItem> allNews = new List<NewsItem>();
List<NewsItem> newsRecent = new List<NewsItem>();
List<NewsItem> newsTrending = new List<NewsItem>();

// create a stack to keep track of the read news items
Stack<NewsItem> recents = new Stack<NewsItem>();
        
// read json file as a stream
try
{
    const string filePath = "/Users/sergio/RiderProjects/DataStructureProject/data/MOCK_DATA.json";
    JsonSerializer serializer = new JsonSerializer();
    using FileStream fileStream = File.OpenRead(filePath);
    using StreamReader streamReader = new StreamReader(fileStream);
    using JsonTextReader jsonTextReader = new JsonTextReader(streamReader);
    while (jsonTextReader.Read())
    {
        if (jsonTextReader.TokenType != JsonToken.StartObject) continue;
        NewsItem? newsItem = serializer.Deserialize<NewsItem>(jsonTextReader);
        if (newsItem != null)
        {
            allNews.Add(newsItem);
            newsTrending.Add(newsItem);
            newsRecent.Add(newsItem);
        }
    }
} catch (FileNotFoundException e)
{
    Console.WriteLine($"File not found: {e.Message}");
    Environment.Exit(0);
} catch (IOException e)
{
    Console.WriteLine($"Error reading file: {e.Message}");
    Environment.Exit(0);
} catch (JsonException e)
{
    Console.WriteLine($"Error deserializing JSON: {e.Message}");
    Environment.Exit(0);
}

// sort the news
newsRecent.Sort((x, y) => y.Time.CompareTo(x.Time));
newsTrending.Sort((x, y) => y.Hits.CompareTo(x.Hits));

// keep showing the menu until the user exits
Console.Clear();
while (true)
{
    // show the menu
    Console.WriteLine("--------------------\n" + 
                      "Available commands (case insensitive):\n" + 
                      "> SHOW <recent|trending> [--keywords <keyword>]... [--time <epoch time>]\n" +
                      "> SELECT <id>\n" +
                      "> BACK\n" +
                      "> SET <epoch>\n" +
                      "> EXIT \n");

    // read the user input
    Console.Write("Enter your command:\n" +
                  "> ");
    string? command = Console.ReadLine();
    command = String.IsNullOrWhiteSpace(command) ? "" : command;
    Console.Clear();
    Console.WriteLine("--------------------\n    ↓ RESULTS ↓");

    // Regex patterns for each possible command
    string showRecentPattern = @"^SHOW (recent|trending)( --keywords [a-zA-Z]*)*( --time \d{10})?$";
    string selectPattern = @"^SELECT \d+$";
    string setPattern = @"^SET \d{10}$";
    string backPattern = @"^BACK$";
    string exitPattern = @"^EXIT$";

    switch (command)
    {
        case string cmd when Regex.IsMatch(cmd, showRecentPattern, RegexOptions.IgnoreCase):
            // 
            long? epochFilter = null;
            long? epochStartOfDay = null;
            long? epochEndOfDay = null;
            List<string>? keywordsFilter = null;
            List<NewsItem> newsFiltered = new List<NewsItem>();

            // extract time filter if present
            Match matchTime = Regex.Match(cmd, @"--time \d{10}", RegexOptions.IgnoreCase);
            if (matchTime.Success)
            {
                string time = matchTime.Value;
                time = time.Replace("--time ", "");
                epochFilter = Convert.ToInt64(time);
                epochStartOfDay = (long)(epochFilter / 86400 * 86400);
                epochEndOfDay = epochStartOfDay + 86399;
                cmd = cmd.Replace($" --time {time}", "");
            }
                    
            // extract keywords filter if present
            Match matchKeywords = Regex.Match(cmd, @"(SHOW (recent|trending)) (--keywords.*)", RegexOptions.IgnoreCase);
            if (matchKeywords.Success)
            {
                string keywordsString = matchKeywords.Groups[3].Value;
                keywordsString = keywordsString.Replace("--keywords ", "");
                keywordsFilter = keywordsString.Split(" ").ToList();
            }

            // extract type of news to show
            Match matchType = Regex.Match(cmd, @"^SHOW (recent|trending)", RegexOptions.IgnoreCase);
            // filter and show the news
            if (matchType.Success)
            {
                string type = matchType.Groups[1].Value;
                if (type == "recent")
                {
                    long? epochLastDay = (long)(epoch - 86400);
                    newsFiltered = newsRecent
                        .Where(obj => obj.Time > epochLastDay && obj.Time <= epoch)
                        .ToList();
                    if(keywordsFilter != null)
                        newsFiltered = newsFiltered
                            .Where(obj => obj.Keywords.Intersect(keywordsFilter).Any())
                            .ToList();
                    newsFiltered = newsFiltered.Take(10).ToList();
                    newsFiltered.ForEach(n => Console.WriteLine(n));
                }
                else if (type == "trending")
                {
                    // if there is a time filter, show the news from that day
                    if(epochFilter == null)
                        newsFiltered = newsTrending
                            .Where(obj => obj.Time <= epoch)
                            .ToList();
                    else
                        newsFiltered = newsTrending
                            .Where(obj => obj.Time >= epochStartOfDay && obj.Time <= epochEndOfDay)
                            .ToList();
                    // if there are keywords, filter the news by them
                    if(keywordsFilter != null)
                        newsFiltered = newsFiltered
                            .Where(obj => obj.Keywords.Intersect(keywordsFilter).Any())
                            .ToList();
                    // display the news
                    newsFiltered = newsFiltered.Take(10).ToList();
                    newsFiltered.ForEach(obj => Console.WriteLine(obj));
                }
            }
            break;
        case string cmd when Regex.IsMatch(cmd, selectPattern, RegexOptions.IgnoreCase):
            Console.Clear();
            int id = Convert.ToInt32(command.Split(" ")[1]);
            var result = allNews.Find(x => x.Id == id);
            if (result != null)
            {
                // TODO increase the hits counter for the selected news item
                recents.Push(result);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(result.Time);
                Console.WriteLine($"-------------------\n ↓ BREAKING NEWS ↓\n");
                Console.WriteLine($"Date:");
                Console.WriteLine($"\t{dateTimeOffset.ToString("F")}");
                Console.WriteLine($"Categories:");
                Console.WriteLine($"\t{String.Join("/", result.Keywords)}");
                Console.WriteLine("Content:");
                Console.WriteLine($"\t{result.Content}\n");
            }
            else
                Console.WriteLine("News item not found");
            break;
        case string cmd when Regex.IsMatch(cmd, backPattern, RegexOptions.IgnoreCase):
            Console.Clear();
            try
            {
                recents.TryPop(out NewsItem? last);
                NewsItem lastRead = recents.Peek();
                // TODO decrease the hits counter for the last read news item
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(lastRead.Time);
                Console.WriteLine($"-----------------\n↓ BREAKING NEWS ↓\n");
                Console.WriteLine($"Date:");
                Console.WriteLine($"\t{dateTimeOffset.ToString("yyyy MMMM dd")}");
                Console.WriteLine($"Categories:");
                Console.WriteLine($"\t{String.Join("/", lastRead.Keywords)}");
                Console.WriteLine("Content:");
                Console.WriteLine($"\t{lastRead.Content}\n");
            }
            catch{
                Console.WriteLine("--------------------");
                Console.WriteLine("No more news to show");
            }
            break;
        case string cmd when Regex.IsMatch(cmd, setPattern, RegexOptions.IgnoreCase):
            Console.Clear();
            string? input = command.Substring(command.Length - 10);
            
            DateTimeOffset currentDate = DateTimeOffset.FromUnixTimeSeconds(epoch);
            DateTimeOffset newDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(input));
            
            Console.WriteLine($"--------------------\n" +
                              $"{currentDate.ToString("yyyy MMMM dd")}\n" +
                              $"  ↓ \n" +
                              $"{newDate.ToString("yyyy MMMM dd")}.");
            epoch = long.Parse(input);
            break;
        case string cmd when Regex.IsMatch(cmd, exitPattern, RegexOptions.IgnoreCase):
            Console.Clear();
            Console.WriteLine("Bye!");
            Environment.Exit(0);
            break;
        default:
            Console.WriteLine("Invalid option");
            break;
    }
}