using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using WindowsProgrammering.Commands;
using WindowsProgrammering.Model;
using WindowsProgrammering.Utility;

namespace WindowsProgrammering.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        /*
         * OBS!
         * Når der refereres til UMLDesigner, henvises til: https://github.com/plantener/02350.git
         */

        private Canvas canvas;
        private UndoRedoController undoRedoController = UndoRedoController.GetInstance();
        private Thread saveThread;
        private int classIndex = 1;
        public int ClassIndex
        {
            get
            {
                return classIndex++;
            }
            private set
            {
                classIndex = value;
                RaisePropertyChanged(() => ClassIndex);
            }
        }
        private double scale = 1;
        public double Scale
        {
            get
            {
                return scale;
            } set {
                scale = value;
                RaisePropertyChanged(() => Scale);
            }
        }

        #region positionering
        private int relativeMousePositionX = -1;
        private int relativeMousePositionY = -1;
        private Point oldMousePositionPoint;
        private Point moveArrowPoint;
        FrameworkElement movingClass;
        #endregion

        #region booleans til mousemovement og delete
        public bool DeleteActive
        {
            get
            {
                if (isFocused || arrowIsFocused)
                {
                    deleteActive = true;
                    return deleteActive;
                } else {
                    deleteActive = false;
                    return deleteActive;
                }
            } set {
                deleteActive = value;
                RaisePropertyChanged(() => DeleteActive);
            }
        }
        private bool isFocused = false;
        public bool IsFocused
        {
            get
            {
                return isFocused;
            } set {
                isFocused = value;
                RaisePropertyChanged(() => IsFocused);
                RaisePropertyChanged(() => DeleteActive);
            }
        }
        private bool arrowIsFocused = false;
        public bool ArrowIsFocused {
            get
            {
                return arrowIsFocused;
            } set {
                arrowIsFocused = value;
                RaisePropertyChanged(() => ArrowIsFocused);
                RaisePropertyChanged(() => DeleteActive);
            }
        }
        private bool isAddingArrow = false;
        private bool deleteActive = false;
        #endregion

        #region opretter ViewModels og lister af disse
        private ClassViewModel focusedClass = null;
        public ClassViewModel FocusedClass {
            get
            {
                return focusedClass;
            }
            private set
            {
                if (focusedClass != null)
                {
                    FocusedClass.Selected = false;
                }
                focusedClass = value;
                if (focusedClass == null)
                {
                    IsFocused = false;
                } else {
                    IsFocused = true;
                    FocusedClass.Selected = true;
                    FocusedArrow = null;
                };
            }
        }
        private ArrowViewModel focusedArrow = null;
        public ArrowViewModel FocusedArrow
        {
            get
            {
                return focusedArrow;
            }
            private set
            {
                if (focusedArrow != null)
                {
                    FocusedArrow.Selected = false;
                }
                focusedArrow = value;
                if (focusedArrow == null)
                {
                    ArrowIsFocused = false;
                } else {
                    ArrowIsFocused = true;
                    FocusedArrow.Selected = true;
                    FocusedClass = null;
                };
            }
        }
        public ObservableCollection<ClassViewModel> classCollection { get; set; }
        public ObservableCollection<ArrowViewModel> arrowCollection { get; set; }
        private ClassViewModel startArrow;
        private EArrows type = EArrows.NORMAL;
        #endregion

        #region opretter Commands
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand AddClassCommand { get; private set; }
        public ICommand AddAssociationArrowCommand { get; private set; }
        public ICommand AddNormalArrowCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand MouseDown { get; private set; }
        public ICommand MouseMove { get; private set; }
        public ICommand MouseUp { get; private set; }
        public ICommand mouseDownCanvas { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }
        #endregion

        
        public MainViewModel()
        {
            classCollection = new ObservableCollection<ClassViewModel>();
            arrowCollection = new ObservableCollection<ArrowViewModel>();

            #region instansierer commands med argumenter

            UndoCommand = new RelayCommand(undoRedoController.Undo, undoRedoController.CanUndo);
            RedoCommand = new RelayCommand(undoRedoController.Redo, undoRedoController.CanRedo);

            AddClassCommand = new RelayCommand(AddDiagram);
            AddAssociationArrowCommand = new RelayCommand(AddAssociationArrow);
            AddNormalArrowCommand = new RelayCommand(AddNormalArrow);

            ZoomOutCommand = new RelayCommand(ZoomOut);
            ZoomInCommand = new RelayCommand(ZoomIn);
            MouseDown = new RelayCommand<MouseButtonEventArgs>(MouseDownDiagram);
            MouseMove = new RelayCommand<MouseEventArgs>(MouseMoveDiagram);
            MouseUp = new RelayCommand<MouseButtonEventArgs>(MouseUpDiagram);

            mouseDownCanvas = new RelayCommand<MouseEventArgs>(MouseDownCanvas);
            DeleteCommand = new RelayCommand(Delete);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);

            #endregion


        }

        #region XML serialization - Inspireret fra AdvancedWPFDemo og inspireret af UMLDesigner, tilpasset til vores projekt

        public void SerializeObjectToXML(string filepath)
        {
            SaveLoadCollection serializetype = new SaveLoadCollection(classCollection, arrowCollection);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveLoadCollection));
            using (StreamWriter wr = new StreamWriter(filepath))
            {
                serializer.Serialize(wr, serializetype);
            }
        }

        private void DeSerializeXMLToObject(string filepath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveLoadCollection));
            using (StreamReader wr = new StreamReader(filepath))
            {
                SaveLoadCollection Load = (SaveLoadCollection)serializer.Deserialize(wr);
                classCollection.Clear();
                arrowCollection.Clear();
                ClassIndex = Load.tempDiagrams.Count + 1;
                foreach (Diagram tempDiagram in Load.tempDiagrams)
                {
                    classCollection.Add(new ClassViewModel(tempDiagram));
                }
                foreach (ArrowViewModel arrow in Load.tempArrows)
                {
                    ClassViewModel DiagramA = null;
                    ClassViewModel DiagramB = null;
                    foreach (ClassViewModel diagram in classCollection)
                    {
                        if (arrow.DiagramA.Id == diagram.Id)
                        {
                            DiagramA = diagram;
                        }
                        else if (arrow.DiagramB.Id == diagram.Id)
                        {
                            DiagramB = diagram;
                        }
                    }
                    ArrowViewModel tempArrow = new ArrowViewModel(DiagramA, DiagramB, arrow.arrow);
                    undoRedoController.AddAndExecute(new AddArrowCommand(arrowCollection, tempArrow));
                }
                undoRedoController.Reset();

            }
        }

        #endregion

        #region save/load og delete - Inspireret af UMLDesigner, tilpasset til vores projekt

        public class SaveLoadCollection
        {
            public ObservableCollection<Diagram> tempDiagrams = new ObservableCollection<Diagram>();
            public ObservableCollection<ArrowViewModel> tempArrows = new ObservableCollection<ArrowViewModel>();
            public SaveLoadCollection(ObservableCollection<ClassViewModel> classes, ObservableCollection<ArrowViewModel> arrows)
            {
                foreach (ClassViewModel diagram in classes)
                {
                    tempDiagrams.Add(diagram.diagram);
                }
                tempArrows = arrows;
            }
            public SaveLoadCollection() { }
        }

        private void Load()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Load diagram",
                Filter = "XML (*.xml)|*.xml"
            };
            if (dialog.ShowDialog() != true)
                return;

            string path = dialog.FileName;
            DeSerializeXMLToObject(path);
        }

        private void Save()
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Title = "Save diagram",
                FileName = "classdiagram",
                Filter = " XML (*.xml)|*.xml| All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;
            string path = dialog.FileName;
            saveThread = new Thread(() => SerializeObjectToXML(path));
            saveThread.Start();
        }

        private void Delete()
        {
         /*
         * Inspireret af UMLDesigner
         */
            if (FocusedClass != null)
            {
                if (MessageBox.Show("Slet klasse?", "Bekræft sletning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    undoRedoController.AddAndExecute(new DeleteClassCommand(classCollection, FocusedClass, arrowCollection));
                    FocusedClass = null;
                }
            }
            else if (FocusedArrow != null)
            {
                if (MessageBox.Show("Slet Pilen?", "Bekræft sletning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    undoRedoController.AddAndExecute(new DeleteArrowCommand(arrowCollection, FocusedArrow));
                    FocusedArrow = null;
                }
            }
        }

        #endregion

        #region add-implementationer  - Inspireret af UMLDesigner, tilpasset til vores projekt

        public void AddDiagram()
        {
            undoRedoController.AddAndExecute(new AddClassCommand(classCollection, ClassIndex));
        }

        public void AddArrow()
        {
            isAddingArrow = true;
            FocusedClass = null;
            FocusedArrow = null;
        }

        public void AddAssociationArrow()
        {
            type = EArrows.ASSOCIATION;
            AddArrow();
        }

        public void AddNormalArrow()
        {
            type = EArrows.NORMAL;
            AddArrow();
        }

        #endregion

        #region mouse-control - Inspireret af UMLDesigner, tilpasset til vores projekt

        private void MouseDownCanvas(MouseEventArgs obj)
        {
            FrameworkElement clickedObj = (FrameworkElement)obj.MouseDevice.Target;
            try
            {
                if (obj.Source is MainWindow)
                {
                    FocusedClass = null;
                    FocusedArrow = null;

                    if (movingClass != null)
                    {
                        DependencyObject scope = FocusManager.GetFocusScope(movingClass);
                        FocusManager.SetFocusedElement(scope, clickedObj as IInputElement);
                        Keyboard.ClearFocus();
                        Application.Current.MainWindow.Focus();
                    }
                }
                else if (clickedObj.DataContext is ArrowViewModel)
                {
                    FocusedArrow = (ArrowViewModel)clickedObj.DataContext;
                }
            }
            catch
            {

            }
        }

        public void MouseDownDiagram(MouseButtonEventArgs e)
        {
            if (!isAddingArrow)
            {
                canvas = FindParent<Canvas>((FrameworkElement)e.MouseDevice.Target);
                oldMousePositionPoint = e.GetPosition(canvas);
                e.MouseDevice.Target.CaptureMouse();
            }
        }

        public void MouseMoveDiagram(MouseEventArgs e)
        {
            if (Mouse.Captured != null)
            {
                movingClass = (FrameworkElement)e.MouseDevice.Target;

                if (movingClass is TextBox)
                {
                    return;
                }

                if (relativeMousePositionX == -1 && relativeMousePositionY == -1)
                {
                    relativeMousePositionX = (int)Mouse.GetPosition(movingClass).X;
                    relativeMousePositionY = (int)Mouse.GetPosition(movingClass).Y;
                }

                ClassViewModel movingDiagram = (ClassViewModel)movingClass.DataContext;

                canvas = FindParent<Canvas>(movingClass);
                Point mousePosition = Mouse.GetPosition(canvas);
                if (moveArrowPoint == default(Point)) moveArrowPoint = mousePosition;
                if ((int)mousePosition.X - relativeMousePositionX >= 0)
                    movingDiagram.X = (int)mousePosition.X - relativeMousePositionX;
                else
                    movingDiagram.X = 0;

                if ((int)mousePosition.Y - relativeMousePositionY >= 0)
                    movingDiagram.Y = (int)mousePosition.Y - relativeMousePositionY;
                else
                    movingDiagram.Y = 0;

                for (int i = 0; i < arrowCollection.Count; i++)
                {
                    if (movingDiagram == arrowCollection[i].DiagramA || movingDiagram == arrowCollection[i].DiagramB)
                        arrowCollection[i].newPath();
                }
            }
        }

        public void MouseUpDiagram(MouseEventArgs e)
        {
            FrameworkElement movingClass = (FrameworkElement)e.MouseDevice.Target;
            FocusedClass = (ClassViewModel)movingClass.DataContext;

            if (isAddingArrow)
            {
                if (startArrow == null)
                {
                    startArrow = FocusedClass;
                }
                else if (startArrow != FocusedClass)
                {
                    undoRedoController.AddAndExecute(new AddArrowCommand(arrowCollection, new ArrowViewModel(startArrow, FocusedClass, type)));
                    isAddingArrow = false;
                    startArrow = null;
                    type = EArrows.NORMAL;
                }
            }
            else
            {
                if (oldMousePositionPoint == e.GetPosition(FindParent<Canvas>((FrameworkElement)e.MouseDevice.Target)))
                {
                    e.MouseDevice.Target.ReleaseMouseCapture();
                    return;
                }

                ClassViewModel movingDiagram = (ClassViewModel)movingClass.DataContext;
                canvas = FindParent<Canvas>(movingClass);
                Point mousePosition = Mouse.GetPosition(canvas);

                int X, Y, oldX, oldY;
                if ((int)mousePosition.X - relativeMousePositionX >= 0)
                    X = (int)mousePosition.X - relativeMousePositionX;
                else
                    X = 0;
                if ((int)mousePosition.Y - relativeMousePositionY >= 0)
                    Y = (int)mousePosition.Y - relativeMousePositionY;
                else
                    Y = 0;

                if ((int)moveArrowPoint.X - relativeMousePositionX >= 0)
                    oldX = (int)moveArrowPoint.X - relativeMousePositionX;
                else
                    oldX = 0;
                if ((int)moveArrowPoint.Y - relativeMousePositionY >= 0)
                    oldY = (int)moveArrowPoint.Y - relativeMousePositionY;
                else
                    oldY = 0;

                undoRedoController.AddAndExecute(new MoveDiagramCommand(movingDiagram, X, Y, oldX, oldY, arrowCollection));
                moveArrowPoint = new Point();
                relativeMousePositionX = -1;
                relativeMousePositionY = -1;
                e.MouseDevice.Target.ReleaseMouseCapture();
            }
        }

        #endregion

        #region zoom

        public void ZoomOut()
        {
            if (Scale > 0.2)
            {
                Scale -= 0.1;
            }
        }

        public void ZoomIn()
        {
            if (Scale < 2.0)
            {
                Scale += 0.1;
            }
        }

        public void ZoomNorm()
        {
            Scale = 1;
        }

        #endregion

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }
}