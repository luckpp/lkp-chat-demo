# lkp-chat-demo

## Overview

`lkp-chat-demo` is a .NET 10 ASP.NET Core Web API that implements chat with Retrieval-Augmented Generation (RAG) using AWS services.

The API:
- Stores chats in DynamoDB.
- Retrieves context from an Amazon Bedrock Knowledge Base.
- Uses Amazon Nova for both response generation and query rephrasing.

## Endpoints

- `GET /api/chats` -> `IEnumerable<ChatSummaryDto>`
- `POST /api/chats` -> `ChatDto`
- `GET /api/chats/{chatId}` -> `ChatDto` (`404` if missing)
- `PUT /api/chats/{chatId}` -> `ChatItemDto` (`404` if missing)
- `DELETE /api/chats/{chatId}` -> `204 No Content`


## Data Storage Structure

### DynamoDB table
- Table name: `Chats`
- Partition key: `ChatId` (string)
- Stored attributes:
  - `ChatId` (string)
  - `Name` (string)
  - `Items` (list of maps)

Each item in `Items` contains:
- `Id` (string)
- `ChatId` (string)
- `Content` (string)
- `Role` (string)

## Runtime and Dependencies

- Target framework: `.NET 10` (`net10.0`)
- ASP.NET Core Web API
- AWS SDK dependencies for:
  - DynamoDB
  - Bedrock Runtime
  - Bedrock Agent Runtime
- Lambda hosting package is included (`Amazon.Lambda.AspNetCoreServer`).

## Notes

- This repository does **not** currently implement a local document ingestion pipeline (`IIngestionService`, `IEmbeddingService`, `IVectorStore`) as application services.
- Retrieval is delegated to Bedrock Knowledge Base instead of directly managing vectors in this API.

## Reference Diagrams

### RAG Architecture

![RAG Architecture](docs/01_RAG_Architecture.png)

### Vector Store

![Vector Store](docs/02_Vector_Store.png)

### Tokenization

![Tokenization](docs/03_Tokenization.png)

### Embeddings

![Embeddings](docs/04_Embeddings.png)

This solution implements a modern RAG architecture where documents are transformed into embeddings and stored inside a vector database. User questions are converted into vectors, matched against the stored knowledge base, and the retrieved information is injected into the prompt provided to the Foundation Model. The result is a chatbot capable of providing accurate, context-aware answers based on organization-specific data.
