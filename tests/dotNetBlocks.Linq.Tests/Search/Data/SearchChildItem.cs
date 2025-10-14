using System;
using System.Collections.Generic;
using System.Text;


public class SearchChildItem
{
    public int Id { get; set; }
    public string? ChildField1 { get; set; }
    public string? ChildField2 { get; set; }
    public int ParentId { get; set; }
    public SearchItem? Parent { get; set; } = default;
}
