namespace Backend.Application.Interface.Caching
{
    public interface IInMemoryCache
    {
        Task<T> GetData<T>(string key);
        Task<bool>   SetData<T>(string key, T value, DateTimeOffset expirationTime);
        Task<bool>  RemoveData(string key);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory);
    }
}
