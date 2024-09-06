using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SKDemo.Models;
using SKDemo.Utils;
using System.ComponentModel;
using System.Text;

namespace SKDemo.Plugins
{
    public class RAGPlugin
    {
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

        [KernelFunction, Description("Get text related to tokens. When the user asks for a text with specific tokens call this function.")]
        public string GetTextWithSpecificTokens([Description("The max tokens for the text to be.")] int aMaxTokens)
        {
            // Read text files from the azure-docs folder.
            // Stop when max token limit is reached.

            StringBuilder lExit = new StringBuilder();

            //TODO: Update this value with your own.
            string lDocsFolderPath = "C:\\01_SourceCode\\SemanticKernel\\azure-docs";

            int lCurrentTokenCount = 0;

            foreach (var lFile in Directory.GetFiles(lDocsFolderPath))
            {
                var lLines = File.ReadLines(lFile);

                foreach (var lLine in lLines)
                {
                    // Get the tokens.
                    int lTokens = Utils.Utils.TokenCount(lLine);

                    if (lTokens + lCurrentTokenCount <= aMaxTokens)
                    {
                        lCurrentTokenCount += lTokens;
                        lExit.AppendLine(lLine);
                    }
                    else
                    {
                        break;
                    }
                }

                if (lCurrentTokenCount >= aMaxTokens)
                {
                    break;
                }
            }

            Console.WriteLine($"****************************************************");
            Console.WriteLine($"-> Returining text of {lCurrentTokenCount} tokens.");
            Console.WriteLine($"****************************************************");

            return lExit.ToString();
        }
    }
}
