using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsProgrammering.ViewModel;

namespace WindowsProgrammering.Commands
{
    class AddClassCommand : IUndoRedoCommand
    {

        //Inspireret fra AdvancedWPFDemo

        private ObservableCollection<ClassViewModel> classCollection;
        private ClassViewModel classModel;
        private int index = 1;

        public AddClassCommand(ObservableCollection<ClassViewModel> inputCCollection, int inputIndex)
        {
            classCollection = inputCCollection;
            this.index = inputIndex;
        }

        public void Execute()
        {
            if (classModel == null)
            {
                classCollection.Add(classModel = new ClassViewModel() { ClassName = "Diagram", X = 100, Y = 100, Id = index });
            }
            else
            {
                classCollection.Add(classModel);
            }
        }
        public void UnExecute()
        {
            classCollection.Remove(classModel);
        }

    }
}
