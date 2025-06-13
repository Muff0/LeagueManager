using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Data.Commands
{
    /// <summary>
    /// Command Object that updates the database values of a set of entities in a single transaction
    /// </summary>
    public class InsertEntitiesCommand<T,T2> : Command<T> where T : DbContext where T2: class
    {
        #region Fields

        private readonly IEnumerable<T2> _entities;

        #endregion Fields

        #region Constructors

        public InsertEntitiesCommand(IEnumerable<T2> entities)
        {
            _entities = entities;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.AddRange(_entities);
            context.SaveChanges();
        }

        #endregion Methods
    }
}