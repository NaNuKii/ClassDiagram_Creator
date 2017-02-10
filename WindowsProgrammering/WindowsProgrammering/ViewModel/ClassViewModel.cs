using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsProgrammering.Model;

namespace WindowsProgrammering.ViewModel
{
    [System.Xml.Serialization.XmlInclude(typeof(ClassViewModel))]
    public class ClassViewModel : ViewModelBase
    {

        /*
         * Tilpasset af UMLDesigner, inspireret af AdvancedWPFDemo
         */
        public Diagram diagram;

        public int Id
        {
            get { return diagram.Id; }
            set { diagram.Id = value; RaisePropertyChanged(() => Id); }
        }

        public int X
        {
            get { return diagram.X; }
            set { diagram.X = value; RaisePropertyChanged(() => X); RaisePropertyChanged(() => North); RaisePropertyChanged(() => South); RaisePropertyChanged(() => West); RaisePropertyChanged(() => East); }
        }

        public int Y
        {
            get { return diagram.Y; }
            set { diagram.Y = value; RaisePropertyChanged(() => Y); RaisePropertyChanged(() => North); RaisePropertyChanged(() => South); RaisePropertyChanged(() => West); RaisePropertyChanged(() => East); }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set { width = value; RaisePropertyChanged(() => Width); RaisePropertyChanged(() => North); RaisePropertyChanged(() => South); RaisePropertyChanged(() => West); RaisePropertyChanged(() => East); }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set { height = value; RaisePropertyChanged(() => Height); RaisePropertyChanged(() => North); RaisePropertyChanged(() => South); RaisePropertyChanged(() => West); RaisePropertyChanged(() => East); }
        }

        private Point north;
        private Point south;
        private Point east;
        private Point west;

        public Point North { get { north.X = diagram.X + Width / 2; north.Y = diagram.Y; return north; } set { north.X = diagram.X + Width / 2; north.Y = diagram.Y; RaisePropertyChanged(() => North); } }
        public Point South { get { south.X = diagram.X + Width / 2; south.Y = diagram.Y + Height; return south; } set { south.X = diagram.X + Width / 2; south.Y = diagram.Y + Height; RaisePropertyChanged(() => South); } }
        public Point East { get { east.X = diagram.X + Width; east.Y = diagram.Y + Height / 2; return east; } set { east.X = diagram.X + Width; east.Y = diagram.Y + Height / 2; RaisePropertyChanged(() => East); } }
        public Point West { get { west.X = diagram.X; west.Y = diagram.Y + Height / 2; return west; } set { west.X = diagram.X; west.Y = diagram.Y + Height / 2; RaisePropertyChanged(() => West); } }

        public String ClassName
        {
            get { return diagram.ClassName; }
            set { diagram.ClassName = value; }
        }

        private bool selected = false;
        public bool Selected { get { return selected; } set { selected = value; RaisePropertyChanged(() => Selected); RaisePropertyChanged(() => SelectedColor); } }

        public String SelectedColor { get { return Selected ? "Gray" : "#2E8DEF"; } }


        public ClassViewModel()
        {
            diagram = new Diagram();
        }

        public ClassViewModel(Diagram diagram)
        {
            this.diagram = diagram;
        }
    }
}
