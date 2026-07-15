# RAG lkp-chat-demo

# C# RAG Chatbot Solution

## Overview

This project demonstrates a Retrieval-Augmented Generation (RAG) chatbot implemented in C#.

Instead of relying only on the knowledge stored inside a Large Language Model (LLM), the chatbot retrieves relevant information from a custom knowledge base before generating a response.

This approach improves:
- Accuracy
- Traceability
- Domain-specific knowledge
- Reduced hallucinations
- Ability to answer questions about private company data

---

# What is RAG?

RAG (Retrieval-Augmented Generation) combines two capabilities:

1. **Information Retrieval**
   - Find relevant information from a knowledge base.

2. **Text Generation**
   - Use a Foundation Model (LLM) to generate a natural language response.

The LLM receives both:
- User question
- Retrieved context

and generates an answer based on the retrieved information.

---

# High-Level Architecture

```text
User Question
      │
      ▼
   Retrieval
      │
      ▼
 Knowledge Base
      │
      ▼
 Relevant Documents
      │
      ▼
 Augmented Prompt
      │
      ▼
 Foundation Model
      │
      ▼
 Generated Response
```

---

# Example RAG Flow

### User Query

```text
Who is the Product Manager for John?
```

### Retrieved Context

```text
Support Contacts

Product Manager: Jessie Smith
Engineer: Sara Ronald
```

### Augmented Prompt

```text
Question:
Who is the Product Manager for John?

Context:
Product Manager: Jessie Smith
Engineer: Sara Ronald
```

### Generated Response

```text
Jessie Smith is the Product Manager for John.
```

---

# Knowledge Ingestion Pipeline

Before the chatbot can answer questions, documents must be processed and stored.

## Step 1 - Store Documents

Documents are uploaded into a storage location such as:

- Amazon S3
- File System
- SharePoint
- Database

Example:

```text
product-manual.pdf
support-contacts.docx
release-notes.pdf
```

---

## Step 2 - Document Chunking

Large documents are split into smaller chunks.

Example:

```text
Document:

The Product Manager for John is Jessie Smith.
For technical support contact Sara Ronald.
```

Chunks:

```text
Chunk 1:
The Product Manager for John is Jessie Smith.

Chunk 2:
For technical support contact Sara Ronald.
```

Chunking improves retrieval accuracy and reduces unnecessary context sent to the LLM.

---

## Step 3 - Generate Embeddings

Each chunk is converted into a numerical vector using an embedding model.

Possible embedding providers:

- Amazon Titan Embeddings
- Cohere Embeddings
- OpenAI Embeddings

Example:

```text
"The Product Manager for John is Jessie Smith"

↓

[0.17, -0.42, 0.88, 0.11, ...]
```

These vectors capture semantic meaning.

---

# Understanding Embeddings

Embeddings transform words and sentences into high-dimensional vectors.

Words with similar meanings produce vectors that are close together.

Example:

```text
Dog   -> [0.6, 0.9, 0.1, ...]
Puppy -> [0.5, 0.8, -0.1, ...]
Cat   -> [0.7, -0.1, 0.4, ...]
```

The vectors for:

```text
Dog
Puppy
```

are closer to one another than:

```text
Dog
House
```

This allows semantic search instead of simple keyword matching.

---

# Vector Database

Generated embeddings are stored in a vector database.

Possible implementations:

- OpenSearch
- Pinecone
- MongoDB Atlas Vector Search
- Redis Vector Search
- Aurora
- Neptune Analytics
- S3 Vectors

Stored data:

```json
{
  "id": "chunk-001",
  "text": "The Product Manager for John is Jessie Smith",
  "embedding": [0.17, -0.42, 0.88, ...]
}
```

---

# Query Processing

When a user asks a question:

```text
Who is the Product Manager for John?
```

The same embedding model converts the question into a vector.

```text
Question
      ↓
Embedding Model
      ↓
Query Vector
```

---

# Semantic Search

The query vector is compared against vectors stored in the database.

Similarity algorithms:

- Cosine Similarity
- Dot Product
- Euclidean Distance

The most relevant chunks are retrieved.

Example result:

```text
1. The Product Manager for John is Jessie Smith.
2. Sara Ronald provides engineering support.
```

---

# Prompt Augmentation

The retrieved chunks are injected into the final prompt.

```text
Context:
The Product Manager for John is Jessie Smith.

Question:
Who is the Product Manager for John?
```

This process is called:

```text
Retrieval-Augmented Generation
```

---

# Response Generation

The augmented prompt is sent to the Foundation Model.

Examples:

- Claude
- Llama
- Amazon Nova
- GPT
- Mistral

The model generates:

```text
Jessie Smith is the Product Manager for John.
```

---

# C# Solution Components

## Document Ingestion Service

Responsibilities:

- Read documents
- Chunk documents
- Generate embeddings
- Store vectors

```csharp
public interface IIngestionService
{
    Task ProcessDocumentAsync(string filePath);
}
```

---

## Embedding Service

Responsibilities:

- Generate embeddings for text
- Call external embedding models

```csharp
public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}
```

---

## Vector Store

Responsibilities:

- Save embeddings
- Search similar vectors

```csharp
public interface IVectorStore
{
    Task StoreAsync(DocumentChunk chunk);

    Task<List<DocumentChunk>> SearchAsync(
        float[] embedding,
        int topK);
}
```

---

## Chat Service

Responsibilities:

- Generate query embedding
- Retrieve context
- Build prompt
- Call LLM

```csharp
public interface IChatService
{
    Task<string> AskAsync(string question);
}
```

---

# End-to-End Flow

```text
1. Upload Documents
        │
        ▼
2. Chunk Documents
        │
        ▼
3. Generate Embeddings
        │
        ▼
4. Save in Vector Database
        │
        ▼
5. User Question
        │
        ▼
6. Generate Query Embedding
        │
        ▼
7. Semantic Search
        │
        ▼
8. Retrieve Relevant Chunks
        │
        ▼
9. Build Augmented Prompt
        │
        ▼
10. Send to LLM
        │
        ▼
11. Generate Response
```

---

# Benefits of RAG

✅ More accurate responses

✅ Reduced hallucinations

✅ Uses private company knowledge

✅ No model retraining required

✅ Easy document updates

✅ Source-aware responses

✅ Scalable architecture

---

# Technologies Used

## Application Layer

- .NET 8
- ASP.NET Core
- C#

## Storage

- Amazon S3

## Embeddings

- Amazon Titan
- Cohere

## Vector Database

- OpenSearch
- Pinecone
- MongoDB Atlas
- Redis

## Foundation Models

- Claude
- Amazon Nova
- Llama
- GPT

---

# Conclusion

This solution implements a modern RAG architecture where documents are transformed into embeddings and stored inside a vector database. User questions are converted into vectors, matched against the stored knowledge base, and the retrieved information is injected into the prompt provided to the Foundation Model. The result is a chatbot capable of providing accurate, context-aware answers based on organization-specific data.