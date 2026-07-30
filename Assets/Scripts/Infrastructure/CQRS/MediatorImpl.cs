using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Domain;
using Application;

namespace Infrastructure
{
    public class MediatorImpl : IMediator
    {
        public MediatorImpl()
        {
        }

        public async UniTask<R> Send<T, R>(T command) where T : ICommand<R>
        {
            //Debug.Log($"[Mediator] {typeof(T).Name}: {JsonUtility.ToJson(command)}");

            var serviceProvider = GameManager.Instance.Get<IServiceProvider>();
            ICommandHandler <T, R> handler = serviceProvider.GetRequiredService<ICommandHandler<T, R>>();
            return await handler.Handle(command);
        }
    }
}
