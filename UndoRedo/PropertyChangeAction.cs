using System.Linq.Expressions;

namespace libQB.UndoRedo;

public class PropertyChangeAction<T> : IUndoableAction
{
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;
    private readonly T _oldValue;
    private readonly T _newValue;

    public PropertyChangeAction(Expression<Func<T>> propertyExpression, T oldValue, T newValue)
    {
        if (propertyExpression.Body is not MemberExpression memberExpr)
            throw new ArgumentException("Expression must target a property", nameof(propertyExpression));

        var targetExpression = memberExpr.Expression;
        if (targetExpression == null)
            throw new ArgumentException("Target object cannot be null", nameof(propertyExpression));

        var parameter = Expression.Parameter(typeof(T), "value");

        // Getter
        _getter = propertyExpression.Compile();

        // Setter
        var setterExpr = Expression.Lambda<Action<T>>(
            Expression.Assign(memberExpr, parameter),
            parameter
        );
        _setter = setterExpr.Compile();

        _oldValue = oldValue;
        _newValue = newValue;
    }

    public void Undo() => _setter(_oldValue);
    public void Redo() => _setter(_newValue);
}
