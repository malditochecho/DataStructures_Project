namespace DataStructureProject.classes;

public class NewsItem : IComparable
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
        return $"({Time.ToString().PadLeft(10)}) / {Hits.ToString().PadLeft(2)} hits / id: {Id.ToString().PadLeft(2)} / {Content.Substring(0, Content.Length)}...";
    }

    public int CompareTo(object? obj)
    {
        if(Hits > ((NewsItem)obj).Hits)
            return 1;
        if(Hits < ((NewsItem)obj).Hits)
            return -1;
        return 0;
    }
}