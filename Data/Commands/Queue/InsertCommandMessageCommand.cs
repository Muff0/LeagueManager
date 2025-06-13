using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Commands;
using Data.Model;
using Shared.Enum;

namespace Data.Commands.Queue
{
    public class InsertCommandMessageCommand : Command<QueueContext>
    {
        public CommandMessage? NewCommand { get; set; }

        protected override void RunAction(QueueContext context)
        {
            base.RunAction(context);

            if (NewCommand != null) 
                context.Add(NewCommand);

        }
    }
}
