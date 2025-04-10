using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Commands;
using Shared.Enum;

namespace Data.Commands.Queue
{
    public class SetCommandMessageStatusCommand : Command<QueueContext>
    {
        public int CommandMessageId { get; set; }
        public QueueStatus NewStatus { get; set; }

        public bool UpdateProcessedTime { get; set; } = false;
        public bool RaiseRetriesCounter { get; set; } = false;

        protected override void RunAction(QueueContext context)
        {
            base.RunAction(context);
            var command = context.CommandQueue.FirstOrDefault(cm => cm.Id == CommandMessageId);
            
            if (command == null)
                throw new InvalidOperationException("Invalid Queue Id");

            command.Status = NewStatus;

            if (RaiseRetriesCounter)
                command.Retries++;
            if (UpdateProcessedTime)
                command.ProcessedAtUtc = DateTime.UtcNow;

        }
    }
}
