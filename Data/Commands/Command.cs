using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Commands
{
    public class Command<T> where T : DbContext
    {
        #region Events

        public event EventHandler? JobCompleted;

        #endregion Events


        public bool AutoDetectChangesEnabled { get; set; } = true;

        #region Methods

        protected virtual void RunAction(T context)
        {

        }

        public virtual void Execute(T context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");

            ConfigureParameters(context);

            try
            {
                RunAction(context);
                context.SaveChanges();
            }            
            catch (Exception e)
            {
                throw new DbUpdateException("Command Execution Failed: " + e.Message, e);
            }

            RaiseJobCompleted();
        }

        protected virtual void ConfigureParameters(T context)
        {
            context.ChangeTracker.AutoDetectChangesEnabled = AutoDetectChangesEnabled;
        }

        public virtual async Task ExecuteAsync(T context)
        {

            if (context == null)
                throw new ArgumentNullException("Context");

            ConfigureParameters(context);

            try
            {
                RunAction(context);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DbUpdateException("Command Execution Failed: " + e.Message, e);
            }

            RaiseJobCompleted();
        }

        protected virtual void RaiseJobCompleted()
        {
            JobCompleted?.Invoke(this, new EventArgs());
        }

        #endregion Methods
    }
}