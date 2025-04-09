using Data.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data
{
    public class DataServiceBase<T> : IDataService<T> where T : DbContext
    {
        #region Fields

        private IDbContextFactory<T> _contextFactory;

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
            commandObject.Execute(GetDbContext());
        }

        public void Execute(BatchCommand<T> commandObject)
        {
            foreach (Command<T> batch in commandObject.GetCommands())
                Execute(batch);
        }

        public async Task ExecuteAsync(Command<T> commandObject)
        {
            await commandObject.ExecuteAsync(GetDbContext());
        }

        public async Task ExecuteAsync(BatchCommand<T> commandObject)
        {
            foreach (Command<T> batch in commandObject.GetCommands())
                await ExecuteAsync(batch);
        }

        public T2 RunQuery<T2>(Scalar<T, T2> queryObject)
        {
            return queryObject.Execute(GetDbContext());
        }

        public ICollection<T2> RunQuery<T2>(Query<T, T2> queryObject) where T2 : class
        {
            return queryObject.Execute(GetDbContext());
        }

        public async Task<T2> RunQueryAsync<T2>(Scalar<T, T2> queryObject)
        {
            return await queryObject.ExecuteAsync(GetDbContext());
        }

        public async Task<ICollection<T2>> RunQueryAsync<T2>(Query<T, T2> queryObject) where T2 : class
        {
            return await queryObject.ExecuteAsync(GetDbContext());
        }

        private T GetDbContext()
        {
            return _contextFactory.CreateDbContext();
        }

        #endregion Methods
    }
}