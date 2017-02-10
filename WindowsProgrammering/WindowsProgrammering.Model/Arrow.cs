using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsProgrammering.Model
{
    public class Arrow : ViewModelBase
    {
        private Diagram endA;
        public Diagram EndA { get { return endA; } set { endA = value; } }

        private Diagram endB;
        public Diagram EndB { get { return endB; } set { endB = value; } }

        private EArrows type;
        public EArrows Type { get { return type; } set { type = value; } }
    }
}
