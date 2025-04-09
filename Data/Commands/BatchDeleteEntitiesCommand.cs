using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Data.Commands
{
    /// <summary>
    /// Command object that deletes multiple entries in a single transaction
    /// </summary>
    /// <typeparam name="T">The type of DbContext to target</typeparam>
    public class BatchDeleteEntitiesCommand<T> : BatchCommand<T> where T : DbContext
    {
        #region Constructors

        public BatchDeleteEntitiesCommand(IEnumerable<object> entities) : base(entities)
        {
        }

        #endregion Constructors

        #region Methods

        protected override Command<T> GetCommandForSingleBatch(IEnumerable<object> batch) => new DeleteEntitiesCommand<T>(batch);

        #endregion Methods
    }
}