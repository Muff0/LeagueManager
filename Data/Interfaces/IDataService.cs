﻿using Data.Commands;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public interface IDataService<T2> where T2 : DbContext
    {
        #region Methods

        void Execute(Command<T2> commandObject);

        void Execute(BatchCommand<T2> commandObject);

        Task ExecuteAsync(Command<T2> commandObject);

        Task ExecuteAsync(BatchCommand<T2> commandObject);

        T RunQuery<T>(Scalar<T2, T> queryObject) where T : class;

        IList<T> RunQuery<T>(Query<T2, T> queryObject) where T : class;

        Task<T> RunQueryAsync<T>(Scalar<T2, T> queryObject) where T : class;

        Task<IList<T>> RunQueryAsync<T>(Query<T2, T> queryObject) where T : class;

        #endregion Methods
    }
}