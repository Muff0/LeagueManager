using Microsoft.EntityFrameworkCore;

namespace Data.Commands
{
    /// <summary>
    /// Command object that truncates the table associated with an entity type
    /// </summary>
    /// <typeparam name="T">The entity type to truncate</typeparam>
    public class DeleteAllEntitiesCommand<T, T2> : Command<T> where T : DbContext where T2 : class
    {
        #region Constructors

        public DeleteAllEntitiesCommand()
        {
        }

        #endregion Constructors

        #region Methods

        protected override void RunAction(T context)
        {
            context.RemoveRange(context.Set<T2>().ToList());
            context.SaveChanges();
        }

        #endregion Methods
    }
}