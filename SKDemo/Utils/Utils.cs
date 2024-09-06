using Azure.AI.OpenAI;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKDemo.Utils
{
    public static class Utils
    {
        private static SharpToken.GptEncoding tokenizer = SharpToken.GptEncoding.GetEncoding("cl100k_base");

        public static ReadOnlyMemory<float> GetEmbeddings(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    Uri endpoint = new Uri(MyConfig.AzureOpenAIEndPoint);
                    AzureKeyCredential credential = new AzureKeyCredential(MyConfig.AzureOpenAIKey);

                    OpenAIClient openAIClient = new OpenAIClient(endpoint, credential);
                    EmbeddingsOptions embeddingsOptions = new("text-embedding-ada-002", new string[] { input });

                    Embeddings embeddings = openAIClient.GetEmbeddings(embeddingsOptions);

                    return embeddings.Data[0].Embedding;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get embeddings for message: {input} -> Reason: {ex.Message}");
            }

            return null;
        }

        public static int TokenCount(string text)
        {
            var tokens = tokenizer.Encode(text);
            return tokens.Count;
        }
    }
}
