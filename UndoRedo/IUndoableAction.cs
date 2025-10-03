namespace libQB.UndoRedo;

public interface IUndoableAction
{
    void Undo();
    void Redo();
}
