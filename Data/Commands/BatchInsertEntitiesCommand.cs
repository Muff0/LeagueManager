using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command Object that inserts a set of entities in a single transaction
    /// </summary>
    public class BatchInsertEntitiesCommand<T, T2> : BatchCommand<T> where T : DbContext where T2 : class
    {
        #region Constructors

        public BatchInsertEntitiesCommand(IEnumerable<T2> entities) : base(entities)
        {
        }

        #endregion Constructors

        #region Methods

        protected override Command<T> GetCommandForSingleBatch(IEnumerable<object> batch) => new InsertEntitiesCommand<T, T2>(batch as IEnumerable<T2>);

        #endregion Methods
    }
}