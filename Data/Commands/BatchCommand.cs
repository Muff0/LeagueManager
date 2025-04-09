using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Data.Commands
{
    public abstract class BatchCommand<T> : Command<T> where T : DbContext
    {
        #region Fields

        protected int _fullBatchesNumber,
            _totalBatchesNumber,
            _lastBatchSize,
            _currentBatch = 0;

        private List<object> _entities;

        #endregion Fields

        #region Constructors

        public BatchCommand(IEnumerable<object> entities)
        {
            _entities = entities.ToList();
        }

        #endregion Constructors

        #region Events

        public event ProgressChangedEventHandler? ProgressChanged;

        #endregion Events

        #region Properties

        public int BatchSize { get; set; } = 10000;
        public int ProgressPercentage => (_totalBatchesNumber == 0) ? 0 : (int)(((double)_currentBatch / _totalBatchesNumber) * 100);

        #endregion Properties

        #region Methods

        public List<Command<T>> GetCommands()
        {
            List<Command<T>> _outCommands = new List<Command<T>>();

            foreach (IEnumerable<object> batch in GetBatches())
            {
                Command<T> currentCommand = GetCommandForSingleBatch(batch);
                currentCommand.JobCompleted += OnBatchCompleted;
                _outCommands.Add(currentCommand);
            }

            return _outCommands;
        }

        protected virtual IEnumerable<IEnumerable<object>> GetBatches()
        {
            if (BatchSize < 1)
                throw new InvalidOperationException("BatchSize must be a positive integer number greater than 0");

            _fullBatchesNumber = 0;
            _totalBatchesNumber = 0;
            _lastBatchSize = 0;

            List<IEnumerable<object>> output = new List<IEnumerable<object>>();

            if (!_entities.Any())
                return output;

            _fullBatchesNumber = _entities.Count / BatchSize;
            _lastBatchSize = _entities.Count % BatchSize;
            _totalBatchesNumber = (_lastBatchSize != 0) ? _fullBatchesNumber + 1 : _fullBatchesNumber;

            for (int counter = 0; counter < _fullBatchesNumber; counter++)
                output.Add(_entities.GetRange(counter * BatchSize, BatchSize));

            if (_lastBatchSize != 0)
                output.Add(_entities.GetRange(_fullBatchesNumber * BatchSize, _lastBatchSize));

            return output;
        }

        protected virtual Command<T> GetCommandForSingleBatch(IEnumerable<object> batch) => new Command<T>();

        protected virtual void OnBatchCompleted(object sender, EventArgs e)
        {
            _currentBatch++;
            RaiseProgressChanged();
        }

        protected virtual void RaiseProgressChanged()
        {
            ProgressChangedEventArgs e = new ProgressChangedEventArgs(ProgressPercentage, null);

            ProgressChanged?.Invoke(this, e);
        }

        #endregion Methods
    }
}