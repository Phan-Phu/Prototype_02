using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Domain;

namespace Infrastructure
{
    public class MediatorImpl : IMediator
    {
        private readonly IServiceProvider serviceProvider;

        // IServiceProvider is injected by the container itself, so the mediator never reaches
        // back into the Application layer (GameManager) to find its handlers.
        public MediatorImpl(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async UniTask<R> Send<T, R>(T command) where T : ICommand<R>
        {
            ICommandHandler<T, R> handler = serviceProvider.GetRequiredService<ICommandHandler<T, R>>();
            return await handler.Handle(command);
        }
    }
}
