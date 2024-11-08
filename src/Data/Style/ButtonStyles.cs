using System.Drawing;
using System.Numerics;
using Cervo.Type.Enum;
using Mochi.DearImGui;

namespace Cervo.Data.Style;

public unsafe struct NormalButtonStyle
{
    public Padding Padding;
    public ImFont* Font;
    public TextAlign TextAlign;
    public Display Display;

    public uint BorderThickness;
    public uint Radius;
    public uint FadeinSpeed;
    public uint FadeoutSpeed;

    public Color BackgroundColor;
    public Color BackgroundHoverColor;
    public Color BackgroundActiveColor;
    public Color BackgroundDisabledColor;

    public Color TextColor;
    public Color TextHoverColor;
    public Color TextActiveColor;
    public Color TextDisabledColor;

    public Color BorderColor;
    public Color BorderHoverColor;
    public Color BorderActiveColor;
    public Color BorderDisabledColor;
}