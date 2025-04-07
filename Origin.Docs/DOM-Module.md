# DOM Module Documentation

## Overview
The DOM module implements the Document Object Model for Origin, representing the hierarchical structure of documents.

## Key Components

### 1. Element Class
- Represents HTML elements
- Properties:
  - `TagName`: The element's tag
  - `Children`: Child elements
  - `Attributes`: Element attributes
  - `TextContent`: Text content

### 2. Document Class
- Root of the DOM tree
- Manages the entire document structure
- Provides document-level operations

### 3. Node Types
- Element nodes
- Text nodes
- Comment nodes (future support)

## Usage Example
```csharp
var doc = new Document();
var div = new Element("div");
div.TextContent = "Hello World";
doc.RootElement = div;
