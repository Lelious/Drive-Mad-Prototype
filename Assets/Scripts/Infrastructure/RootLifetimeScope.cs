using Services;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SceneLoaderService>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

        builder.RegisterEntryPoint<Bootstrapper>();
    }
}
