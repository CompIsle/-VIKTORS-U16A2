using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = @"C:\RM Desktop\Unit16-As2\IndexRef\IndexRef\input.csv";
            string outputFilePath = @"C:\RM Desktop\Unit16-As2\IndexRef\IndexRef\output.csv";

            try
            {
                // Read the books from the input CSV file
                List<Book> books = ReadBooksFromCSV(inputFilePath);

                // Generate IDs for the books
                List<BookWithID> booksWithID = GenerateIDs(books);

                // Write the books with IDs to the output CSV file
                WriteBooksToCSV(booksWithID, outputFilePath);

                Console.WriteLine("Data processed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while processing the data:");
                Console.WriteLine(ex.Message);
            }
        }

        static List<Book> ReadBooksFromCSV(string filePath)
        {
            List<Book> books = new List<Book>();

            try
            {
                // Read the CSV file using CsvHelper
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, GetCsvConfiguration()))
                {
                    // Register the class map for mapping CSV columns to Book properties
                    csv.Context.RegisterClassMap<BookMap>();
                    // Read the records from the CSV file and convert them to Book objects
                    books = csv.GetRecords<Book>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading the CSV file.", ex);
            }

            return books;
        }

        static List<BookWithID> GenerateIDs(List<Book> books)
        {
            List<BookWithID> booksWithID = new List<BookWithID>();
            Dictionary<Book, string> idMap = new Dictionary<Book, string>();

            foreach (Book book in books)
            {
                // Generate a unique ID for each book
                if (!idMap.ContainsKey(book))
                {
                    int idCounter = idMap.Count + 1;
                    string id = $"XO{idCounter.ToString().PadLeft(5, '0')}";
                    idMap.Add(book, id);
                }

                // Retrieve the existing ID for the book (in case of duplicates)
                string existingId = idMap[book];

                // Create a BookWithID object with the ID and other book properties
                booksWithID.Add(new BookWithID
                {
                    ID = existingId,
                    Name = book.Name,
                    Title = book.Title,
                    PlaceOfPublication = book.PlaceOfPublication,
                    Publisher = book.Publisher,
                    PublicationDate = book.PublicationDate
                });
            }

            // Sort the booksWithID list based on the numerical part of the ID
            booksWithID = booksWithID.OrderBy(b => int.Parse(b.ID.Substring(2))).ToList();

            return booksWithID;
        }

        static void WriteBooksToCSV(List<BookWithID> books, string filePath)
        {
            try
            {
                // Write the books with IDs to the output CSV file using CsvHelper
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, GetCsvConfiguration()))
                {
                    // Register the class map for mapping BookWithID properties to CSV columns
                    csv.Context.RegisterClassMap<BookWithIDMap>();
                    // Write the header row
                    csv.WriteHeader<BookWithID>();
                    csv.NextRecord();
                    // Write the records to the CSV file
                    csv.WriteRecords(books);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing to the CSV file.", ex);
            }
        }

        static CsvConfiguration GetCsvConfiguration()
        {
            // Create and configure the CsvConfiguration
            CsvConfiguration configuration = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            configuration.HeaderValidated = null;
            configuration.MissingFieldFound = null;
            return configuration;
        }
    }

    // Define the Book record with properties for each field in the CSV
    public record Book
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string PlaceOfPublication { get; set; }
        public string Publisher { get; set; }
        public string PublicationDate { get; set; }
    }

    // Define the class map for mapping Book properties to CSV columns
    public sealed class BookMap : ClassMap<Book>
    {
        public BookMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Title).Name("Title");
            Map(m => m.PlaceOfPublication).Name("Place publication");
            Map(m => m.Publisher).Name("Publisher");
            Map(m => m.PublicationDate).Name("Date of publication");
        }
    }

    // Define the BookWithID record that inherits from Book and adds the ID property
    public record BookWithID : Book
    {
        public string ID { get; set; }
    }

    // Define the class map for mapping BookWithID properties to CSV columns
    public sealed class BookWithIDMap : ClassMap<BookWithID>
    {
        public BookWithIDMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.Name).Name("Name");
            Map(m => m.Title).Name("Title");
            Map(m => m.PlaceOfPublication).Name("Place publication");
            Map(m => m.Publisher).Name("Publisher");
            Map(m => m.PublicationDate).Name("Date of publication");
        }
    }
}
