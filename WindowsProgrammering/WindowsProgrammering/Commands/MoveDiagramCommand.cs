using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsProgrammering.ViewModel;

namespace WindowsProgrammering.Commands
{
    //inspireret fra AdvancedWPFDemo
    public class MoveDiagramCommand : IUndoRedoCommand
    {
        private ObservableCollection<ArrowViewModel> arrowCollection;
        private ClassViewModel classModel;
        private int X, Y, oldX, oldY;

        public MoveDiagramCommand(ClassViewModel inputCModel, int inputX, int inputY, int inputOldX, int inputOldY, ObservableCollection<ArrowViewModel> inputACollection)
        {
            this.classModel = inputCModel;
            this.X = inputX;
            this.Y = inputY;
            this.oldX = inputOldX;
            this.oldY = inputOldY;
            this.arrowCollection = inputACollection;
        }

        public void Execute()
        {
            classModel.X = X;
            classModel.Y = Y;
            for (int i = 0; i < arrowCollection.Count; i++)
            {
                if (classModel == arrowCollection[i].DiagramA || classModel == arrowCollection[i].DiagramB)
                {
                    arrowCollection[i].newPath();
                }
            }
        }
        public void UnExecute()
        {
            classModel.X = oldX;
            classModel.Y = oldY;
            for (int i = 0; i < arrowCollection.Count; i++)
            {
                if (classModel == arrowCollection[i].DiagramA || classModel == arrowCollection[i].DiagramB)
                {
                    arrowCollection[i].newPath();
                }
            }
        }

    }
}
