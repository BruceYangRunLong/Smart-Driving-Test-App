namespace dpa.Library.Models;

[SQLite.Table("records")]
public class Record
{
    [SQLite.Column("date")] public string date { get; set; }
    [SQLite.Column("right")] public int right { get; set; }
    [SQLite.Column("wrong")] public int wrong { get; set; }
}