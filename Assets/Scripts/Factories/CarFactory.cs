using Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;
using Vehicle;

public class CarFactory
{
    private readonly IObjectResolver _resolver;

    [Inject]
    public CarFactory(IObjectResolver resolver)
    {
        _resolver = resolver;
    }

    public async UniTask<CarController> CreateAsync(CarConfig config, Transform spawnPoint, Quaternion rotation)
    {
        GameObject carPrefab = await Addressables.LoadAssetAsync<GameObject>(config.CarPrefabReference).ToUniTask();

        if (carPrefab == null)
        {
            return null;
        }

        var car = _resolver.Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<CarController>();
        car.InitializeCar(config);

        return car;
    }
}
