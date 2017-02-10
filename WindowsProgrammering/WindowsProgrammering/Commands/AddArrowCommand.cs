using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsProgrammering.ViewModel;

namespace WindowsProgrammering.Commands
{
    class AddArrowCommand : IUndoRedoCommand
    {
        //Inspireret fra AdvancedWPFDemo

        private ObservableCollection<ArrowViewModel> arrowCollection;
        private ArrowViewModel arrowModel;

        public AddArrowCommand(ObservableCollection<ArrowViewModel> inputACollection, ArrowViewModel inputAModel)
        {
            arrowCollection = inputACollection;
            arrowModel = inputAModel;
        }

        public void Execute()
        {
            arrowCollection.Add(arrowModel);
        }

        public void UnExecute()
        {
            arrowCollection.Remove(arrowModel);
        }
    }
}