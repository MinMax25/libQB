namespace libQB.WindowServices;

public interface IResultProvider<TResult>
{
    TResult GetResult();
}
