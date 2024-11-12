using System.Drawing;
using System.Numerics;
using Cervo.Type.Enum;

namespace Cervo.Data.Style;

public struct NormalPanelStyle
{
    public Vector2 Padding { get; set; }
    public Display Display { get; set; }

    public Color BackgroundColor { get; set; }
    public Color BorderColor { get; set; }

    public uint BorderThickness;
    public uint Radius;
}