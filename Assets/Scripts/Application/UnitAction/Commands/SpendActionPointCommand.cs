using Domain;

namespace Application
{
    public class SpendActionPointCommand : ICommand<bool>
    {
        public Unit Unit { get; }
        public BaseAction Action { get; }

        public SpendActionPointCommand(Unit unit, BaseAction action)
        {
            Unit = unit;
            Action = action;
        }
    }
}
