# Semantic Kernel with Azure AI Search Demo

## TL;DL

The idea of this repo is to quickly demonstrate two basics things:

1. How to get started with Semantic Kernel.
2. How to implement and end-to-end RAG pattern using Semantic Kernel and Azure AI Search.

## Longer version

Cover the basic of Semantic Kernel based on [this](https://learn.microsoft.com/en-us/semantic-kernel/overview/?tabs=Csharp).
More specifically:

1. [Prompts Basics](https://learn.microsoft.com/en-us/semantic-kernel/prompts/)
2. [Plugins Basics](https://learn.microsoft.com/en-us/semantic-kernel/agents/plugins/?tabs=Csharp)
3. Text chunking using Semantic Kernel.
4. Implementing RAG Pattern (end-to-end) using Azure AI Search, but not using the out-of-the-box SK integration.

Prerequisites:

1. Azure OpenAI instance.
2. Azure AI Search instance.

### Section 1 - Getting Started | Prompting

1. Uncomment [`await SKPromptBasics.Run();`] on programs.cs and explore the code.
2. Uncomment [`await SKPromptTemplating.Run();`] on programs.cs and explore the code as well.

## Section 2 - SK Plugins

Play with your first plug-in, uncomment [`await SKFirstPlugin.Run();`] on programs.cs and take the follwing steps:

1. Ask the question: What is the answer to the great question? -> You should get 42 (da).
2. Then ask the question, what is the answer to the even greater question. -> You should get a vague reply, with not an actual number as a reply.
3. Uncomment line 28 on SKFirstPlugin.cs, and ask the same question. -> You should get 24, right?

## Section 3 - SK Text Chunking & RAG

For doing the RAG pattern implementation we are going to use (some - not all) files of [Azure Documentation](https://github.com/MicrosoftDocs/azure-docs/archive/refs/heads/main.zip). The idea is to have the LLM reply questions related to Azure Documentation.
So, in this section we will do the following:

1. Read as text the md files that are zipped in azure-docs.zip (on the root of the project).
2. Chunk those documents using SK's chunking functionality.
3. Upload the chunked 'entities' on our Azure AI Search Index.
4. Implement the RAG pattern by using an SK Plugin to query those docs.

Steps:

1. First, just unzip and store somewhere on your disk the contents of the `azure-docs.zip`.
2. Uncomment [`await SKChunking.Run();`] on program.cs
3. Explore the code, as it does the following:
    1. Creates (or updates) an Azure AI Search Index.
    2. Reads the files and chunks them accordingly.
    3. Uploads chunks once every 20 files have been chunked.
    4. Since the docs are quite a lot, this can time significant time - feel free to stop it any time.
4. Start asking the bot questions about the Azure docs that have been indexed.
