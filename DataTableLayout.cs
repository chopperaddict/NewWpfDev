using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Text;

namespace NewWpfDev
{
    public  class DataTableLayout
    {
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged ( string PropertyName )
        {
            if ( this . PropertyChanged != null )
            {
                var e = new PropertyChangedEventArgs ( PropertyName );
                this . PropertyChanged ( this , e );
            }
        }

        #endregion OnPropertyChanged

        private string fieldname;
        private string fieldtype;
        private int fieldlength;
        private int fielddec;
        private int fieldpart;
        private object datavalue;
        public string Fieldname
        {
            get
            {
                return fieldname;
            }
            set
            {
                fieldname = value; OnPropertyChanged ( nameof ( Fieldname ) );
            }
        }
        public string Fieldtype
        {
            get
            {
                return ( string ) fieldtype;
            }
            set
            {
                fieldtype = value; OnPropertyChanged ( nameof ( Fieldtype ) );
            }
        }
        public int Fieldlength
        {
            get
            {
                return fieldlength;
            }
            set
            {
                fieldlength = value; OnPropertyChanged ( nameof ( Fieldlength ) );
            }
        }
        public int Fielddec
        {
            get
            {
                return fielddec;
            }
            set
            {
                fielddec = value; OnPropertyChanged ( nameof ( Fielddec ) );
            }
        }
        public int Fieldpart
        {
            get
            {
                return fieldpart;
            }
            set
            {
                fieldpart = value; OnPropertyChanged ( nameof ( fieldpart ) );
            }
        }
        public object DataValue
        {
            get
            {
                return ( object ) datavalue;
            }
            set
            {
                datavalue = value; OnPropertyChanged ( nameof ( DataValue ) );
            }
        }

        public DataTableLayout ( )
        {
        }


    }
}

