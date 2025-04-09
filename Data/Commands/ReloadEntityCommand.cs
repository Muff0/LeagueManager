using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command object that reloads all values for a given Entry
    /// </summary>
    public class ReloadEntityCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        private readonly object _entity;

        #endregion Fields

        #region Constructors

        public ReloadEntityCommand(object entity)
        {
            _entity = entity;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.Attach(_entity);
            context.Entry(_entity).Reload();
        }

        #endregion Methods
    }
}