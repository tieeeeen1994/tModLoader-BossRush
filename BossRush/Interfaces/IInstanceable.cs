namespace BossRush.Interfaces;

internal interface IInstanceable<T>
{
    static abstract T Instance { get; }
    static abstract T I { get; }
}
