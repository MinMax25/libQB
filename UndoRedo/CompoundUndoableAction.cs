namespace libQB.UndoRedo;

public class CompoundUndoableAction
    : IUndoableAction
{
    private readonly List<IUndoableAction> _actions = new();

    public void Add(IUndoableAction action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        _actions.Add(action);
    }

    public void Undo()
    {
        for (int i = _actions.Count - 1; i >= 0; i--)
        {
            _actions[i].Undo();
        }
    }

    public void Redo()
    {
        foreach (var action in _actions)
        {
            action.Redo();
        }
    }

    public bool IsEmpty => _actions.Count == 0;
}
