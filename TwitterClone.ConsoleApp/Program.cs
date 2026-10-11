using TwitterClone.Domain.Entities;

// Quick smoke test: construct a Bookmark, print its full record description.
var bookmark = new Bookmark();
Console.WriteLine(bookmark.DescribeRecord());
