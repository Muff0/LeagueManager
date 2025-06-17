using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command object that deletes multiple entries in a single transaction
    /// </summary>
    /// <typeparam name="T">The type of DbContext to target</typeparam>
    public class DeleteEntitiesCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        private readonly IEnumerable<object> _entities;

        #endregion Fields

        #region Constructors

        public DeleteEntitiesCommand(IEnumerable<object> entities)
        {
            _entities = entities;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.RemoveRange(_entities);
            context.SaveChanges();
        }

        #endregion Methods
    }
}