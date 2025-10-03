using System.ComponentModel;
using System.Linq.Expressions;

namespace libQB.UndoRedo;

public interface IUndoManager
    : INotifyPropertyChanged
{
    bool CanUndo { get; }
    bool CanRedo { get; }

    public bool IsExecutingUndoRedo { get; }

    void PushAction(IUndoableAction action);
    void RegisterPropertyChange<T>(Expression<Func<T>> expr, T oldValue, T newValue);
    void Undo();
    void Redo();
    void Clear();
}
