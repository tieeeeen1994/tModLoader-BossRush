using Microsoft.Xna.Framework;

namespace BossRushAPI.Types;

public struct Message(string text, bool literal, Color? color = null)
{
    public string Text { get; private set; } = text;
    public Color Color { get; private set; } = color ?? Color.White;
    public bool Literal { get; private set; } = literal;

    public static Message Vanilla(string key) => new(key, false, new(175, 75, 255));

    public readonly void Display()
    {
        Util.NewText(Text, Color, Literal);
    }
}
