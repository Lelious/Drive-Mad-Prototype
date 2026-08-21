using Services;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class Bootstrapper : IAsyncStartable
{
    private readonly SceneLoaderService _sceneLoader;

    [Inject]
    public Bootstrapper(SceneLoaderService sceneLoader)
    { 
        _sceneLoader = sceneLoader;
    }

    public async Awaitable StartAsync(CancellationToken cancellation = default)
    {
        await _sceneLoader.LoadScene(AssetPath.GameScene);
        await _sceneLoader.SwitchScenes();
    }
}
