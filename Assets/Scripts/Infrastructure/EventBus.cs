using System;
using UnityEngine;

public class EventBus : IEventBus
{
    private static class EventStorage<T> where T : struct
    {
        public static Action<T> OnEvent;
    }

    public void Push<T>(T signal) where T : struct
    {
        EventStorage<T>.OnEvent?.Invoke(signal);
    }

    public void Subscribe<T>(Action<T> action) where T : struct
    {
        EventStorage<T>.OnEvent += action;
    }

    public void Unsubscribe<T>(Action<T> action) where T : struct
    {
        EventStorage<T>.OnEvent -= action;
    }
}
