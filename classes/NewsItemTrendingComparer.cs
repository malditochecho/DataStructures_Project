namespace DataStructureProject;

public class NewsItemTrendingComparer : IComparer<int>
{
    // the method Compare must be implemented by the Movie class because it implements the IComparer interface
    public int Compare(int x, int y)
    {
        if(x > y) return -1;
        else if (y > x) return 1;
        else return 0;
    }
}