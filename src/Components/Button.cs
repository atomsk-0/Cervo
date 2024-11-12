using System.Drawing;
using System.Numerics;
using Cervo.Data.Style;
using Cervo.Extensions;
using Cervo.Type.Enum;
using Cervo.Util;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;

namespace Cervo.Components;

public static unsafe class Button
{
    private static readonly Dictionary<uint, float> fade_timers = [];

    public static bool Normal(string id, string label, in NormalButtonStyle style, bool disabled = false, Vector2 size = default)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return false;

        ImGuiContext* g = *ImGuiInternal.GImGui;
        uint uId = window->GetID(id);
        Vector2 position = window->DC.CursorPos;

        ImGui.PushFont(style.Font.GetImFont());
        Vector2 labelSize = ImGui.CalcTextSize(label);

        Vector2 buttonSize = new Vector2(labelSize.X + style.Padding.Left + style.Padding.Right, labelSize.Y + style.Padding.Top + style.Padding.Bottom);

        if (style.Display == Display.Flex)
        {
            buttonSize.X = window->Size.X - window->WindowPadding.X * 2;
        }

        if (size != default) buttonSize = size;
        ImRect rect = new ImRect(position, position + buttonSize);

        ImGuiInternal.ItemSize(size, 0);
        if (ImGuiInternal.ItemAdd(rect, uId) == false)
        {
            ImGui.PopFont();
            return false;
        }

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, uId, &hovered, &held);

        if (disabled)
        {
            hovered = hovered = pressed = false;
        }

        if (style.Display != Display.Hidden)
        {
            Color backgroundColor;
            Color textColor;
            Color borderColor;

            if (disabled)
            {
                backgroundColor = style.BackgroundDisabledColor;
                textColor = style.TextDisabledColor;
                borderColor = style.BorderDisabledColor;
            }
            else
            {
                if (hovered)
                {
                    fade_timers.TryAdd(uId, 0);
                    fade_timers[uId] += g->IO.DeltaTime * style.FadeinSpeed / 100f;
                    fade_timers[uId] = Math.Min(fade_timers[uId], 1);
                }
                else
                {
                    if (fade_timers.ContainsKey(uId))
                    {
                        fade_timers[uId] -= g->IO.DeltaTime * style.FadeoutSpeed / 100f;
                        fade_timers[uId] = Math.Max(fade_timers[uId], 0);
                    }
                }

                if (!held)
                {
                    float lerpAmount = fade_timers.TryGetValue(uId, out float timer) ? timer : 0f;
                    backgroundColor = ColorExtensions.Lerp(style.BackgroundColor, style.BackgroundHoverColor, lerpAmount);
                    textColor = ColorExtensions.Lerp(style.TextColor, style.TextHoverColor, lerpAmount);
                    borderColor = ColorExtensions.Lerp(style.BorderColor, style.BorderHoverColor, lerpAmount);
                }
                else
                {
                    backgroundColor = style.BackgroundActiveColor;
                    textColor = style.TextActiveColor;
                    borderColor = style.BorderActiveColor;
                }
            }

            window->DrawList->AddRectFilled(rect.Min, rect.Max, backgroundColor.ToUint32(), style.Radius);
            if (style.BorderThickness > 0)
            {
                window->DrawList->AddRect(rect.Min, rect.Max, borderColor.ToUint32(), style.Radius, 0, style.BorderThickness);
            }

            Vector2 textPos = Vector2.Zero;
            switch (style.TextAlign)
            {
                case TextAlign.Left:
                    textPos = new Vector2(rect.Min.X + style.Padding.Left, rect.Min.Y + style.Padding.Top);
                    break;
                case TextAlign.Center:
                    textPos = new Vector2(rect.Min.X + (buttonSize.X - labelSize.X) * 0.5f, rect.Min.Y + style.Padding.Top);
                    break;
                case TextAlign.Right:
                    textPos = new Vector2(rect.Max.X - style.Padding.Right - labelSize.X, rect.Min.Y + style.Padding.Top);
                    break;
            }

            window->DrawList->AddText(textPos, textColor.ToUint32(), label);
        }


        ImGui.PopFont();

        return pressed;
    }


    public static bool Icon(string id, string icon, in IconButtonStyle style, bool disabled = false, Vector2 size = default)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return false;

        ImGuiContext* g = *ImGuiInternal.GImGui;
        uint uId = window->GetID(id);
        Vector2 position = window->DC.CursorPos;

        ImGui.PushFont(style.Font.GetImFont());
        Vector2 labelSize = ImGui.CalcTextSize(icon);

        Vector2 buttonSize = new Vector2(labelSize.X + style.Padding.Left + style.Padding.Right, labelSize.Y + style.Padding.Top + style.Padding.Bottom);

        if (style.Display == Display.Flex)
        {
            buttonSize.X = window->Size.X - window->WindowPadding.X * 2;
        }

        if (size != default) buttonSize = size;
        ImRect rect = new ImRect(position, position + buttonSize);

        ImGuiInternal.ItemSize(size, 0);
        if (ImGuiInternal.ItemAdd(rect, uId) == false)
        {
            ImGui.PopFont();
            return false;
        }

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, uId, &hovered, &held);

        if (disabled)
        {
            hovered = hovered = pressed = false;
        }

        if (style.Display != Display.Hidden)
        {
            Color backgroundColor;
            Color textColor;
            Color borderColor;

            if (disabled)
            {
                backgroundColor = style.BackgroundDisabledColor;
                textColor = style.IconDisabledColor;
                borderColor = style.BorderDisabledColor;
            }
            else
            {
                if (hovered)
                {
                    fade_timers.TryAdd(uId, 0);
                    fade_timers[uId] += g->IO.DeltaTime * style.FadeinSpeed / 100f;
                    fade_timers[uId] = Math.Min(fade_timers[uId], 1);
                }
                else
                {
                    if (fade_timers.ContainsKey(uId))
                    {
                        fade_timers[uId] -= g->IO.DeltaTime * style.FadeoutSpeed / 100f;
                        fade_timers[uId] = Math.Max(fade_timers[uId], 0);
                    }
                }

                if (!held)
                {
                    float lerpAmount = fade_timers.TryGetValue(uId, out float timer) ? timer : 0f;
                    backgroundColor = ColorExtensions.Lerp(style.BackgroundColor, style.BackgroundHoverColor, lerpAmount);
                    textColor = ColorExtensions.Lerp(style.IconColor, style.IconHoverColor, lerpAmount);
                    borderColor = ColorExtensions.Lerp(style.BorderColor, style.BorderHoverColor, lerpAmount);
                }
                else
                {
                    backgroundColor = style.BackgroundActiveColor;
                    textColor = style.IconActiveColor;
                    borderColor = style.BorderActiveColor;
                }
            }

            window->DrawList->AddRectFilled(rect.Min, rect.Max, backgroundColor.ToUint32(), style.Radius);
            if (style.BorderThickness > 0)
            {
                window->DrawList->AddRect(rect.Min, rect.Max, borderColor.ToUint32(), style.Radius, 0, style.BorderThickness);
            }

            Vector2 textPos = new Vector2(rect.Min.X + (buttonSize.X - labelSize.X) * 0.5f, rect.Min.Y + style.Padding.Top);

            window->DrawList->AddText(textPos, textColor.ToUint32(), icon);
        }

        ImGui.PopFont();

        return pressed;
    }
}