using Microsoft.Xna.Framework;

namespace BossRush.Types;

public struct Message(string text, bool literal, Color? color = null)
{
    public string Text { get; private set; } = text;
    public Color Color { get; private set; } = color ?? Color.White;
    public bool Literal { get; private set; } = literal;

    public readonly void Display()
    {
        Util.NewText(Text, Color, Literal);
    }
}
