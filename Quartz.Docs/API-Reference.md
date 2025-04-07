# Origin API Reference

## Core Module

### EventSystem
```csharp
public class EventSystem
{
    /// <summary>
    /// Subscribes a handler to an event
    /// </summary>
    /// <param name="eventName">Name of the event to subscribe to</param>
    /// <param name="handler">Callback to invoke when event occurs</param>
    /// <example>
    /// <code>
    /// var eventSystem = new EventSystem();
    /// eventSystem.Subscribe("click", data => {
    ///     Console.WriteLine($"Click event: {data}");
    /// });
    /// </code>
    /// </example>
    public void Subscribe(string eventName, Action<object> handler);

    /// <summary>
    /// Publishes an event to all subscribers
    /// </summary>
    /// <param name="eventName">Name of the event to publish</param>
    /// <param name="data">Payload data to send to subscribers</param>
    public void Publish(string eventName, object data);
}
```

## DOM Module

### Document
```csharp
public class Document
{
    public Element RootElement { get; set; }
    public Element CreateElement(string tagName);
}
```

### Element
```csharp
public class Element
{
    /// <summary>
    /// Gets the HTML tag name of this element
    /// </summary>
    public string TagName { get; }

    /// <summary>
    /// Gets or sets the text content of this element
    /// </summary>
    public string TextContent { get; set; }

    /// <summary>
    /// Gets the child elements of this element
    /// </summary>
    /// <example>
    /// <code>
    /// var div = new Element("div");
    /// div.Children.Add(new Element("span"));
    /// Console.WriteLine(div.Children.Count); // Output: 1
    /// </code>
    /// </example>
    public List<Element> Children { get; }

    /// <summary>
    /// Gets the attributes of this element
    /// </summary>
    public Dictionary<string, string> Attributes { get; }
}
```

## Styling Module

### CssParser
```csharp
public class CssParser
{
    public StyleSheet Parse(string css);
}
```

### StyleContext
```csharp
public class StyleContext
{
    public void ApplyStyles(Document document, StyleSheet styleSheet);
}
```

## Rendering Module

### PageRenderer
```csharp
public class PageRenderer
{
    /// <summary>
    /// Creates a new PageRenderer
    /// </summary>
    /// <param name="paintEngine">Paint engine to use for rendering</param>
    /// <param name="layoutEngine">Layout engine to use for calculations</param>
    public PageRenderer(PaintEngine paintEngine, LayoutEngine layoutEngine);

    /// <summary>
    /// Renders a document to the output surface
    /// </summary>
    /// <param name="document">Document to render</param>
    /// <exception cref="ArgumentNullException">Thrown if document is null</exception>
    /// <example>
    /// <code>
    /// var surface = SKSurface.Create(...);
    /// var paintEngine = new PaintEngine(surface);
    /// var layoutEngine = new LayoutEngine();
    /// var renderer = new PageRenderer(paintEngine, layoutEngine);
    ///
    /// var doc = new Document();
    /// doc.RootElement = new Element("div");
    /// renderer.Render(doc);
    /// </code>
    /// </example>
    public void Render(Document document);
}
```

### PaintEngine
```csharp
public class PaintEngine
{
    public PaintEngine(SKSurface surface);
    public void Render(Element root);
}
```

## Common Types

### StyleSheet
```csharp
public class StyleSheet
{
    public List<CssRule> Rules { get; }
    public List<MediaQuery> MediaQueries { get; }
}
```

### CssRule
```csharp
public class CssRule
{
    public List<string> Selectors { get; }
    public Dictionary<string, string> Declarations { get; }
}
