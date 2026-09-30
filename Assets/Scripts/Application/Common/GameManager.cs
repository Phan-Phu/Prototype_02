using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Domain;
using Infrastructure;
using UnityEngine.SceneManagement;

namespace Application
{
    // Access rules used across the project:
    //  - Scene MonoBehaviour systems (LevelGrid, UnitActionSystem, ...) -> static Instance.
    //  - Plain C# services (ITurnService, IGridSystemHexFactory, ...)  -> GameManager.Instance.Get<T>().
    //  - State-changing player requests                                 -> IMediator commands.
    //  - Notifications                                                  -> EventManager.
    // Runs before every other script so services exist when other Awake methods resolve them.
    [DefaultExecutionOrder(-1000)]
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

            // Plain C# services only - scene MonoBehaviours are reached through their static Instance.
            services.AddSingleton<IGridSystemHexFactory, GridSystemHexFactory>();
            services.AddSingleton<ITurnService, TurnServiceImpl>();

            // Turn advancement is fire-and-forget (no caller needs a response), so it's driven
            // directly by TurnSystem + EventManager instead of the CQRS mediator below.

            services.AddSingleton<IMediator, MediatorImpl>();
            services.AddSingleton<ICommandHandler<SelectUnitCommand, bool>, SelectUnitCommandHandler>();
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
