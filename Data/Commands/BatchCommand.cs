using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Data.Commands;

public abstract class BatchCommand<T> : Command<T> where T : DbContext
{
    #region Constructors

    public BatchCommand(IEnumerable<object> entities)
    {
        _entities = entities.ToList();
    }

    #endregion Constructors

    #region Events

    public event ProgressChangedEventHandler? ProgressChanged;

    #endregion Events

    #region Fields

    protected int FullBatchesNumber,
        TotalBatchesNumber,
        LastBatchSize,
        CurrentBatch;

    private readonly List<object> _entities;

    #endregion Fields

    #region Properties

    public int BatchSize { get; set; } = 10000;

    public int ProgressPercentage =>
        TotalBatchesNumber == 0 ? 0 : (int)((double)CurrentBatch / TotalBatchesNumber * 100);

    #endregion Properties

    #region Methods

    public List<Command<T>> GetCommands()
    {
        var outCommands = new List<Command<T>>();

        foreach (var batch in GetBatches())
        {
            var currentCommand = GetCommandForSingleBatch(batch);
            currentCommand.JobCompleted += OnBatchCompleted;
            outCommands.Add(currentCommand);
        }

        return outCommands;
    }

    protected virtual IEnumerable<IEnumerable<object>> GetBatches()
    {
        if (BatchSize < 1)
            throw new InvalidOperationException("BatchSize must be a positive integer number greater than 0");

        FullBatchesNumber = 0;
        TotalBatchesNumber = 0;
        LastBatchSize = 0;

        var output = new List<IEnumerable<object>>();

        if (!_entities.Any())
            return output;

        FullBatchesNumber = _entities.Count / BatchSize;
        LastBatchSize = _entities.Count % BatchSize;
        TotalBatchesNumber = LastBatchSize != 0 ? FullBatchesNumber + 1 : FullBatchesNumber;

        for (var counter = 0; counter < FullBatchesNumber; counter++)
            output.Add(_entities.GetRange(counter * BatchSize, BatchSize));

        if (LastBatchSize != 0)
            output.Add(_entities.GetRange(FullBatchesNumber * BatchSize, LastBatchSize));

        return output;
    }

    protected virtual Command<T> GetCommandForSingleBatch(IEnumerable<object> batch)
    {
        return new Command<T>();
    }

    protected virtual void OnBatchCompleted(object sender, EventArgs e)
    {
        CurrentBatch++;
        RaiseProgressChanged();
    }

    protected virtual void RaiseProgressChanged()
    {
        var e = new ProgressChangedEventArgs(ProgressPercentage, null);

        ProgressChanged?.Invoke(this, e);
    }

    #endregion Methods
}