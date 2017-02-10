using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using WindowsProgrammering.Model;
using WindowsProgrammering.ViewModel;

namespace WindowsProgrammering.ViewModel
{
    /*
     * Tilpasset fra UMLDesigner
     */
    public class ArrowViewModel : ViewModelBase
    {
        public Arrow arrow;

        private ClassViewModel diagramA;
        public ClassViewModel DiagramA
        {
            get { return diagramA; }
            set { diagramA = value; RaisePropertyChanged(() => DiagramA); RaisePropertyChanged(() => Path); }
        }

        private ClassViewModel diagramB;
        public ClassViewModel DiagramB
        {
            get { return diagramB; }
            set { diagramB = value; RaisePropertyChanged(() => DiagramB); RaisePropertyChanged(() => Path); }
        }

        private Point posName;

        public Point PosName
        {
            get { return posName; }
            set { posName = value; RaisePropertyChanged(() => PosName); }
        }

        private EArrows Type { get { return arrow.Type; } set { arrow.Type = value; RaisePropertyChanged(() => Type); } }
        public EArrows type
        {
            get { return Type; }
            set { Type = value; RaisePropertyChanged(() => type); }
        }
        private string colorFill;
        public string ColorFill { get { return colorFill; } set { colorFill = value; RaisePropertyChanged(() => ColorFill); } }

        private string dashed;
        public string Dashed { get { return dashed; } set { dashed = value; RaisePropertyChanged(() => Dashed); } }

        private string path;
        public string Path { get { return path; } set { path = value; RaisePropertyChanged(() => Path); } }

        private string arrowText;
        public string Arrow { get { return arrowText; } set { arrowText = value; RaisePropertyChanged(() => Arrow); } }

        private string newAnchor;
        private string oldAnchor = "";
        private int angle;

        private PointCollection pathObjects = new PointCollection();
        private PointCollection normArrow = new PointCollection();
        private PointCollection thisArrow;
        private PointCollection rotatingArrow;

        public ArrowViewModel() { }

        private bool selected = false;
        public bool Selected { get { return selected; } set { selected = value; RaisePropertyChanged(() => Selected); RaisePropertyChanged(() => SelectedColor); } }

        public String SelectedColor { get { return Selected ? "Gray" : "#2E8DEF"; } }

        public ArrowViewModel(ClassViewModel nVMEndA, ClassViewModel nVMEndB, EArrows type)
        {
            arrow = new Arrow();
            DiagramA = nVMEndA;
            DiagramB = nVMEndB;
            Type = arrowTypeConverter(type);
            initArrow();
            newPath();
        }

        public ArrowViewModel(ClassViewModel diagramA, ClassViewModel diagramB, Arrow arrow)
        {
            this.arrow = arrow;
            DiagramA = diagramA;
            DiagramB = diagramB;
            Type = arrowTypeConverter(this.arrow.Type);
            initArrow();
            newPath();
        }

        public void newPath()
        {
            pathObjects = getAnchor();
            rotateArrow();
            setPath();
            setArrow();
        }

        private void initArrow()
        {
            //Definere pilens form
            normArrow.Add(new Point(-5, 10));
            normArrow.Add(new Point(0, 0));
            normArrow.Add(new Point(5, 10));
        }

        // tegner ruten fra Diagram A til B på pilene
        private void setPath()
        {
            string temp = "M";
            for (int i = 0; i < pathObjects.Count; i++)
            {
                temp += " " + pathObjects.ElementAt(i).X + "," + pathObjects.ElementAt(i).Y;
            }
            Path = temp;
        }

        private void setArrow()
        {
            string temp = "";
            if (!Type.Equals(EArrows.NORMAL) && pathObjects.Count != 0)
            {
                for (int i = 0; i < thisArrow.Count; i++)
                {
                    temp += " " + (pathObjects.ElementAt(pathObjects.Count - 1).X + rotatingArrow.ElementAt(i).X) +
                        "," + (pathObjects.ElementAt(pathObjects.Count - 1).Y + rotatingArrow.ElementAt(i).Y);

                }
                Arrow = temp;
            }
        }

        private EArrows arrowTypeConverter(EArrows type)
        {
            switch (type)
            {
                case EArrows.ASSOCIATION:
                    thisArrow = normArrow;
                    Dashed = "1 0";
                    ColorFill = "Transparent";
                    return EArrows.ASSOCIATION;


                case EArrows.NORMAL:
                    thisArrow = new PointCollection();
                    Dashed = "1 0";
                    ColorFill = "Transparent";
                    return EArrows.NORMAL;

                default:
                    thisArrow = normArrow;
                    Dashed = "1 0";
                    ColorFill = "Transparent";
                    return EArrows.NORMAL;
            }
        }

        public void rotateArrow()
        {
            if (oldAnchor != newAnchor)
            {
                oldAnchor = newAnchor;
                double cTheta = Math.Cos(angle * (Math.PI / 180));
                double sTheta = Math.Sin(angle * (Math.PI / 180));
                PointCollection temp = new PointCollection();
                for (int i = 0; i < thisArrow.Count; i++)
                {
                    double x = (int)((thisArrow.ElementAt(i).X - 0) * cTheta - (thisArrow.ElementAt(i).Y - 0) * sTheta);
                    double y = (int)((thisArrow.ElementAt(i).X - 0) * sTheta + (thisArrow.ElementAt(i).Y - 0) * cTheta);
                    temp.Add(new Point(x, y));
                }
                rotatingArrow = temp;
            }
        }

        public void setAnchor(string anchor)
        {
            if (anchor == "north")
                angle = 180;
            else if (anchor == "south")
                angle = 0;
            else if (anchor == "west")
                angle = 90;
            else if (anchor == "east")
                angle = 270;
            this.newAnchor = anchor;
        }

        private PointCollection getAnchor()
        {
            int lengthHalf;
            PointCollection temp = new PointCollection();
            if (DiagramA.West.X >= DiagramB.East.X && DiagramA.West.X + 30 >= DiagramB.East.X && (DiagramA.North.Y >= DiagramB.South.Y || DiagramA.South.Y <= DiagramB.North.Y))
            {
                if (DiagramA.North.Y >= DiagramB.South.Y)
                {
                    temp.Add(DiagramA.North);
                    temp.Add(new Point(DiagramA.North.X, DiagramB.East.Y));
                    temp.Add(DiagramB.East);
                    setAnchor("east");
                }
                else
                {
                    temp.Add(DiagramA.South);
                    temp.Add(DiagramA.West);
                    temp.Add(new Point(DiagramB.North.X, DiagramA.West.Y));
                    temp.Add(DiagramB.North);
                    setAnchor("north");
                }

            }
            else if (DiagramA.East.X <= DiagramB.West.X && DiagramB.West.X + 30 >= DiagramA.East.X && (DiagramA.North.Y > DiagramB.South.Y || DiagramA.South.Y < DiagramB.North.Y))
            {
                if (DiagramA.North.Y >= DiagramB.South.Y)
                {
                    temp.Add(DiagramA.East);
                    temp.Add(new Point(DiagramB.South.X, DiagramA.East.Y));
                    temp.Add(DiagramB.South);
                    setAnchor("south");
                }
                else
                {
                    temp.Add(DiagramA.South);
                    temp.Add(new Point(DiagramA.South.X, DiagramB.West.Y));
                    temp.Add(DiagramB.West);
                    setAnchor("west");
                }

            }
            else if (DiagramA.West.X >= DiagramB.East.X)
            {
                lengthHalf = (int)((DiagramA.West.X - DiagramB.East.X) / 2);
                temp.Add(DiagramA.West);
                temp.Add(new Point(DiagramB.East.X + lengthHalf, DiagramA.West.Y));
                temp.Add(new Point(DiagramB.East.X + lengthHalf, DiagramB.East.Y));
                temp.Add(DiagramB.East);
                setAnchor("east");
            }
            else if (DiagramA.East.X <= DiagramB.West.X)
            {
                lengthHalf = (int)((DiagramB.West.X - DiagramA.East.X) / 2);
                temp.Add(DiagramA.East);
                temp.Add(new Point(DiagramA.East.X + lengthHalf, DiagramA.East.Y));
                temp.Add(new Point(DiagramA.East.X + lengthHalf, DiagramB.West.Y));
                temp.Add(DiagramB.West);
                setAnchor("west");
            }
            else if (DiagramA.North.Y >= DiagramB.South.Y)
            {
                lengthHalf = (int)((DiagramA.North.Y - DiagramB.South.Y) / 2);
                temp.Add(DiagramA.North);
                temp.Add(new Point(DiagramA.North.X, DiagramB.South.Y + lengthHalf));
                temp.Add(new Point(DiagramB.South.X, DiagramB.South.Y + lengthHalf));
                temp.Add(DiagramB.South);
                setAnchor("south");
            }
            else if (DiagramA.South.Y <= DiagramB.North.Y)
            {
                lengthHalf = (int)((DiagramB.North.Y - DiagramA.South.Y) / 2);
                temp.Add(DiagramA.South);
                temp.Add(new Point(DiagramA.South.X, DiagramA.South.Y + lengthHalf));
                temp.Add(new Point(DiagramB.North.X, DiagramA.South.Y + lengthHalf));
                temp.Add(DiagramB.North);
                setAnchor("north");
            }
            else
            {
                lengthHalf = (int)((DiagramB.North.Y - DiagramA.South.Y) / 2);
                temp.Add(new Point(DiagramA.X + DiagramA.Width / 2, DiagramA.Y + DiagramA.Height / 2));
                temp.Add(new Point(DiagramB.X + DiagramB.Width / 2, DiagramB.Y + DiagramB.Height / 2));
            }
            return temp;
        }
    }
}

