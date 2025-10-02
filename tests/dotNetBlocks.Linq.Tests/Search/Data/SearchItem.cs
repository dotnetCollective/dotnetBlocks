using System;
using System.Collections.Generic;
using System.Text;



public class SearchItem
{
    public int Id { get; set; }
    public string? Field1 { get; set; }
    public string? Field2 { get; set; }
    public SearchChildItem? Child { get; set; }
}
