using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Domain;
using Infrastructure;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class MediatorImplTests
    {
        private class DoubleCommand : ICommand<int>
        {
            public int Value { get; }

            public DoubleCommand(int value)
            {
                Value = value;
            }
        }

        private class DoubleCommandHandler : ICommandHandler<DoubleCommand, int>
        {
            public UniTask<int> Handle(DoubleCommand command)
            {
                return UniTask.FromResult(command.Value * 2);
            }
        }

        // Minimal container: MediatorImpl only needs IServiceProvider.GetService.
        private class FakeServiceProvider : IServiceProvider
        {
            private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

            public void Add<T>(T service)
            {
                services[typeof(T)] = service;
            }

            public object GetService(Type serviceType)
            {
                return services.TryGetValue(serviceType, out object service) ? service : null;
            }
        }

        [Test]
        public void Send_ResolvesTheHandlerAndReturnsItsResult()
        {
            var serviceProvider = new FakeServiceProvider();
            serviceProvider.Add<ICommandHandler<DoubleCommand, int>>(new DoubleCommandHandler());
            var mediator = new MediatorImpl(serviceProvider);

            int result = mediator.Send<DoubleCommand, int>(new DoubleCommand(21)).GetAwaiter().GetResult();

            Assert.AreEqual(42, result);
        }

        [Test]
        public void Send_WithoutRegisteredHandler_Throws()
        {
            var mediator = new MediatorImpl(new FakeServiceProvider());

            Assert.Throws<InvalidOperationException>(() =>
                mediator.Send<DoubleCommand, int>(new DoubleCommand(1)).GetAwaiter().GetResult());
        }
    }
}
