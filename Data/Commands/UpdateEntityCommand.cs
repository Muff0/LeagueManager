using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    public class UpdateEntityCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        private readonly object _entity;

        #endregion Fields

        #region Constructors

        public UpdateEntityCommand(object entity)
        {
            _entity = entity;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.Update(_entity);
            context.SaveChanges();
        }
        #endregion Methods
    }
}