﻿using System;
using System . Windows;
using System . Windows . Controls;
//using DocumentFormat . OpenXml . Presentation;

namespace NewWpfDev . UserControls
{
    public class SelectListboxArgs : EventArgs
    {
        public Canvas gcc;
        public bool IsOpen;
        public GenericGridControl grictrl;
        public Point InitPoint = new Point ( 50 , 20 );
        public object caller;
    }
}