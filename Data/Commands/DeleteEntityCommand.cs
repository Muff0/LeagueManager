using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command object that deletes a given entry from the database
    /// </summary>
    public class DeleteEntityCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        private readonly object? _entity;

        #endregion Fields

        #region Constructors

        public DeleteEntityCommand()
        {
        }

        public DeleteEntityCommand(object entity)
        {
            _entity = entity;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.Remove(_entity);
            context.SaveChanges();
        }

        #endregion Methods
    }
}