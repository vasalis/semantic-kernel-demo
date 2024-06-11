using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Azure.Core;
using Microsoft.SemanticKernel;
using SKDemo.Utils;

namespace SKDemo.Prompting
{
    public static class SKPromptBasics
    {


        public static async Task Run()
        {
            Console.WriteLine("Hello from SK Basics.");

            // Use this request:
            // I want to send an email to the marketing team celebrating their recent milestone.

            Console.WriteLine("What is your request?");
            string? lUserRequest = Console.ReadLine();

            if (!string.IsNullOrEmpty(lUserRequest))
            {
                await BasicPromptInvokation(lUserRequest);

                await PromptInvokationWithStructure(lUserRequest);

                await JsonPromptInvokation(lUserRequest);

                await HistoryAndContextPromptInvokation(lUserRequest);
            }
        }

        private static async Task BasicPromptInvokation(string aUserRequest)
        {
            // Use this request:
            // I want to send an email to the marketing team celebrating their recent milestone.

            var lKernel = SKKernel.GetKernel();

            // make prompt specific.
            string lPrompt = @$"What is the intent of this request? {aUserRequest}. 
                                You can choose between SendEmail, SendMessage, CompleteTask, CreateDocument.";

            Console.WriteLine("Simple prompt:");
            Console.WriteLine(await lKernel.InvokePromptAsync(lPrompt));
        }

        private static async Task PromptInvokationWithStructure(string aUserRequest)
        {
            var lKernel = SKKernel.GetKernel();

            // Add structure to the output with formatting
            string lPrompt = @$"Instructions: What is the intent of this request?
                    Choices: SendEmail, SendMessage, CompleteTask, CreateDocument.
                    User Input: {aUserRequest}
                    Intent: ";

            Console.WriteLine("Prompt with structure:");
            Console.WriteLine(await lKernel.InvokePromptAsync(lPrompt));
        }

        private static async Task JsonPromptInvokation(string aUserRequest)
        {
            string lPrompt = $$"""
                             ## Instructions
                             Provide the intent of the request using the following format:
         
                             ```json
                             {
                                 "intent": {intent}
                             }
                             ```
         
                             ## Choices
                             You can choose between the following intents:
         
                             ```json
                             ["SendEmail", "SendMessage", "CompleteTask", "CreateDocument"]
                             ```
         
                             ## User Input
                             The user input is:
         
                             ```json
                             {
                                 "request": "{{aUserRequest}}"
                             }
                             ```
         
                             ## Intent
                             """;

            var lKernel = SKKernel.GetKernel();

            Console.WriteLine("Prompt with JSON formating:");
            Console.WriteLine(await lKernel.InvokePromptAsync(lPrompt));
        }

        private static async Task HistoryAndContextPromptInvokation(string aUserRequest)
        {
            string lHistory = """
                 User input: I hate sending emails, no one ever reads them.
                 AI response: I'm sorry to hear that. Messages may be a better way to communicate.
                 """;

            string lPrompt = $"""
                 Instructions: What is the intent of this request?
                 If you don't know the intent, don't guess; instead respond with "Unknown".
                 Choices: SendEmail, SendMessage, CompleteTask, CreateDocument, Unknown.
         
                 User Input: Can you send a very quick approval to the marketing team?
                 Intent: SendMessage
         
                 User Input: Can you send the full update to the marketing team?
                 Intent: SendEmail
         
                 {lHistory}
                 User Input: {aUserRequest}
                 Intent: 
                 """;

            var lKernel = SKKernel.GetKernel();

            Console.WriteLine("Prompt with History and Context formating:");
            Console.WriteLine(await lKernel.InvokePromptAsync(lPrompt));
        }

    }
}
