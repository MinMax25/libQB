using System.Windows;
using System.Windows.Controls;

namespace libQB.Attributes;

public abstract class DIAttribute
    : Attribute
{
    public Type? Type1 { get; set; }
    public Type? Type2 { get; set; }

    public DIAttribute(Type? value1 = null, Type? value2 = null)
    {
        this.Type1 = value1;
        this.Type2 = value2;
    }
}

public class DIWindowAttribute<TWindow>
    : DIAttribute
    where TWindow : Window
{
    public DIWindowAttribute() : base(typeof(TWindow))
    {
    }
}

public class DIWindowAttribute<TInterface, TWindow>
    : DIAttribute
    where TInterface : class
    where TWindow : Window
{
    public DIWindowAttribute() : base(typeof(TInterface), typeof(TWindow))
    {
    }
}

public class DIPageAttribute<T>
    : DIAttribute
    where T : Page
{
    public DIPageAttribute() : base(typeof(T))
    {
    }
}

public class DIUserControlAttribute<T>
    : DIAttribute
    where T : UserControl
{
    public DIUserControlAttribute() : base(typeof(T))
    {
    }
}

public class DISingletonAttribute
    : DIAttribute
{
}

public class DISingletonAttribute<T>
    : DIAttribute
{
    public DISingletonAttribute() : base(typeof(T))
    {
    }
}
