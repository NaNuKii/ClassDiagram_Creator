using GalaSoft.MvvmLight;
using System;


namespace WindowsProgrammering.Model
{
    public class Diagram : ViewModelBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int x;
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int y;
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        private String className;
        public String ClassName
        {
            get { return className; }
            set
            {
                className = value;
                RaisePropertyChanged(() => ClassName);
            }
        }
    }
}
