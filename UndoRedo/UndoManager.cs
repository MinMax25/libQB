namespace libQB.UndoRedo;

using System.ComponentModel;
using libQB.Attributes;

[DISingleton<IUndoManager>]
public class UndoManager
    : IUndoManager
{
    private readonly Stack<IUndoableAction> _undoStack = new();
    private readonly Stack<IUndoableAction> _redoStack = new();
    private bool _isExecutingUndoRedo = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public bool IsExecutingUndoRedo => _isExecutingUndoRedo;

    public void PushAction(IUndoableAction action)
    {
        if (_isExecutingUndoRedo) return;

        _undoStack.Push(action);
        _redoStack.Clear();
        NotifyStateChanged();
    }

    public void RegisterPropertyChange<T>(System.Linq.Expressions.Expression<Func<T>> expr, T oldValue, T newValue)
    {
        PushAction(new PropertyChangeAction<T>(expr, oldValue, newValue));
    }

    public void Undo()
    {
        if (!CanUndo) return;

        var action = _undoStack.Pop();
        _isExecutingUndoRedo = true;
        try
        {
            action.Undo();
            _redoStack.Push(action);
        }
        finally
        {
            _isExecutingUndoRedo = false;
            NotifyStateChanged();
        }
    }

    public void Redo()
    {
        if (!CanRedo) return;

        var action = _redoStack.Pop();
        _isExecutingUndoRedo = true;
        try
        {
            action.Redo();
            _undoStack.Push(action);
        }
        finally
        {
            _isExecutingUndoRedo = false;
            NotifyStateChanged();
        }
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
    }
}

public static class UndoManagerExtensions
{
    public static void Do(this IUndoManager undoManager, IUndoableAction action)
    {
        //action.Do();
        undoManager.PushAction(action);
    }
}
