using Application;
using Domain;

namespace Application
{
    public class SelectUnitCommand : ICommand<bool>
    {
        public Unit Unit { get; }

        public SelectUnitCommand(Unit unit)
        {
            Unit = unit;
        }
    }
}
