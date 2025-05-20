using CsvHelper.Configuration;

namespace BookPlatformMVC.Models
{
    public class StoryGraphBookMap : ClassMap<StoryGraphBook>
    {
        public StoryGraphBookMap()
        {
            Map(m => m.Title).Name("Title");
            Map(m => m.Author).Name("Authors");
            Map(m => m.ISBN).Name("ISBN/UID");
            Map(m => m.Format).Name("Format");
            Map(m => m.ReadStatus).Name("Read Status");
            Map(m => m.DateAdded).Name("Date Added");
            Map(m => m.LastDateRead).Name("Last Date Read");
            Map(m => m.ReadCount).Name("Read Count");
            Map(m => m.StarRating).Name("Star Rating");
            Map(m => m.Review).Name("Review");
        }
    }
}