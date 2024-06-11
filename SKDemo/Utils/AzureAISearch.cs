using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using SKDemo.Models;
using System.Net;

namespace SKDemo.Utils
{
    public static class AzureAISearch
    {
        private static string mIndexName = "azure-docs-index";

        public static async Task CreateIndex()
        {
            string lVectorSearchProfileName = "my-vector-profile";
            string lVectorSearchHnswConfig = "my-hsnw-vector-config";


            int lModelDimensions = 1536;

            SearchIndex searchIndex = new(mIndexName)
            {
                Fields =
                {
                    new SimpleField("Id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true},
                    new SearchableField("Title") { IsFilterable = true, IsSortable = true},
                    new SearchableField("Text"),
                    new VectorSearchField("TextVector", lModelDimensions, lVectorSearchProfileName),
                },
                VectorSearch = new()
                {
                    Profiles =
                    {
                        new VectorSearchProfile(lVectorSearchProfileName, lVectorSearchHnswConfig)
                    },
                    Algorithms =
                    {
                        new HnswAlgorithmConfiguration(lVectorSearchHnswConfig)
                    }
                },
                SemanticSearch = new()
                {
                    Configurations =
                    {
                        new SemanticConfiguration("my-semantic-config", new()
                        {
                            TitleField = new SemanticField("Title"),
                            ContentFields =
                            {
                                new SemanticField("Text")
                            },
                            KeywordsFields =
                            {
                                new SemanticField("Title"),
                                new SemanticField("Text"),
                            }
                        })
                    }
                }
            };

            SearchIndexClient lIndexClient = new(new Uri(MyConfig.AzureAISearchEndPoint), new AzureKeyCredential(MyConfig.AzureAISearchKey));
            await lIndexClient.CreateOrUpdateIndexAsync(searchIndex);
        }

        public static void InsertChunksToIndex(List<AzureDocEntity> aChunks)
        {
            try
            {
                Console.WriteLine("About to insert chunks into Index...");

                SearchClient lSearchClient = new(new Uri(MyConfig.AzureAISearchEndPoint), mIndexName, new AzureKeyCredential(MyConfig.AzureAISearchKey));

                var lReply = lSearchClient.IndexDocuments(IndexDocumentsBatch.Upload(aChunks));

                Console.WriteLine($"Indexing - Successful documents: {lReply.Value.Results.Where(p => p.Succeeded).Count()}");
                Console.WriteLine($"Indexing - Failed documents: {lReply.Value.Results.Where(p => !p.Succeeded).Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: {ex.Message}");
            }
        }

        public static async Task<List<AzureDocEntity>> DoKeywordSearchAsync(string aKeywords)
        {
            List<AzureDocEntity> lExit = new List<AzureDocEntity>();
            try
            {
                var lSearchOptions = new SearchOptions();
                lSearchOptions.Size = 10;

                var lSearchIndex = new SearchClient(new Uri(MyConfig.AzureAISearchEndPoint), mIndexName, new AzureKeyCredential(MyConfig.AzureAISearchKey));

                SearchResults<AzureDocEntity> lResponse = await lSearchIndex.SearchAsync<AzureDocEntity>(aKeywords, lSearchOptions).ConfigureAwait(false);

                await foreach (SearchResult<AzureDocEntity> result in lResponse.GetResultsAsync())
                {
                    lExit.Add(result.Document);
                }
            }
            catch (Exception ex)
            {                
                Console.WriteLine($"DoKeywordSearchAsync Failed, details: {ex.Message}");
            }

            return lExit;
        }

        public static async Task<List<AzureDocEntity>> DoEmbeddingsSearchAsync(string aQuery)
        {
            List<AzureDocEntity> lExit = new List<AzureDocEntity>();
            try
            {
                var lSearchIndex = new SearchClient(new Uri(MyConfig.AzureAISearchEndPoint), mIndexName, new AzureKeyCredential(MyConfig.AzureAISearchKey));

                ReadOnlyMemory<float> lVectorizedResult = Utils.GetEmbeddings(aQuery);

                SearchResults<AzureDocEntity> lResponse = await lSearchIndex.SearchAsync<AzureDocEntity>(
                    new SearchOptions
                    {
                        VectorSearch = new()
                        {
                            Queries = { new VectorizedQuery(lVectorizedResult) { KNearestNeighborsCount = 3, Fields = { "TextVector" } } }
                        }                        
                    }).ConfigureAwait(false);

                await foreach (SearchResult<AzureDocEntity> result in lResponse.GetResultsAsync())
                {

                    lExit.Add(result.Document);
                }
            }
            catch (Exception ex)
            {                
                Console.WriteLine($"DoEmbeddingSearchOnChunkedAsync Failed, details: {ex.Message}");
            }

            return lExit;
        }
    }
}
