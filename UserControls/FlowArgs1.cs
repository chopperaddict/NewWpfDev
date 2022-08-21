using System;
//using DocumentFormat . OpenXml . Presentation;

namespace NewWpfDev . UserControls
{
    public class FlowArgs : EventArgs
    {
        public double Height
        {
            get; set;
        }
        public double Width
        {
            get; set;
        }
        public double CTop
        {
            get; set;
        }
        public double CLeft
        {
            get; set;
        }
        public double Xpos
        {
            get; set;
        }
        public double Ypos
        {
            get; set;
        }
        public bool BorderClicked
        {
            get; set;
        }
    }
}