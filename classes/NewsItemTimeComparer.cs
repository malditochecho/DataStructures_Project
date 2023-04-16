namespace DataStructureProject;

public class NewsItemTimeComparer : IComparer<long>
{
    // the method Compare must be implemented by the Movie class because it implements the IComparer interface
    public int Compare(long x, long y)
    {
        // this comparison, on the PriorityQueue, will return the items in descending order
        return y.CompareTo(x);
    }
}