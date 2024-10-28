using Cervo.Type.Interface;

namespace Cervo;

public class Dom
{
    private readonly List<Element> elements = [];
    public void AddElement(Element element)
    {
        if (element.GetId() != null)
        {
            if (elements.Any(c => c.GetId() == element.GetId()))
            {
                throw new ArgumentException("Element with the same id already exists");
            }
        }
        elements.Add(element);
    }

    public Element? GetElement(string id)
    {
        return elements.FirstOrDefault(c => c.GetId() == id);
    }

    public void Render()
    {
        foreach (var element in elements)
        {
            element.Render();
        }
    }
}