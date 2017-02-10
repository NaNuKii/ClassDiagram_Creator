using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsProgrammering.ViewModel;

namespace WindowsProgrammering.Commands
{
    public class DeleteClassCommand : IUndoRedoCommand
    {

        //Inspireret fra AdvancedWPFDemo

        private ObservableCollection<ClassViewModel> classCollection;
        private ClassViewModel classModel;
        private ObservableCollection<ArrowViewModel> arrowCollection;
        private ObservableCollection<ArrowViewModel> arrowRemoveCollection;

        public DeleteClassCommand(ObservableCollection<ClassViewModel> inputCCollection, ClassViewModel inputCMOdel, ObservableCollection<ArrowViewModel> inputACollection)
        {
            classCollection = inputCCollection;
            classModel = inputCMOdel;
            arrowCollection = inputACollection;
            arrowRemoveCollection = new ObservableCollection<ArrowViewModel>();
        }

        public void Execute()
        {
            foreach (ArrowViewModel arrow in arrowCollection)
            {
                if (classModel == arrow.DiagramA || classModel == arrow.DiagramB)
                {
                    arrowRemoveCollection.Add(arrow);
                }
            }
            foreach (ArrowViewModel arrow in arrowRemoveCollection)
            {
                arrowCollection.Remove(arrow);
            }
            classCollection.Remove(classModel);
        }

        public void UnExecute()
        {
            if (arrowCollection.Count == 1)
            {
                arrowCollection.Clear();
            }
            foreach (ArrowViewModel arrow in arrowRemoveCollection)
            {
                arrowCollection.Add(arrow);
            }
            classCollection.Add(classModel);
        }
    }
}
