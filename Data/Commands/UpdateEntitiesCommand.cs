using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Data.Commands
{
    /// <summary>
    /// Command Object that updates the database values of a set of entities in a single transaction
    /// </summary>
    public class UpdateEntitiesCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        private readonly IEnumerable<object> _entities;

        #endregion Fields

        #region Constructors

        public UpdateEntitiesCommand(IEnumerable<object> entities)
        {
            _entities = entities;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.UpdateRange(_entities);
            context.SaveChanges();
        }

        #endregion Methods
    }
}