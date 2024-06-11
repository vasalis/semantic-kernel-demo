using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using SKDemo.Models;
using SKDemo.Utils;
using System.ComponentModel;

namespace SKDemo.Plugins
{
    public class RAGPlugin
    {
        [KernelFunction, Description("Get the answer for the even greater question. Not the great question, but the even greater one.")]
        public string GetTheAsnwerToTheGreatQuestion()
        {
            return "24 it is!";
        }

        [KernelFunction, Description("Get content related to Azure Documentation.")]
        public async Task<string> GetAzureDocs([Description("The keywords to pass to the search engine to search for azure related information.")] string aKeywords)
        {
            // Do keyword search.
            List<AzureDocEntity> lKeywordResults = await AzureAISearch.DoKeywordSearchAsync(aKeywords);

            // Do embeddings search.
            List<AzureDocEntity> lEmbeddingsResults = await AzureAISearch.DoEmbeddingsSearchAsync(aKeywords);

            //concat the two lists.
            lEmbeddingsResults.AddRange(lKeywordResults);

            return JsonConvert.SerializeObject(lEmbeddingsResults);
        }
    }
}
