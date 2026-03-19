using Data.Model;

namespace LeagueCoreService.Queue;

    public interface ICommandHandler
    {
        string CommandType { get; }
        Task HandleAsync(CommandMessage cmd);
    }