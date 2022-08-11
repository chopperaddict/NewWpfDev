using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Diagnostics;
using System . Text;

namespace NewWpfDev
{
    public class DataGridLayout
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
        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional ( "DEBUG" )]
        [DebuggerStepThrough]
        public virtual void VerifyPropertyName ( string propertyName )
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if ( TypeDescriptor . GetProperties ( this ) [ propertyName ] == null )
            {
                string msg = "Invalid property name: " + propertyName;

                if ( this . ThrowOnInvalidPropertyName )
                    throw new Exception ( msg );
                else
                    Debug . Fail ( msg );
            }
        }

        //public void BeginEdit() => throw new NotImplementedException();
        //public void CancelEdit() => throw new NotImplementedException();
        //public void EndEdit() => throw new NotImplementedException();

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName
        {
            get; private set;
        }

        #endregion OnPropertyChanged

        private string fieldname;
        private string fieldtype;
        private int fieldlength;
        public string Fieldname
        {
            get { return fieldname; }
            set { fieldname = value; OnPropertyChanged ( nameof ( Fieldname ) ); }
        }
        public string Fieldtype
        {
            get { return fieldtype; }
            set { fieldtype = value; OnPropertyChanged ( nameof ( Fieldtype) ); }
        }
        public int Fieldlength
        {
            get { return fieldlength; }
            set { fieldlength = value; OnPropertyChanged ( nameof ( Fieldlength) ); }        
        }
        public DataGridLayout ( )
        {}


    }
}
