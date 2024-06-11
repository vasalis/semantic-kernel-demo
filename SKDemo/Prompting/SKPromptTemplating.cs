using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SKDemo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKDemo.Prompting
{
    public static class SKPromptTemplating
    {
        public static async Task Run()
        {
            Console.WriteLine("Hello from SK Prompt Templating.");

            await DoVariablesInThePrompt();
        }

        private static async Task DoVariablesInThePrompt()
        {
            var lKernel = SKKernel.GetKernel();

            // Adding variables to the prompt
            var lChat = lKernel.CreateFunctionFromPrompt(
                @"{{$history}}
                User: {{$request}}
                Assistant: ");

            ChatHistory history = [];

            // Start the chat loop
            while (true)
            {
                // Get user input
                Console.Write("User > ");
                var lRequest = Console.ReadLine();

                // Get chat response
                var chatResult = lKernel.InvokeStreamingAsync<StreamingChatMessageContent>(
                    lChat,
                    new()
                    {
                        { "request", lRequest },
                        { "history", string.Join("\n", history.Select(x => x.Role + ": " + x.Content)) }
                    }
                );

                // Stream the response
                string message = "";
                await foreach (var chunk in chatResult)
                {
                    if (chunk.Role.HasValue)
                    {
                        Console.Write(chunk.Role + " > ");
                    }

                    message += chunk;
                    Console.Write(chunk);
                }
                Console.WriteLine();

                // Append to history
                history.AddUserMessage(lRequest!);
                history.AddAssistantMessage(message);
            }
        }
    }
}
