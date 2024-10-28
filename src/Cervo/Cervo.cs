using Cervo.Type.Interface;

namespace Cervo;

internal static class Cervo
{
    internal static IWindow? CurrentWindow;

    private static readonly List<Element> element_render_queue = [];

    internal static void AddElement(Element element)
    {
        if (element_render_queue.Any(c => c == element)) return;
        element_render_queue.Add(element);
    }

    // Render all elements in the render queue and reset the queue
    internal static void Render()
    {
        foreach (var element in element_render_queue)
        {
            element.Render();
        }
        element_render_queue.Clear();
    }

    /*public void AddElement(Element element)
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
    }*/
}