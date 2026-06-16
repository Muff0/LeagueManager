using Data.Commands;
using Microsoft.EntityFrameworkCore;

namespace Data;

public interface IDataService<T2> where T2 : DbContext
{
    #region Methods

    void Execute(Command<T2> commandObject);

    void Execute(BatchCommand<T2> commandObject);

    Task ExecuteAsync(Command<T2> commandObject);

    Task ExecuteAsync(BatchCommand<T2> commandObject);

    T? TakeFirst<T>(Query<T2, T> queryObject) where T : class;

    IList<T> RunQuery<T>(Query<T2, T> queryObject) where T : class;

    Task<T?> TakeFirstAsync<T>(Query<T2, T> queryObject) where T : class;

    Task<IList<T>> RunQueryAsync<T>(Query<T2, T> queryObject) where T : class;

    int Count<T>(Query<T2, T> queryObject) where T : class;

    Task<int> CountAsync<T>(Query<T2, T> queryObject) where T : class;


    #endregion Methods
}