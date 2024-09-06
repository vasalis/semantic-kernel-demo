// See https://aka.ms/new-console-template for more information
using SKDemo.Chunking;
using SKDemo.Prompting;
using SKDemo.RAG;

Console.WriteLine("Hello, SK ;)");

// SK Prompts basics.
// await SKPromptBasics.Run();

// SK Prompt templating.
// await SKPromptTemplating.Run();

// SK First Plugin.
// await SKFirstPlugin.Run();

// await SKChunking.Run();

await SKRAG.Run();

Console.WriteLine("Bye...");
Console.ReadLine();