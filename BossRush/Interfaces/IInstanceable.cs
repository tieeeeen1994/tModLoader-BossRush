namespace BossRush.Interfaces;

public interface IInstanceable<T>
{
    static abstract T Instance { get; }
    static abstract T I { get; }
}
