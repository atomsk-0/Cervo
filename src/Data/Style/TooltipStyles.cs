using System.Drawing;
using System.Numerics;

namespace Cervo.Data.Style;

public struct TooltipStyle
{
    public Vector2 Padding;
    public Font Font;

    public uint BorderThickness;
    public uint Radius;

    public Color BackgroundColor;
    public Color BorderColor;
}