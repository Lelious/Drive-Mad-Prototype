using InputModule;
using Services;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IInputService, InputService>(Lifetime.Singleton);
        builder.Register<CarFactory>(Lifetime.Singleton);
        builder.Register<GameplayService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameInitializerService>();
    }
}
