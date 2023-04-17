namespace DataStructureProject;

public class NewsItem : IComparable<NewsItem>
{
    public int Id { get; set; }
    public long Time { get; set; }
    public string[] Keywords { get; set; }
    public string Content { get; set; }
    public int Hits { get; set; }
    
    public NewsItem(int id, long time, string[] keywords, string content, int hits)
    {
        Id = id;
        Time = time;
        Keywords = keywords;
        Content = content;
        Hits = hits;
    }

    public override string ToString()
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Time);
        return $"{dateTimeOffset.ToString("yyyy MMMM dd"),10} | " +
               $"{Hits.ToString(),2} hits | " +
               $"id {Id.ToString(),2} | " +
               $"{Content[..20]}... | " +
               $"{String.Join(" / ", Keywords)} |";
    }
    
    public int CompareTo(NewsItem? that)
    {
        return that != null ? this.Time.CompareTo(that.Time) : 1;
    }
}