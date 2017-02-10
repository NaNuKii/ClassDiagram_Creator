using System.Collections.Generic;
using System.Linq;

namespace WindowsProgrammering.Commands
{  
        public interface IUndoRedoCommand
        {
            void Execute();
            void UnExecute();
        }
}   