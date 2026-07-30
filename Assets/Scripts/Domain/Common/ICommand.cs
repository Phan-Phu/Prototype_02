using Cysharp.Threading.Tasks;

namespace Domain
{
    // Lives in Domain (not Application) so Infrastructure's MediatorImpl - which only references
    // Domain - can implement IMediator, and Infrastructure command handlers can implement
    // ICommandHandler<,> without a forbidden Infrastructure -> Application reference.
    public interface ICommand<R>
    {
    }

    public interface ICommandHandler<C, R> where C : ICommand<R>
    {
        UniTask<R> Handle(C command);
    }

    public interface IMediator
    {
        UniTask<R> Send<T, R>(T command) where T : ICommand<R>;
    }
}
