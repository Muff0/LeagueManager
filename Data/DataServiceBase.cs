using Data.Commands;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class DataServiceBase<T> : IDataService<T> where T : DbContext
{
    #region Fields

    private readonly IDbContextFactory<T> _contextFactory;

    #endregion Fields

    #region Constructors

    public DataServiceBase(IDbContextFactory<T> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    #endregion Constructors

    #region Methods

    public void Execute(Command<T> commandObject)
    {
        using (var dbContext = GetDbContext())
        {
            commandObject.Execute(dbContext);
        }
    }

    public void Execute(BatchCommand<T> commandObject)
    {
        foreach (var batch in commandObject.GetCommands())
            Execute(batch);
    }

    public async Task ExecuteAsync(Command<T> commandObject)
    {
        using (var dbContext = GetDbContext())
        {
            await commandObject.ExecuteAsync(dbContext);
        }
    }

    public int Count<T2>(Query<T, T2> queryObject) where T2 : class
    {
        return queryObject.BuildQuery(GetDbContext()).Count();
    }


    public async Task<int> CountAsync<T2>(Query<T, T2> queryObject) where T2 : class
    {
        return await queryObject.BuildQuery(GetDbContext()).CountAsync();
    }

    public async Task ExecuteAsync(BatchCommand<T> commandObject)
    {
        foreach (var batch in commandObject.GetCommands())
            await ExecuteAsync(batch);
    }

    public T1? TakeFirst<T1>(Query<T, T1> queryObject) where T1 : class
    {
        return queryObject.BuildQuery(GetDbContext()).FirstOrDefault();
    }

    public IList<T2> RunQuery<T2>(Query<T, T2> queryObject) where T2 : class
    {
        return queryObject.Execute(GetDbContext());
    }

    public async Task<T1?> TakeFirstAsync<T1>(Query<T, T1> queryObject) where T1 : class
    {
        return await queryObject.BuildQuery(GetDbContext()).FirstOrDefaultAsync();
    }

    public async Task<IList<T2>> RunQueryAsync<T2>(Query<T, T2> queryObject) where T2 : class
    {
        return await queryObject.ExecuteAsync(GetDbContext());
    }

    private T GetDbContext()
    {
        return _contextFactory.CreateDbContext();
    }

    #endregion Methods
}