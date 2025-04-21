namespace AvaloniaAppUsingHost.Infrastructure;

public interface IServiceLocator
{
    public T? GetService<T>();
    public T GetRequiredService<T>() where T : notnull;
}