using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Services 
{
    public class SceneLoaderService : IAsyncStartable
    {
        private AsyncOperationHandle<SceneInstance> _previousScene;
        private AsyncOperationHandle<SceneInstance> _nextScene;
        private string _builtInSceneToUnload;

        private bool _isInitialized;

        [Inject]
        public SceneLoaderService(){}

        public async Awaitable StartAsync(CancellationToken cancellation = default)
        {
            if (_isInitialized) return;

            await Addressables.InitializeAsync().ToUniTask(cancellationToken: cancellation);

            _builtInSceneToUnload = AssetPath.BootScene;

            _isInitialized = true;
        }

        public async UniTask LoadScene(string sceneName)
        {
            while (!_isInitialized)
            {
                await UniTask.Yield();
            }

            _previousScene = _nextScene;
            _nextScene = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive, false);

            await _nextScene.ToUniTask();
        }

        public async UniTask SwitchScenes()
        {
            if (_nextScene.IsDone)
            {
                await _nextScene.Result.ActivateAsync().ToUniTask();

                if (!string.IsNullOrEmpty(_builtInSceneToUnload))
                {
                    await SceneManager.UnloadSceneAsync(_builtInSceneToUnload).ToUniTask();
                    _builtInSceneToUnload = "";
                }
                else if (_previousScene.IsValid())
                {
                    await Addressables.UnloadSceneAsync(_previousScene).ToUniTask();
                }
            }
        }
    }
}
