using Cysharp.Threading.Tasks;
using Domain;

namespace Application
{
    public class SelectUnitCommandHandler : ICommandHandler<SelectUnitCommand, bool>
    {
        private readonly UnitActionSystem unitActionSystem;

        public SelectUnitCommandHandler(UnitActionSystem unitActionSystem)
        {
            this.unitActionSystem = unitActionSystem;
        }

        public UniTask<bool> Handle(SelectUnitCommand command)
        {
            unitActionSystem.SetSelectedUnit(command.Unit);
            return UniTask.FromResult(true);
        }
    }
}
