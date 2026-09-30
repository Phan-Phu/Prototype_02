using Cysharp.Threading.Tasks;
using Domain;

namespace Application
{
    public class SelectUnitCommandHandler : ICommandHandler<SelectUnitCommand, bool>
    {
        public UniTask<bool> Handle(SelectUnitCommand command)
        {
            // Resolved per call: the handler is a DI singleton that outlives gameplay scenes.
            UnitActionSystem.Instance.SetSelectedUnit(command.Unit);
            return UniTask.FromResult(true);
        }
    }
}
