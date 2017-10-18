using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure_Search
{
    using System.Threading;

    using Microsoft.Azure.Search;
    using Microsoft.Azure.Search.Models;

    //https://github.com/Azure-Samples/search-dotnet-getting-started/tree/master/DotNetHowTo
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            SearchServiceClient serviceClient = CreateSearchServiceClient(configuration);

            string indexName = "catgang";

            Console.WriteLine("{0}", "Deleting index...\n");
            DeleteIndexIfExists(serviceClient, indexName);

            Console.WriteLine("Creating index...");
            CreateIndex<Cat>(serviceClient, indexName);

            var indexClient = GetSearchIndexClient(serviceClient, indexName);
            UploadDocuments<Cat>(indexClient, Samples.GenerateSampleCats());

            ISearchIndexClient indexClientForQueries = CreateSearchIndexClient(configuration, indexName);

            RunQueries<Cat>(indexClientForQueries);

            SeeMe();
        }

        private static void RunQueries<T>(ISearchIndexClient indexClient) where T: class
        {
            SearchParameters parameters;
            DocumentSearchResult<T> results;

            Console.WriteLine("Search the entire index for the term 'lazy' and return only the name field:\n");

            parameters =
                new SearchParameters()
                    {
                        Select = new[] { "name" }
                    };

            results = indexClient.Documents.Search<T>("lazy", parameters);

            WriteDocuments(results);

            Console.Write("Apply a filter to find Blackie cat, ");
            Console.WriteLine("and return the name and description:\n");

            parameters =
                new SearchParameters()
                {
                    Filter = "name eq 'Blackie'",
                    Select = new[] { "name", "description" }
                };

            results = indexClient.Documents.Search<T>("*", parameters);

            WriteDocuments(results);

            Console.Write("Search the entire index, order by a specific field (name) ");
            Console.Write("in descending order, take the top two results, and show only name and ");
            Console.WriteLine("id:\n");

            parameters =
                new SearchParameters()
                {
                    OrderBy = new[] { "name desc" },
                    Select = new[] { "name", "id" },
                    Top = 2
                };

            results = indexClient.Documents.Search<T>("*", parameters);

            WriteDocuments(results);

            Console.WriteLine("Search the entire index for the term 'White':\n");

            parameters = new SearchParameters();
            results = indexClient.Documents.Search<T>("White", parameters);

            WriteDocuments(results);
        }


        private static void WriteDocuments<T>(DocumentSearchResult<T> searchResults) where T: class
        {
            foreach (SearchResult<T> result in searchResults.Results)
            {
                Console.WriteLine(result.Document);
            }

            Console.WriteLine();
        }

        private static ISearchIndexClient CreateSearchIndexClient(IConfigurationRoot configuration, string indexName)
        {
            string searchServiceName = configuration["SearchServiceName"];
            string queryApiKey = configuration["SearchServiceQueryApiKey"];

            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));
            return indexClient;
        }

        private static void UploadDocuments<T>(ISearchIndexClient indexClient, List<T> documents)
            where T : class
        {
            var batch = IndexBatch.Upload(documents);

            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }

            Console.WriteLine("Waiting for documents to be indexed...\n");
            Thread.Sleep(2000);
        }

        private static ISearchIndexClient GetSearchIndexClient(SearchServiceClient serviceClient, string indexName)
        {
            return serviceClient.Indexes.GetClient(indexName);
        }
        private static void SeeMe()
        {
            Console.ReadLine();
        }

        private static void CreateIndex<ModelType>(SearchServiceClient serviceClient, string indexName)
        {
            if (serviceClient.Indexes.Exists(indexName)) return;

            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<ModelType>()
            };

            serviceClient.Indexes.Create(definition);
        }

        private static void DeleteIndexIfExists(SearchServiceClient serviceClient, string indexName)
        {
            if (serviceClient.Indexes.Exists(indexName))
            {
                serviceClient.Indexes.Delete(indexName);
            }
        }

        private static SearchServiceClient CreateSearchServiceClient(IConfigurationRoot configuration)
        {
            string searchServiceName = configuration["SearchServiceName"];
            string adminApiKey = configuration["SearchServiceAdminApiKey"];

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
            return serviceClient;
        }
    }
}
