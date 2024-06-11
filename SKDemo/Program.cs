// See https://aka.ms/new-console-template for more information
using SKDemo.Chunking;
using SKDemo.Prompting;
using SKDemo.RAG;

Console.WriteLine("Hello, SK ;)");

//await SKPromptBasics.Run();

//await SKPromptTemplating.Run();

// await SKChunking.Run();

await SKRAG.Run();

Console.WriteLine("Bye...");
Console.ReadLine();