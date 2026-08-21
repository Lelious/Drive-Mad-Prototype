using System;

public interface IEventBus
{
    public void Push<T>(T signal) where T : struct;
    public void Subscribe<T>(Action<T> action) where T : struct;
    public void Unsubscribe<T>(Action<T> action) where T : struct;
}
