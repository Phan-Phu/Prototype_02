using Cysharp.Threading.Tasks;
using Domain;

namespace Application
{
    public class SpendActionPointCommandHandler : ICommandHandler<SpendActionPointCommand, bool>
    {
        public UniTask<bool> Handle(SpendActionPointCommand command)
        {
            bool spent = command.Unit.TrySpendActionPointToTakeAction(command.Action);
            return UniTask.FromResult(spent);
        }
    }
}
