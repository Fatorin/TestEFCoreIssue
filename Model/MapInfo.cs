namespace TestEFCoreIssue.Model;

public class MapInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public List<MapTag>? Tags { get; set; }
}
