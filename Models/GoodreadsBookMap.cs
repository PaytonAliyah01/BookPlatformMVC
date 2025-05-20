using CsvHelper.Configuration;

namespace BookPlatformMVC.Models
{
    public class GoodreadsBookMap : ClassMap<GoodreadsBook>
    {
        public GoodreadsBookMap()
        {
            Map(m => m.Id).Name("Book Id");
            Map(m => m.Title).Name("Title");
            Map(m => m.Author).Name("Author");
            Map(m => m.ISBN).Name("ISBN13");
            Map(m => m.AverageRating).Name("Average Rating");
            Map(m => m.Publisher).Name("Publisher");
            Map(m => m.NumberOfPages).Name("Number of Pages");
            Map(m => m.YearPublished).Name("Year Published");
            Map(m => m.ExclusiveShelf).Name("Exclusive Shelf");
            Map(m => m.MyReview).Name("My Review");
            Map(m => m.ReadCount).Name("Read Count");
        }
    }
}