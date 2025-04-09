using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Data.Commands
{
    /// <summary>
    /// Command Object that inserts a set of entities in a single transaction
    /// </summary>
    public class BatchInsertEntitiesCommand<T> : BatchCommand<T> where T : DbContext
    {
        #region Constructors

        public BatchInsertEntitiesCommand(IEnumerable<object> entities) : base(entities)
        {
        }

        #endregion Constructors

        #region Methods

        protected override Command<T> GetCommandForSingleBatch(IEnumerable<object> batch) => new InsertEntitiesCommand<T>(batch);

        #endregion Methods
    }
}