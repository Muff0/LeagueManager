using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command Object that inserts a set of entities in a single transaction
    /// </summary>
    public class BatchUpdateEntitiesCommand<T> : BatchCommand<T> where T : DbContext
    {
        #region Constructors

        public BatchUpdateEntitiesCommand(IEnumerable<object> entities) : base(entities)
        {
        }

        #endregion Constructors

        #region Methods

        protected override Command<T> GetCommandForSingleBatch(IEnumerable<object> batch) => new UpdateEntitiesCommand<T>(batch);

        #endregion Methods
    }
}