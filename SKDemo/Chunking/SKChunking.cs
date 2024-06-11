using Azure.AI.OpenAI;
using Azure;
using SKDemo.Models;
using SKDemo.Utils;
using Microsoft.SemanticKernel.Text;

namespace SKDemo.Chunking
{
#pragma warning disable SKEXP0050 // Type or member is obsolete

    public static class SKChunking
    {
        private static string mDocsFolderPath = "C:\\01_SourceCode\\SemanticKernel\\azure-docs";
        public static async Task Run()
        {
            Console.WriteLine("Creating Search Index...");

            await AzureAISearch.CreateIndex();

            Console.WriteLine("Getting documents in chuncks with their vectors and uploading in batches...");
            CreateChunksAndUploadToIndex();
        }

        private static void CreateChunksAndUploadToIndex()
        {
            List<AzureDocEntity> lExit = new List<AzureDocEntity>();
            var lMdFiles = Directory.GetFiles(mDocsFolderPath);

            Console.WriteLine($"Found {lMdFiles.Length} files.");

            int lFileCounter = 0;
            int lTotalChunksCounter = 0;
            foreach (var lMdFile in lMdFiles)
            {
                string lFileName = Path.GetFileNameWithoutExtension(lMdFile);
                Console.WriteLine($"About to create chunks for: {lFileName}");

                // Get the text from the file.
                string lText = File.ReadAllText(lMdFile);

                // Split into chunks using SK TextChunker

                // first get lines.
                var lLines = TextChunker.SplitPlainTextLines(lText, 2000);

                // then get paragraphs
                var lParagraphs = TextChunker.SplitPlainTextParagraphs(lLines, 1000, 100);

                int lChunksCounter = 0;
                foreach (var lParagraph in lParagraphs)
                {
                    
                    AzureDocEntity lChunk = new AzureDocEntity()
                    { Id = $"{lFileCounter}_{lChunksCounter++}",
                      Title = lFileName,
                      Text = lParagraph,
                      TextVector = Utils.Utils.GetEmbeddings(lParagraph)
                    };

                    // add to the list
                    lExit.Add(lChunk);

                    lTotalChunksCounter++;
                }

                Console.WriteLine($"Created {lChunksCounter} chunks for: {lFileName}");
                lFileCounter++;    
                
                if(lFileCounter % 20 == 0)
                {
                    Console.WriteLine($"About to upload to Index...");

                    // upload to index
                    AzureAISearch.InsertChunksToIndex(lExit);
                    lExit.Clear();
                }
            }
            
            if(lExit.Count > 0) 
            {
                // upload remaining chunks to index.
                AzureAISearch.InsertChunksToIndex(lExit);
            }

            Console.WriteLine($"Uploaded in total {lTotalChunksCounter} chunks to search index.");
        }        
    }
}
