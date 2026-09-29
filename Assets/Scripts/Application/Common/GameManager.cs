using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Domain;
using Infrastructure;
using UnityEngine.SceneManagement;

namespace Application
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private IServiceProvider serviceProvider;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Has more than 1 GameManager " + Instance + "- " + transform);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            ConfigServices();
        }

        private void ConfigServices()
        {
            ServiceCollection services = new ServiceCollection();

            // Existing scene-placed MonoBehaviours - registered as-is, not spawned by the container.
            services.AddSingleton(typeof(IGridSystemHex<>), typeof(GridSystemHex<>));
            services.AddSingleton<IInteractable, Door>();
            services.AddSingleton<IInteractable, InteractSphere>();
            services.AddSingleton<IPathfindingAlgorithm, AStarPathfinder>();
            services.AddSingleton<ITurnService, TurnServiceImpl>();

            // Scene-placed MonoBehaviour - resolved via factory (can't be built by the container's own
            // Activator-based construction) so DI consumers share the one instance already in the scene.
            // Transient (not singleton): GameManager outlives gameplay scenes, so a cached instance
            // would point at a destroyed object after the scene is reloaded.
            services.AddTransient(sp => FindAnyObjectByType<UnitActionSystem>());

            // Turn advancement is fire-and-forget (no caller needs a response), so it's driven
            // directly by TurnSystem + EventManager instead of the CQRS mediator below.

            services.AddSingleton<IMediator, MediatorImpl>();
            // Transient because it holds the scene's UnitActionSystem.
            services.AddTransient<ICommandHandler<SelectUnitCommand, bool>, SelectUnitCommandHandler>();
            services.AddSingleton<ICommandHandler<SpendActionPointCommand, bool>, SpendActionPointCommandHandler>();

            serviceProvider = services.BuildServiceProvider();
            print("Config Service successfully");
        }

        public T Get<T>()
        {
            return serviceProvider.GetRequiredService<T>();
        }

        public void ChangeSceneName (string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
