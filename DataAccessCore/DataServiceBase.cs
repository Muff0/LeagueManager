using DataAccessCore.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessCore
{
    public class DataServiceBase<T> : IDataService<T> where T : DbContext
    {
        #region Fields

        private IDesignTimeDbContextFactory<T> _contextFactory;

        #endregion Fields

        #region Constructors

        public DataServiceBase(IDesignTimeDbContextFactory<T> contextFactory)
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

        public T2 RunQuery<T2>(Scalar<T2, T> queryObject)
        {
            return queryObject.Execute(GetDbContext());
        }

        public IQueryable<T2> RunQuery<T2>(Query<T2, T> queryObject) where T2 : class
        {
            return queryObject.Execute(GetDbContext());
        }

        public async Task<T2> RunQueryAsync<T2>(Scalar<T2, T> queryObject)
        {
            return await queryObject.ExecuteAsync(GetDbContext());
        }

        public async Task<IQueryable<T2>> RunQueryAsync<T2>(Query<T2, T> queryObject) where T2 : class
        {
            return await queryObject.ExecuteAsync(GetDbContext());
        }

        private T GetDbContext()
        {
            return _contextFactory.CreateDbContext(new string[] { });
        }

        #endregion Methods
    }
}