using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKDemo.Utils;
using SKDemo.Plugins;

namespace SKDemo.RAG
{
    public static class SKFirstPlugin
    {
        public static async Task Run()
        {
            Console.WriteLine("Hello from SK First Plugin.");

            // Create the kernel
            var builder = Kernel.CreateBuilder();            

            builder.AddAzureOpenAIChatCompletion(
                        MyConfig.AzureOpenAIDeploymentId,
                        MyConfig.AzureOpenAIEndPoint,
                        MyConfig.AzureOpenAIKey);

            //builder.Plugins.AddFromType<FirstPlugin>();

            var lKernel = builder.Build();

            IChatCompletionService chatCompletionService = lKernel.GetRequiredService<IChatCompletionService>();

            // Create the chat history
            ChatHistory chatMessages = new ChatHistory("""
                You are a friendly assistant who likes to follow the rules. You will complete required steps
                and request approval before taking any consequential actions. If the user doesn't provide
                enough information for you to complete a task, you will keep asking questions until you have
                enough information to complete the task.                
                """);

            // Start the conversation
            while (true)
            {
                // Get user input
                System.Console.Write("User > ");
                chatMessages.AddUserMessage(Console.ReadLine()!);

                // Get the chat completions
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                    chatMessages,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: lKernel);

                // Stream the results
                string fullMessage = "";
                await foreach (var content in result)
                {
                    if (content.Role.HasValue)
                    {
                        System.Console.Write("Assistant > ");
                    }
                    System.Console.Write(content.Content);
                    fullMessage += content.Content;
                }
                System.Console.WriteLine();

                // Add the message from the agent to the chat history
                chatMessages.AddAssistantMessage(fullMessage);
            }
        }
    }
}
