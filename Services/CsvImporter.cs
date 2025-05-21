using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using BookPlatformMVC.Models;

namespace BookPlatformMVC.Services
{
    public class CsvImporter
    {
        public static List<Book> ImportBooks(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                // Read the first line to detect format
                var firstLine = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(firstLine))
                    throw new InvalidDataException("The CSV file is empty or unreadable.");

                // Reset the reader
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();

                // Common config since both are tab-delimited
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    MissingFieldFound = null,
                    HeaderValidated = null,
                    IgnoreBlankLines = true
                };

                using (var csv = new CsvReader(reader, config))
                {
                    if (firstLine.Contains("Exclusive Shelf")) // Goodreads
                    {
                        csv.Context.RegisterClassMap<GoodreadsBookMap>();
                        var records = csv.GetRecords<GoodreadsBook>().ToList();

                        return records.Select(r => new Book
                        {
                            Title = r.Title,
                            Author = r.Author,
                            Description = r.MyReview,
                            ISBN = r.ISBN, // <-- Set required ISBN here
                            ImagePath = "/images/default-cover.png"
                        }).ToList();
                    }
                    else if (firstLine.Contains("Read Status") && firstLine.Contains("Moods")) // StoryGraph
                    {
                        csv.Context.RegisterClassMap<StoryGraphBookMap>();
                        var records = csv.GetRecords<StoryGraphBook>().ToList();

                        return records.Select(r => new Book
                        {
                            Title = r.Title,
                            Author = r.Author,
                            Description = r.Review,
                            ISBN = r.ISBN, // <-- Set required ISBN here
                            ImagePath = "/images/default-cover.png"
                        }).ToList();
                    }
                    else
                    {
                        throw new InvalidDataException("Unsupported CSV format.");
                    }
                }
            }
        }
    }
}
