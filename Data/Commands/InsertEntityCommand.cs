using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command object that inserts a given entity in the database
    /// </summary>
    public class InsertEntityCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        private readonly object _entity;

        #endregion Fields

        #region Constructors

        public InsertEntityCommand(object entity)
        {
            _entity = entity;
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.Add(_entity);
            context.SaveChanges();
        }


        #endregion Methods
    }
}