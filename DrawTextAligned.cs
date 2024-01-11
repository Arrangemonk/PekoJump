
using System.Numerics;
using Raylib_cs;

namespace Peko_Jump;
using static Raylib_cs.Raylib;
public static unsafe class TextAligned
{
    public const int AlignLeft = 0;
    public const int AlignCenter = 1;
    public const int AlignRight = 2;
    public static void DrawTextAligned(Font font, string text, Vector2 position, float fontsize, float spacing, int alignment, Color tint)
    {
        if (alignment == AlignLeft)
        {
            DrawTextEx(font, text, position, fontsize, spacing, tint);
            return;
        }

        text = text.Replace("\r", string.Empty);
        var lines = text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            var dimension = MeasureTextEx(font, lines[i], fontsize, spacing);

            Vector2 currentPosition = alignment == AlignCenter
                ? new Vector2(position.X - dimension.X * .5f, position.Y + (dimension.Y + spacing) * i)
                : new Vector2(position.X - dimension.X, position.Y + (dimension.Y + spacing) * i);

            DrawTextEx(font, lines[i], currentPosition, fontsize, spacing, tint);
        }
    }
}