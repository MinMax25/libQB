namespace libQB.WindowServices;

public interface IWindowClosingAware
{
    Task<bool> OnWindowClosingAsync();
}
