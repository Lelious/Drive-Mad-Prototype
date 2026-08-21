using Configs;
using Cysharp.Threading.Tasks;
using Levels;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Services
{
    public class GameInitializerService : IAsyncStartable
    {
        private readonly IObjectResolver _resolver;
        private readonly CarFactory _carFactory;
        private readonly GameplayService _gameplayService;

        [Inject]
        public GameInitializerService(IObjectResolver resolver, CarFactory carFactory, GameplayService gameplayService)
        {
            _resolver = resolver;
            _carFactory = carFactory;
            _gameplayService = gameplayService;
        }

        public async Awaitable StartAsync(CancellationToken cancellation)
        {
            var carConfigTask = Addressables.LoadAssetAsync<CarConfig>(AssetPath.CarConfig).ToUniTask(cancellationToken: cancellation);
            var levelConfigTask = Addressables.LoadAssetAsync<LevelConfig>(AssetPath.LevelConfig).ToUniTask(cancellationToken: cancellation);
            var gameUITask = Addressables.LoadAssetAsync<GameObject>(AssetPath.GameUI).ToUniTask(cancellationToken: cancellation);
            var cameraControllerTask = Addressables.LoadAssetAsync<GameObject>(AssetPath.CameraController).ToUniTask(cancellationToken: cancellation);

            var (carConfig, levelConfig, gameUIPrefab, cameraController) = await UniTask.WhenAll(carConfigTask, levelConfigTask, gameUITask, cameraControllerTask);

            if (carConfig == null || levelConfig == null)
            {
                return;
            }

            var levelLoadTask = levelConfig.LevelPrefabReference.LoadAssetAsync().ToUniTask(cancellationToken: cancellation);
            var levelPrefab = await levelLoadTask;

            _resolver.Instantiate(gameUIPrefab);

            if (levelPrefab != null)
            {
                var gameLevel = _resolver.Instantiate(levelPrefab, Vector3.zero, Quaternion.identity).GetComponent<GameLevel>();
                var car = await _carFactory.CreateAsync(carConfig, gameLevel.GetSpawnPoint(), Quaternion.identity);
                var camera = _resolver.Instantiate(cameraController, Vector3.zero, Quaternion.identity).GetComponent<CameraController>();

                camera.SetMovePoint(car.transform);

                _gameplayService.Initialize(gameLevel, car);
            }
        }
    }
}
